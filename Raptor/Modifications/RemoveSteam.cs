using Mono.Cecil;

namespace Raptor.Modifications
{
    internal sealed class RemoveSteam : Modification
    {
        public override void Apply(AssemblyDefinition assembly)
        {
            var steam = assembly.GetType("SocialAPI");
            steam.GetMethod("Initialize").BlankOut();
            steam.GetMethod("Shutdown").BlankOut();
        }
    }
}
