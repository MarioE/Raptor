using System;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents a string parser.
    /// </summary>
    public sealed class StringParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return s;
        }
    }
}
