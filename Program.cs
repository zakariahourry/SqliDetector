using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0 || Has(args, "-h", "--help"))
        {
            Help();
            return;
        }

        string listFile = GetArg(args, "-l", "--list");
        if (listFile == null || !File.Exists(listFile)) return;

        string payloadFile = GetArg(args, "-p", "--payload");
        var payloads = payloadFile != null && File.Exists(payloadFile)
            ? new List<string>(File.ReadAllLines(payloadFile))
            : new List<string> { "'" };

        using var client = new HttpClient();

        foreach (var url in File.ReadAllLines(listFile))
        {
            bool vuln = false;
            foreach (var payload in payloads)
            {
                var testUrl = Inject(url, payload);
                try
                {
                    var body = await client.GetStringAsync(testUrl);
                    if (HasSqlError(body))
                    {
                        vuln = true;
                        break;
                    }
                }
                catch { }
            }
            Console.WriteLine($"{url} => {vuln}");
        }
    }

    static string Inject(string url, string payload)
    {
        var q = url.IndexOf('?');
        if (q == -1) return url;
        var baseUrl = url.Substring(0, q + 1);
        var query = url.Substring(q + 1);
        var parts = query.Split('&');
        var sb = new StringBuilder();
        for (int i = 0; i < parts.Length; i++)
        {
            var kv = parts[i].Split('=');
            if (kv.Length == 2)
                sb.Append(kv[0]).Append("=").Append(Uri.EscapeDataString(Uri.UnescapeDataString(kv[1]) + payload));
            else
                sb.Append(parts[i]);
            if (i < parts.Length - 1) sb.Append("&");
        }
        return baseUrl + sb.ToString();
    }

    static bool HasSqlError(string body)
    {
        string[] errors =
        {
            "SQL syntax",
            "mysql_",
            "ORA-",
            "ODBC",
            "Unclosed quotation mark",
            "SQLiteException",
            "PDOException"
        };
        foreach (var e in errors)
            if (body.IndexOf(e, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        return false;
    }

    static bool Has(string[] a, params string[] k)
    {
        foreach (var x in a)
            foreach (var y in k)
                if (x == y) return true;
        return false;
    }

    static string GetArg(string[] a, string s, string l)
    {
        for (int i = 0; i < a.Length - 1; i++)
            if (a[i] == s || a[i] == l)
                return a[i + 1];
        return null;
    }

    static void Help()
    {
        Console.WriteLine("Options:");
        Console.WriteLine("-l, --list <file>");
        Console.WriteLine("-p, --payload <file>");
        Console.WriteLine("-h, --help");
    }
}
