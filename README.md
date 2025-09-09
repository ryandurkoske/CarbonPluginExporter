# CarbonPluginExporter

---

# Carbon Plugin Exporter

**A simple CLI tool to export `.cs` plugin files and folders, from anywhere, to your (Carbon) Rust development server's `plugins` folder.**
It copies `.cs` files directly and automatically zips folders as `.cszip`, so you dont have to drag, drop or zip them into the plugins folder manually.

---

## Installation

```bash
git clone https://github.com/ryandurkoske/CarbonPluginExporter.git
cd CarbonPluginExporter
dotnet build -c Release
cd ../
```

Move the built executable wherever you want.  Makes most since inside your plugin project root directory or wherever your quick access dev scripts go. I like to rename it to 'export.exe'.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE.txt) for details.

---