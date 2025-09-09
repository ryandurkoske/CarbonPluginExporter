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
		string? oRoot = null;
		string plugins = "";
		string output = "server/carbon/plugins";
		bool q = false;

		if (!ParseArgs(args, targets, ref root, ref oRoot, ref plugins, ref output, ref q))
		{
			Environment.Exit(1);
		}

		string cwd = AppDomain.CurrentDomain.BaseDirectory;

        string? rootDir = root != null ? FindExistingParent(cwd, root) : cwd;
		if (rootDir == null)
		{
			ShowError($"Failed to find plugin root directory '{root}' from '{cwd}'");
			Environment.Exit(1);
		}

        string? oRootDir = oRoot != null ? FindExistingParent(cwd, oRoot) : cwd;
        if (oRootDir == null)
        {
            ShowError($"Failed to find server root directory '{oRoot}' from '{cwd}'");
            Environment.Exit(1);
        }

        string srcDir = Path.Combine(rootDir, plugins);
		string destDir = Path.GetFullPath(Path.Combine(oRootDir, output));

		if (!q) Console.WriteLine("Output Directory: " + destDir);

		foreach (var target in targets)
		{
			var src = Path.GetFullPath(Path.Combine(srcDir, target));
			if (!src.EndsWith(".cs") && Directory.Exists(src))
			{
				var dirName = new DirectoryInfo(src).Name;
				var zipName = dirName + ".cszip";
                if (!q) Console.WriteLine($"Exporting '{zipName}'");
                var dest = Path.Combine(destDir, zipName);
                File.Delete(dest);
                System.IO.Compression.ZipFile.CreateFromDirectory(src, dest);
			}
            else 
			{
                if (!src.EndsWith(".cs"))
				{
					src += ".cs";
                }
                var fileName = Path.GetFileName(src);
                if (File.Exists(src))
				{
                    if (!q) Console.WriteLine($"Exporting: {fileName}");
                    var dest = Path.Combine(destDir, Path.GetFileName(src));
                    File.Delete(dest);
                    File.Copy(src, dest);
				}
				else
				{
                    Console.Error.WriteLine($"Error: Plugin file or folder not found: {src}");
					Environment.Exit(1);
                }
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

	static bool ParseArgs(string[] args, List<string> targets, ref string? root, ref string? oroot, ref string plugins, ref string output, ref bool quiet)
	{
		for (int i = 0; i < args.Length; i++)
		{
			string arg = args[i];

			switch (arg)
			{
				case "-pr":
				case "--plugin-root":
					if (i + 1 < args.Length)
					{
						root = args[++i];
					}
					else
					{
						ShowError("Missing value for --plugin-root");
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

                case "-sr":
                case "--server-root":
                    if (i + 1 < args.Length)
                    {
                        oroot = args[++i];
                    }
                    else
                    {
                        ShowError("Missing value for --server-root");
                        return false;
                    }
                    break;

                case "-s":
				case "--server":
					if (i + 1 < args.Length)
					{
						output = args[++i];
					}
					else
					{
						ShowError("Missing value for --server");
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
  -pr, --plugin-root <name>	Root folder name to search up the directory tree for from cwd. (Useful for when running from the Visual Studio debugger.)
  -p,  --plugins <path>		Your development plugins folder relative to the found root otherwise cwd.
  -sr  --server-root <name>	Root folder name to search up the directory tree for from cwd.
  -s,  --server <path>		Your server's plugins folder relative to the found root otherwise cwd.
							Deault: server\carbon\plugins
  -h,  --help				Show this help information.
");
	}

	static void ShowError(string message)
	{
		Console.Error.WriteLine("Error: " + message);
		Console.WriteLine("Try -h for more information.");
	}
}