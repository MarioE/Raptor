using System;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents a byte parser.
    /// </summary>
    public sealed class ByteParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return byte.TryParse(s, out var result) ? (object)result : null;
        }
    }
}
