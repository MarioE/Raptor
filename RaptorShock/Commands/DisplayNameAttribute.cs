using System;
using JetBrains.Annotations;

namespace RaptorShock.Commands
{
    /// <summary>
    ///     Specifies the display name of a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DisplayNameAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisplayNameAttribute" /> class with the specified name.
        /// </summary>
        /// <param name="name">The name, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <c>null</c>.</exception>
        public DisplayNameAttribute([NotNull] string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        [NotNull]
        public string Name { get; }
    }
}
