using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;

namespace Raptor.Modifications
{
    internal sealed class PatchJson : Modification
    {
        public override void Apply(AssemblyDefinition assembly)
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
                module.Resources.Single(r => r.Name == "Terraria.Libraries.JSON.NET.Newtonsoft.Json.dll"));
        }
    }
}
