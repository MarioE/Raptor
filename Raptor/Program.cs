using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using Microsoft.Win32;
using Mono.Cecil;
using Raptor.Api;
using Raptor.Modifications;
using Terraria;

namespace Raptor
{
    internal static class Program
    {
        private static ClientApi _clientApi;
        private static Assembly _terrariaAssembly;

        [STAThread]
        private static void Main(string[] args)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Re-Logic\Terraria");
            var rootPath = (string)key?.GetValue("Install_Path", null);
            if (rootPath == null)
            {
                ShowError("Could not find Terraria installation path.");
                return;
            }
            var terrariaPath = Path.Combine(rootPath, "Terraria.exe");
            if (!File.Exists(terrariaPath))
            {
                ShowError("Could not find Terraria executable.");
                return;
            }
            if (File.Exists("Terraria.exe"))
            {
                ShowError("Raptor should not be placed in the same directory as Terraria");
                return;
            }

            // If necessary, request for administrative privileges and create symlinks to the Content folder and the
            // native DLL. This is necessary because on Windows, only administrators can create symlinks by default.
            // We use symlinks instead of hard links, as they are more versatile and can link across drives.
            if (args.Length == 1 && args[0] == "setup")
            {
                NativeMethods.CreateSymbolicLink("Content", Path.Combine(rootPath, "Content"), 1);
                NativeMethods.CreateSymbolicLink("ReLogic.Native.dll", Path.Combine(rootPath, "ReLogic.Native.dll"), 0);
                return;
            }
            if (!Directory.Exists("Content") || !File.Exists("ReLogic.Native.dll"))
            {
                using (var process = new Process())
                {
                    process.StartInfo =
                        new ProcessStartInfo(Assembly.GetEntryAssembly().Location, "setup") {Verb = "runas"};
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                    }
                    catch (Win32Exception)
                    {
                        ShowError("Could not create symbolic links, as permission was not given.");
                        return;
                    }
                    if (!Directory.Exists("Content") || !File.Exists("ReLogic.Native.dll"))
                    {
                        ShowError("Could not create symbolic links.");
                        return;
                    }
                }
            }

            var assembly = AssemblyDefinition.ReadAssembly(terrariaPath);
            var modifications = from t in Assembly.GetExecutingAssembly().GetTypes()
                                where t.IsSubclassOf(typeof(Modification))
                                select (Modification)Activator.CreateInstance(t);
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

            // Resolve assemblies packed into the Terraria assembly. This is necessary since we're not using
            // WindowsLaunch to launch the game.
            var resourceName = new AssemblyName(args.Name).Name + ".dll";
            resourceName = _terrariaAssembly.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(resourceName));
            if (resourceName == null)
            {
                return null;
            }

            using (var stream = _terrariaAssembly.GetManifestResourceStream(resourceName))
            {
                // ReSharper disable once PossibleNullReferenceException
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Assembly.Load(bytes);
            }
        }

        [UsedImplicitly]
        private static void OnLaunch(object main)
        {
            _clientApi = new ClientApi();
            _clientApi.LoadPlugins((Main)main);
        }

        private static void Run(string[] args)
        {
            Terraria.Program.LaunchGame(args);
            _clientApi.Dispose();
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
