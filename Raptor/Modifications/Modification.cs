using System;
using JetBrains.Annotations;
using Mono.Cecil;

namespace Raptor.Modifications
{
    /// <summary>
    ///     Specifies a modification to be applied to the Terraria assembly.
    /// </summary>
    [UsedImplicitly]
    public abstract class Modification
    {
        /// <summary>
        ///     Gets the order of the modification. Modifications will be applied in increasing order.
        /// </summary>
        public virtual int Order => 0;

        /// <summary>
        ///     Applies the modification.
        /// </summary>
        /// <param name="assembly">The assembly, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assembly" /> is <c>null</c>.</exception>
        public void Apply([NotNull] AssemblyDefinition assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            ApplyImpl(assembly);
        }

        /// <summary>
        ///     Applies the modification.
        /// </summary>
        /// <param name="assembly">The assembly, which must not be <c>null</c>.</param>
        protected abstract void ApplyImpl([NotNull] AssemblyDefinition assembly);
    }
}
