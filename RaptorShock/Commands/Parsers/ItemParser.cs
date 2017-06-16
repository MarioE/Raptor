namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents an item parser.
    /// </summary>
    public sealed class ItemParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            var items = Utils.GetItemsByNameOrId(s);
            return items.Count == 1 ? items[0] : null;
        }
    }
}
