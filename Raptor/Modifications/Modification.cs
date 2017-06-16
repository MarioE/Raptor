using JetBrains.Annotations;
using Mono.Cecil;

namespace Raptor.Modifications
{
    [UsedImplicitly]
    internal abstract class Modification
    {
        public abstract void Apply(AssemblyDefinition assembly);
    }
}
