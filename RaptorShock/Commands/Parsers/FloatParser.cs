using System;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents a float parser.
    /// </summary>
    public sealed class FloatParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return float.TryParse(s, out var result) ? (object)result : null;
        }
    }
}
