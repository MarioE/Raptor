using Mono.Cecil;

namespace Raptor.Modifications
{
    /// <summary>
    ///     Represents a modification that removes steam.
    /// </summary>
    public sealed class RemoveSteam : Modification
    {
        /// <inheritdoc />
        protected override void ApplyImpl(AssemblyDefinition assembly)
        {
            var steam = assembly.GetType("SocialAPI");
            steam.GetMethod("Initialize").BlankOut();
            steam.GetMethod("Shutdown").BlankOut();
        }
    }
}
