# sqlidetector


### SqliDetector

A simple **CLI tool** written in **C#** to detect potential **SQL Injection** issues by injecting payloads into URL query parameters and checking responses for common SQL error messages.

---

### Features

* Scan URLs from a file
* Custom payload support from file
* Short and long CLI options
* Common SQL error detection
* Lightweight and fast

---

### Usage

```bash
SqliDetector.exe -l urls.txt
SqliDetector.exe --list urls.txt --payload payloads.txt
SqliDetector.exe -l urls.txt -p payloads.txt
SqliDetector.exe -h
```

---

### Options

* `-l , --list <file>`
  File containing URLs (one per line)

* `-p , --payload <file>`
  File containing payloads (one per line)

* `-h , --help`
  Show help message

---

### Example

Input URL:

```
https://example.com/search?q=wow
```

Payload:

```
'
```

Tested URL:

```
https://example.com/search?q=wow'
```

If the response contains an SQL error → `True`
Otherwise → `False`

---

### Notes

* For **authorized testing only**
* No exploitation is performed
* Error-based detection only

---

### Requirements

* .NET 6.0 or newer
* Visual Studio or dotnet CLI

---

### Project Status

This tool is an **early-stage / basic project** intended as a starting point.
It will be **actively improved and expanded** over time with more features, better detection logic, and performance enhancements.

Current version focuses on **simple error-based detection only**.

---

### Disclaimer

This tool is intended for educational purposes and authorized security testing only. Any misuse is the responsibility of the user.

---

### License

MIT
