using System;
using Terraria;

namespace RaptorShock.Commands.Parsers
{
    /// <summary>
    ///     Represents a projectile parser.
    /// </summary>
    public sealed class ProjectileParser : Parser
    {
        /// <inheritdoc />
        public override object Parse(string s)
        {
            var projectile = new Projectile();
            if (int.TryParse(s, out var id) && id > 0 && id < Main.maxProjectileTypes)
            {
                projectile.SetDefaults(id);
                return projectile;
            }

            for (var i = 1; i < Main.maxProjectileTypes; ++i)
            {
                projectile.SetDefaults(i);
                if (projectile.Name?.Equals(s, StringComparison.CurrentCultureIgnoreCase) ?? false)
                {
                    return projectile;
                }
            }
            return null;
        }
    }
}
