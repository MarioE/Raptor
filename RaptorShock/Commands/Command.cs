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

        /// <summary>
        ///     Initializes a new instance of the <see cref="Command" /> class with the specified name, syntax, help text, and
        ///     action.
        /// </summary>
        /// <param name="name">The name, which must not be <c>null</c>.</param>
        /// <param name="syntax">The syntax, which must not be <c>null</c>.</param>
        /// <param name="helpText">The help text.</param>
        /// <param name="action">The action, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="name" />, <paramref name="syntax" />, or <paramref name="action" /> is <c>null</c>.
        /// </exception>
        public Command([NotNull] string name, string syntax, string helpText, [NotNull] Action<string> action)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Syntax = syntax ?? throw new ArgumentNullException(nameof(syntax));
            HelpText = helpText;
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        ///     Gets the help text.
        /// </summary>
        [NotNull]
        public string HelpText { get; }

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
