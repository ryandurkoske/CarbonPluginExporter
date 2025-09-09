using System;
using System.Collections.Generic;
using System.IO;

/**
 * Export Rust Carbon plugins to your servers plugin folder for hot reloading.
 * 
 * QoL tips for running with Visual Studio debugger:
 *  Enable Tools > Options > Debugger > Automatically close the console when debugging stops
 *  Disable Tools > Options > Projects and Solutions > Show output window when build starts
 *  
 * Author: Durk
 * Date: 9/9/2025
 */

class Program
{
    static void Main(string[] args)
    {
        List<string> targets = new();
        string? root = null;
        string plugins = "";
        string output = "server\\carbon\\plugins";
        bool q = false;

        if (!ParseArgs(args, targets, ref root, ref plugins, ref output, ref q))
        {
            Environment.Exit(1);
        }

        string? rootDir = root != null ? FindExistingParent(".", root) : ".";
        if (rootDir == null)
        {
            ShowError($"Failed to find root directory '{root}' from '{AppDomain.CurrentDomain.BaseDirectory}'");
            Environment.Exit(1);
        }

        string srcDir = Path.Combine(rootDir, plugins);
        string destDir = Path.Combine(rootDir, output);

        // Example: print parsed values
        if (!q) Console.WriteLine("Output Directory: " + destDir);

        foreach (var target in targets)
        {
            var src = Path.Combine(srcDir, target);
            if (target.EndsWith(".cs"))
            {
                if (!q) Console.WriteLine($"Exporting '{target}'");
                var dest = Path.Combine(destDir, Path.GetFileName(target));
                File.Delete(dest);
                File.Copy(src, dest);
            }
            else
            {
                if (!q) Console.WriteLine($"Exporting '{target}.cszip'");
                var dest = Path.Combine(destDir, Path.GetDirectoryName(target) + ".cszip");
                File.Delete(dest);
                System.IO.Compression.ZipFile.CreateFromDirectory(src, dest);
            }
        }
    }

    static string? FindExistingParent(string startPath, string folder)
    {
        var directory = new DirectoryInfo(startPath);

        // Loop backward until the parent is null (meaning we've reached the root)
        // or we find an existing directory.
        while (directory.Name != folder)
        {
            directory = directory.Parent;
            if (directory == null || !directory.Exists)
            {
                return null;
            }
        }

        // Return the full path of the found directory, or null if none was found.
        return directory.FullName;
    }

    static bool ParseArgs(string[] args, List<string> targets, ref string? root, ref string plugins, ref string output, ref bool quiet)
    {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            switch (arg)
            {
                case "-r":
                case "--root":
                    if (i + 1 < args.Length)
                    {
                        root = args[++i];
                    }
                    else
                    {
                        ShowError("Missing value for --root");
                        return false;
                    }
                    break;

                case "-p":
                case "--plugins":
                    if (i + 1 < args.Length)
                    {
                        plugins = args[++i];
                    }
                    else
                    {
                        ShowError("Missing value for --plugins");
                        return false;
                    }
                    break;

                case "-o":
                case "--output":
                    if (i + 1 < args.Length)
                    {
                        output = args[++i];
                    }
                    else
                    {
                        ShowError("Missing value for --output");
                        return false;
                    }
                    break;

                case "-q":
                case "--quiet":
                    quiet = true;
                    break;

                case "-h":
                case "--help":
                    ShowHelp();
                    return false;

                default:
                    if (arg.StartsWith("-"))
                    {
                        ShowError($"Unknown option: {arg}");
                        return false;
                    }
                    else
                    {
                        targets.Add(arg);
                    }
                    break;
            }
        }

        if (targets.Count == 0)
        {
            ShowError("No files or folders specified.");
            return false;
        }

        return true;
    }

    static void ShowHelp()
    {
        Console.WriteLine(@"
Usage:
  mytool export [files_or_folders...] [options]

Description:
  Exports a list of files or folders to a new location.
  - .cs files are copied directly.
  - Folders are compressed as .cszip files.

Options:
  -r, --root <name>		Root folder name to search up the directory tree for from cwd. (Useful for when running from the Visual Studio debugger.)
  -p, --plugins <path>	Your development plugins folder relative to the found root otherwise cwd.
  -o, --output <path>	Your server plugins folder relative to the found root otherwise cwd.
						Deault: server\carbon\plugins
  -h, --help			Show this help information.
");
    }

    static void ShowError(string message)
    {
        Console.Error.WriteLine("Error: " + message);
        Console.WriteLine("Try -h for more information.");
    }
}



