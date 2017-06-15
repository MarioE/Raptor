using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;

namespace Raptor.Modifications
{
    /// <summary>
    ///     Represents a modification that patches the Newtonsoft.JSON version.
    /// </summary>
    public sealed class PatchJson : Modification
    {
        /// <inheritdoc />
        protected override void ApplyImpl(AssemblyDefinition assembly)
        {
            var module = assembly.MainModule;
            foreach (var reference in module.AssemblyReferences)
            {
                if (reference.Name == "Newtonsft.Json")
                {
                    reference.Version = typeof(JsonConvert).Assembly.GetName().Version;
                }
            }

            module.Resources.Remove(
                module.Resources.Single(x => x.Name == "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll"));
        }
    }
}
