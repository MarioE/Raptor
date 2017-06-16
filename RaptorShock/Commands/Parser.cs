using JetBrains.Annotations;

namespace RaptorShock.Commands
{
    /// <summary>
    ///     Represents a parser.
    /// </summary>
    /// <param name="s">The string to parse, which must not be <c>null</c>.</param>
    /// <returns>The result.</returns>
    [CanBeNull]
    public delegate object Parser([NotNull] string s);
}
