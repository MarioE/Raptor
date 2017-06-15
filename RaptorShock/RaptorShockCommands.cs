using System;
using System.Linq;
using RaptorShock.Commands;
using Terraria;
using Terraria.ID;

namespace RaptorShock
{
    internal sealed class RaptorShockCommands
    {
        public bool IsGodMode { get; private set; }
        public bool IsInfiniteMana { get; private set; }
        public bool IsInfiniteWings { get; private set; }
        public float? SpeedValue { get; private set; }

        [Command("autoreuse", ".autoreuse",
            HelpText = "Toggles your selected item's autoreuse.",
            Aliases = new[] {"ar"})]
        public void AutoReuse()
        {
            Utils.LocalPlayerItem.autoReuse = !Utils.LocalPlayerItem.autoReuse;
            Utils.ShowSuccessMessage($"{(Utils.LocalPlayerItem.autoReuse ? "En" : "Dis")}abled autoreuse.");
        }

        [Command("damage", ".damage <damage>",
            HelpText = "Sets your selected item's damage.")]
        public void Damage(int damage)
        {
            if (damage <= 0)
            {
                Utils.ShowErrorMessage($"Invalid damage '{damage}'.");
                return;
            }

            Utils.LocalPlayerItem.damage = damage;
            Utils.ShowSuccessMessage($"Set damage to '{damage}'.");
        }

        [Command("godmode", ".godmode",
            HelpText = "Toggles god mode.")]
        public void GodMode()
        {
            IsGodMode = !IsGodMode;
            Utils.ShowSuccessMessage($"{(IsGodMode ? "En" : "Dis")}abled god mode.");
        }

        [Command("help", ".help [command-name]",
            HelpText = "Provides help about a command.")]
        public void Help(string commandName = null)
        {
            var commands = RaptorShockPlugin.Instance.CommandManager.Commands;
            if (commandName == null)
            {
                Utils.ShowSuccessMessage("Available commands:");
                foreach (var command in commands)
                {
                    Utils.ShowInfoMessage(command.Aliases == null
                        ? $"{command.Name}: {command.HelpText}"
                        : $"{command.Name} (aliases: {string.Join(", ", command.Aliases)}): {command.HelpText}");
                }
                return;
            }

            var command2 =
                commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                                             (c.Aliases?.Any(a => a.Equals(commandName,
                                                  StringComparison.OrdinalIgnoreCase)) ?? false));
            if (command2 == null)
            {
                Utils.ShowErrorMessage($"Invalid command '{commandName}'.");
                return;
            }

            Utils.ShowInfoMessage($"Syntax: {command2.Syntax}");
            Utils.ShowInfoMessage(command2.HelpText ?? "No help text available.");
        }

        [Command("infmana", ".infmana",
            HelpText = "Toggles infinite mana.")]
        public void InfiniteMana()
        {
            IsInfiniteMana = !IsInfiniteMana;
            Utils.ShowSuccessMessage($"{(IsInfiniteMana ? "En" : "Dis")}abled infinite mana.");
        }

        [Command("infwings", ".infwings",
            HelpText = "Toggles infinite wings.")]
        public void InfiniteWings()
        {
            IsInfiniteWings = !IsInfiniteWings;
            Utils.ShowSuccessMessage($"{(IsInfiniteWings ? "En" : "Dis")}abled infinite wings.");
        }

        [Command("item", ".item <item-name> [stack-size] [prefix]",
            HelpText = "Spawns an item.",
            Aliases = new[] {"i"})]
        public void Item(Item item, int stackSize = 1, byte prefix = 0)
        {
            if (stackSize <= 0 || stackSize > item.maxStack)
            {
                Utils.ShowErrorMessage($"Invalid stack size '{stackSize}'.");
                return;
            }
            if (prefix > PrefixID.Count)
            {
                Utils.ShowErrorMessage($"Invalid prefix '{prefix}'.");
                return;
            }

            var player = Utils.LocalPlayer;
            item.stack = stackSize;
            item.position = player.Center;
            item.Prefix(prefix);

            player.GetItem(player.whoAmI, item);
            Utils.ShowSuccessMessage($"Spawned {stackSize} {item.Name.ToLower()}(s).");
        }

        [Command("speed", ".speed [speed]",
            HelpText = "Sets your speed value.")]
        public void Speed(float? speed = null)
        {
            if (speed == null)
            {
                SpeedValue = null;
                Utils.ShowSuccessMessage("Reset speed.");
            }
            else
            {
                if (speed < 0.0f)
                {
                    Utils.ShowErrorMessage($"Invalid speed '{speed}'.");
                    return;
                }

                SpeedValue = speed;
                Utils.ShowSuccessMessage($"Set speed to '{speed}'.");
            }
        }

        [Command("usetime", ".usetime <use-time>",
            HelpText = "Sets your selected item's use time.",
            Aliases = new[] {"ut"})]
        public void UseTime(int useTime)
        {
            if (useTime <= 0)
            {
                Utils.ShowErrorMessage($"Invalid use time '{useTime}'.");
                return;
            }

            Utils.LocalPlayerItem.useTime = useTime;
            Utils.ShowSuccessMessage($"Set use time to '{useTime}'.");
        }
    }
}
