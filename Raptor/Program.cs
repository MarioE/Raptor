using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using Mono.Cecil;
using Raptor.Api;
using Raptor.Modifications;
using Terraria;

namespace Raptor
{
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        private static string _rootPath;
        private static Assembly _terrariaAssembly;

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [STAThread]
        private static void Main(string[] args)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Re-Logic\Terraria");
            _rootPath = (string)key?.GetValue("Install_Path", null);
            if (_rootPath == null)
            {
                ShowError("Could not find Terraria installation path.");
                return;
            }
            
            if (args.Length == 1 && args[0] == "setup")
            {
                CreateSymbolicLink("Content", Path.Combine(_rootPath, "Content"), 1);
                CreateSymbolicLink("ReLogic.Native.dll", Path.Combine(_rootPath, "ReLogic.Native.dll"), 0);
                return;
            }
            if (!Directory.Exists("Content") || !File.Exists("ReLogic.Native.dll"))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, "setup")
                    {
                        Verb = "runas"
                    }
                };
                process.Start();
                process.WaitForExit();
            }

            var terrariaPath = Path.Combine(_rootPath, "Terraria.exe");
            if (!File.Exists(terrariaPath))
            {
                ShowError("Could not find Terraria executable.");
                return;
            }
            
            var assembly = AssemblyDefinition.ReadAssembly(terrariaPath);
            var modifications = from t in Assembly.GetExecutingAssembly().GetExportedTypes()
                                where t.IsSubclassOf(typeof(Modification))
                                let m = (Modification)Activator.CreateInstance(t)
                                orderby m.Order
                                select m;
            foreach (var modification in modifications)
            {
                modification.Apply(assembly);
            }

            using (var stream = new MemoryStream())
            {
                assembly.Write(stream);
#if DEBUG
                assembly.Write("debug.exe");
#endif
                _terrariaAssembly = Assembly.Load(stream.ToArray());
            }

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            Run(args);
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name;
            var shortName = name.Split(',')[0];
            if (shortName == "Terraria")
            {
                return _terrariaAssembly;
            }

            Directory.CreateDirectory("plugins");
            foreach (var pluginPath in Directory.EnumerateDirectories("plugins", "*.dll"))
            {
                try
                {
                    if (name == AssemblyName.GetAssemblyName(pluginPath).FullName)
                    {
                        return Assembly.LoadFrom(pluginPath);
                    }
                }
                catch (BadImageFormatException)
                {
                }
            }
            return null;
        }

        private static void Run(string[] args)
        {
            using (var clientApi = new ClientApi())
            {
                clientApi.LoadPlugins();
                WindowsLaunch.Main(args);
            }
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
