using System;
using JetBrains.Annotations;

namespace RaptorShock.Commands
{
    /// <summary>
    ///     Specifies that a method is a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandAttribute" /> class with the specified name and syntax.
        /// </summary>
        /// <param name="name">The name, which must not be <c>null</c>.</param>
        /// <param name="syntax">The synatx, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="name" /> or <paramref name="syntax" />is <c>null</c>.
        /// </exception>
        public CommandAttribute([NotNull] string name, [NotNull] string syntax)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
        }

        /// <summary>
        /// Gets or sets the aliases.
        /// </summary>
        [CanBeNull]
        public string[] Aliases { get; set; }

        /// <summary>
        ///     Gets or sets the help text.
        /// </summary>
        [CanBeNull]
        public string HelpText { get; set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        ///     Gets the syntax.
        /// </summary>
        [NotNull]
        public string Syntax { get; }
    }
}
