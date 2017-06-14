using System;
using System.Linq;
using RaptorShock.Commands;
using Terraria;
using Terraria.ID;

namespace RaptorShock
{
    /// <summary>
    ///     Holds the RaptorShock commands.
    /// </summary>
    public sealed class RaptorShockCommands
    {
        /// <summary>
        ///     Gets a value indicating whether god mode is toggled.
        /// </summary>
        public bool IsGodMode { get; private set; }

        /// <summary>
        ///     Gets the speed value.
        /// </summary>
        public float? SpeedValue { get; private set; }

        [Command("godmode", ".godmode", HelpText = "Toggles god mode.")]
        public void GodMode()
        {
            IsGodMode = !IsGodMode;
            Utils.ShowSuccessMessage($"{(IsGodMode ? "En" : "Dis")}abled god mode.");
        }

        [Command("help", ".help [command-name]", HelpText = "Provides help about a command.")]
        public void Help(string commandName = null)
        {
            var commands = RaptorShockPlugin.Instance.CommandManager.Commands;
            if (commandName == null)
            {
                Utils.ShowSuccessMessage("Available commands:");
                Utils.ShowInfoMessage(string.Join(", ", commands.Select(c => $".{c.Name}")));
                return;
            }

            var command = commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
            if (command == null)
            {
                Utils.ShowErrorMessage($"Invalid command '{commandName}'.");
                return;
            }

            Utils.ShowInfoMessage($"Syntax: {command.Syntax}");
            Utils.ShowInfoMessage(command.HelpText);
        }

        [Command("item", ".item <item-name> [stack-size] [prefix]", HelpText = "Spawns an item.")]
        public void Item(Item item, [DisplayName("stack size")] int stackSize = 1, byte prefix = 0)
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
            Utils.ShowSuccessMessage($"Spawned {stackSize} {item.Name}(s).");
        }

        [Command("speed", ".speed [speed]", HelpText = "Sets your speed value.")]
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
    }
}
