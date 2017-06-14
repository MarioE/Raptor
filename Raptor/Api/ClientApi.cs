using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace Raptor.Api
{
    internal sealed class ClientApi : IDisposable
    {
        private static readonly Version ApiVersion = new Version(1, 0);

        private readonly ILog _log = LogManager.GetLogger("API");
        private readonly List<TerrariaPlugin> _plugins = new List<TerrariaPlugin>();

        /// <summary>
        ///     Disposes the client API.
        /// </summary>
        public void Dispose()
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.Dispose();
                    _log.Info($"Disposed plugin '{plugin.Name}'.");
                }
                catch (Exception ex)
                {
                    _log.Error($"An exception occurred while unloading the plugin '{plugin.Name}':");
                    _log.Error(ex);
                }
            }
        }
        
        /// <summary>
        ///     Loads the plugins.
        /// </summary>
        public void LoadPlugins()
        {
            Directory.CreateDirectory("plugins");
            foreach (var pluginPath in Directory.EnumerateFiles("plugins", "*.dll"))
            {
                try
                {
                    var assembly = Assembly.Load(File.ReadAllBytes(pluginPath));
                    var pluginTypes = from t in assembly.GetExportedTypes()
                                      where t.IsSubclassOf(typeof(TerrariaPlugin)) && !t.IsAbstract
                                      select t;
                    foreach (var pluginType in pluginTypes)
                    {
                        var apiVersionAttributes = pluginType.GetCustomAttributes(typeof(ApiVersionAttribute), false);
                        if (apiVersionAttributes.Length == 0)
                        {
                            _log.Error(
                                $"Plugin '{pluginType.FullName}' has no API version attribute and was ignored.");
                            continue;
                        }

                        var apiVersion = ((ApiVersionAttribute)apiVersionAttributes[0]).ApiVersion;
                        if (apiVersion.Major != ApiVersion.Major || apiVersion.Minor != ApiVersion.Minor)
                        {
                            _log.Error(
                                $"Plugin '{pluginType.FullName}' is designed for a different API version and was ignored.");
                            continue;
                        }

                        try
                        {
                            _plugins.Add((TerrariaPlugin)Activator.CreateInstance(pluginType));
                            _log.Info($"Loaded plugin '{pluginType.FullName}'.");
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"An exception occurred while loading the plugin '{pluginType.FullName}':");
                            _log.Error(ex);
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                }
                catch (Exception ex)
                {
                    _log.Error($"An exception occurred while loading the assembly '{pluginPath}':");
                    _log.Error(ex);
                }
            }
        }
    }
}
