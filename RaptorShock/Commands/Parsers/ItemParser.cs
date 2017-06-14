using System;
using Terraria;

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
            var item = new Item();
            if (int.TryParse(s, out var id) && id > 0 && id < Main.maxItemTypes)
            {
                item.SetDefaults(id);
                return item;
            }

            for (var i = 1; i < Main.maxItemTypes; ++i)
            {
                item.SetDefaults(i);
                if (item.Name?.StartsWith(s, StringComparison.CurrentCultureIgnoreCase) ?? false)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
