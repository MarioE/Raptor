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
            var projectiles = Utils.GetProjectilesByNameOrId(s);
            return projectiles.Count == 1 ? projectiles[0] : null;
        }
    }
}
