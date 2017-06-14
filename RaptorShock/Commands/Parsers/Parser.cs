using JetBrains.Annotations;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Specifies a parser that parses a string.
    /// </summary>
    public abstract class Parser
    {
        /// <summary>
        ///     Parses the specified string.
        /// </summary>
        /// <param name="s">The string to parse, which must not be <c>null</c>.</param>
        /// <returns>The result.</returns>
        [CanBeNull]
        public abstract object Parse([NotNull] string s);
    }
}
