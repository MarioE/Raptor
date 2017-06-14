using System;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents an integer parser.
    /// </summary>
    public sealed class IntParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return int.TryParse(s, out var result) ? (object)result : null;
        }
    }
}
