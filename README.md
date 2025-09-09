# CarbonPluginExporter

---

# Carbon Plugin Exporter

**A simple CLI tool to export `.cs` plugin files and folders for Carbon Rust servers.**
It copies `.cs` files directly and automatically zips folders as `.cszip`, making it easy to hot reload during development.

---

## Installation

1. Clone the repository:

```bash
git clone https://github.com/ryandurkoske/CarbonPluginExporter.git
```

2. Build the project using .NET SDK:

```bash
dotnet build
```

3. Move the built executable wherever you want.  Makes most since inside your plugin project root directory or wherever your quick access dev scripts go. I like to rename it to 'export.exe'.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE.txt) for details.

---