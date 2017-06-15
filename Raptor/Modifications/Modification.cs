using JetBrains.Annotations;
using Mono.Cecil;

namespace Raptor.Modifications
{
    [UsedImplicitly]
    internal abstract class Modification
    {
        public virtual int Order => 0;

        public abstract void Apply(AssemblyDefinition assembly);
    }
}
