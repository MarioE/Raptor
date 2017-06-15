using System;
using JetBrains.Annotations;

namespace RaptorShock.Commands
{
    /// <summary>
    ///     Represents a command.
    /// </summary>
    public sealed class Command
    {
        private readonly Action<string> _action;
        private readonly CommandAttribute _attribute;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class with the specified attribute and action.
        /// </summary>
        /// <param name="attribute">The name, which must not be <c>null</c>.</param>
        /// <param name="action">The action, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="attribute" /> or <paramref name="action" /> is <c>null</c>.
        /// </exception>
        public Command([NotNull] CommandAttribute attribute, [NotNull] Action<string> action)
        {
            _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        [CanBeNull]
        public string Alias => _attribute.Alias;

        /// <summary>
        /// Gets the help text.
        /// </summary>
        [CanBeNull]
        public string HelpText => _attribute.HelpText;

        /// <summary>
        /// Gets the name.
        /// </summary>
        [NotNull]
        public string Name => _attribute.Name;

        /// <summary>
        /// Gets the syntax.
        /// </summary>
        [NotNull]
        public string Syntax => _attribute.Syntax;
        
        /// <summary>
        ///     Invokes the command using the specified string.
        /// </summary>
        /// <param name="s">The string, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
        public void Invoke([NotNull] string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            _action(s);
        }
    }
}
