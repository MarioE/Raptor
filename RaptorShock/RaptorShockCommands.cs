using System;
using System.Linq;
using Microsoft.Xna.Framework;
using RaptorShock.Commands;
using Terraria;
using Terraria.ID;

namespace RaptorShock
{
    internal sealed class RaptorShockCommands
    {
        public int? DefenseValue { get; private set; }
        public bool IsFullBright { get; private set; }
        public bool IsGodMode { get; private set; }
        public bool IsInfiniteAmmo { get; private set; }
        public bool IsInfiniteBreath { get; private set; }
        public bool IsInfiniteHealth { get; private set; }
        public bool IsInfiniteMana { get; private set; }
        public bool IsInfiniteWings { get; private set; }
        public bool IsNoclip { get; private set; }
        public Vector2 NoclipPosition { get; set; }
        public int? RangeValue { get; private set; }
        public float? SpeedValue { get; private set; }

        [Command("anitime", ".anitime <animation-time>",
            HelpText = "Sets your selected item's animation time.",
            Alias = "at")]
        public void AniTime(int animationTime)
        {
            if (animationTime <= 0)
            {
                Utils.ShowErrorMessage($"Invalid animation time '{animationTime}'.");
                return;
            }

            Utils.LocalPlayerItem.useAnimation = animationTime;
            Utils.ShowSuccessMessage($"Set animation time to '{animationTime}'.");
        }

        [Command("autoreuse", ".autoreuse",
            HelpText = "Toggles your selected item's autoreuse.",
            Alias = "ar")]
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

        [Command("defense", ".defense <defense>",
            HelpText = "Sets your defense.")]
        public void Defense(int defense)
        {
            if (defense <= 0)
            {
                Utils.ShowErrorMessage($"Invalid defense '{defense}'.");
                return;
            }

            DefenseValue = defense;
            Utils.ShowSuccessMessage($"Set defense to '{defense}'.");
        }

        [Command("fullbright", ".fullbright",
            HelpText = "Toggles fullbright mode.",
            Alias = "fb")]
        public void FullBright()
        {
            IsFullBright = !IsFullBright;
            Utils.ShowSuccessMessage($"{(IsFullBright ? "En" : "Dis")}abled fullbright mode.");
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
                    Utils.ShowInfoMessage(command.Alias == null
                        ? $"{command.Name}: {command.HelpText}"
                        : $"{command.Name} (alias: {command.Alias}): {command.HelpText}");
                }
                return;
            }

            var command2 =
                commands.FirstOrDefault(c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                                             (c.Alias?.Equals(commandName, StringComparison.OrdinalIgnoreCase) ??
                                              false));
            if (command2 == null)
            {
                Utils.ShowErrorMessage($"Invalid command '{commandName}'.");
                return;
            }

            Utils.ShowInfoMessage($"Syntax: {command2.Syntax}");
            Utils.ShowInfoMessage(command2.HelpText ?? "No help text available.");
        }

        [Command("infammo", ".infammo",
            HelpText = "Toggles infinite ammo.")]
        public void InfiniteAmmo()
        {
            IsInfiniteAmmo = !IsInfiniteAmmo;
            Utils.ShowSuccessMessage($"{(IsInfiniteAmmo ? "En" : "Dis")}abled infinite ammo.");
        }

        [Command("infbreath", ".infbreath",
            HelpText = "Toggles infinite breath.")]
        public void InfiniteBreath()
        {
            IsInfiniteBreath = !IsInfiniteBreath;
            Utils.ShowSuccessMessage($"{(IsInfiniteBreath ? "En" : "Dis")}abled infinite breath.");
        }

        [Command("infhealth", ".infhealth",
            HelpText = "Toggles infinite health.")]
        public void InfiniteHealth()
        {
            IsInfiniteHealth = !IsInfiniteHealth;
            Utils.ShowSuccessMessage($"{(IsInfiniteHealth ? "En" : "Dis")}abled infinite health.");
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
            Alias = "i")]
        public void Item(Item item, int? stackSize = null, byte prefix = 0)
        {
            if (stackSize == null)
            {
                stackSize = item.maxStack;
            }
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
            item.stack = stackSize.Value;
            item.position = player.Center;
            item.Prefix(prefix);

            player.GetItem(player.whoAmI, item);
            Utils.ShowSuccessMessage($"Spawned {stackSize} {item.Name}(s).");
        }

        [Command("noclip", ".noclip",
            HelpText = "Toggles noclip.",
            Alias = "nc")]
        public void Noclip()
        {
            IsNoclip = !IsNoclip;
            if (IsNoclip)
            {
                NoclipPosition = Utils.LocalPlayer.position;
            }
            Utils.ShowSuccessMessage($"{(IsNoclip ? "En" : "Dis")}abled noclip.");
        }

        [Command("projectile", ".projectile <projectile-name>",
            HelpText = "Sets your selected item's projectile.")]
        public void Projectile(Projectile projectile)
        {
            Utils.LocalPlayerItem.shoot = projectile.type;
            Utils.ShowSuccessMessage($"Set projectile to {projectile.Name}.");
        }

        [Command("range", ".range <range>",
            HelpText = "Sets your range.")]
        public void Range(int range)
        {
            if (range <= 0)
            {
                Utils.ShowErrorMessage($"Invalid range '{range}'.");
                return;
            }

            RangeValue = range;
            Utils.ShowSuccessMessage($"Set range to '{range}'.");
        }

        [Command("reset", ".reset",
            HelpText = "Resets all modified states.")]
        public void Reset()
        {
            DefenseValue = null;
            IsFullBright = IsGodMode = IsInfiniteAmmo = IsInfiniteBreath =
                IsInfiniteHealth = IsInfiniteMana = IsInfiniteWings = IsNoclip = false;
            RangeValue = null;
            SpeedValue = null;
            Utils.ShowSuccessMessage("Reset everything.");
        }

        [Command("shootspeed", ".shootspeed <shoot-speed>",
            HelpText = "Sets your selected item's projectile speed.",
            Alias = "ss")]
        public void ShootSpeed(float shootSpeed)
        {
            if (shootSpeed < 0.0f)
            {
                Utils.ShowErrorMessage($"Invalid speed '{shootSpeed}'.");
                return;
            }

            Utils.LocalPlayerItem.shootSpeed = shootSpeed;
            Utils.ShowSuccessMessage($"Set shoot speed to '{shootSpeed}'.");
        }

        [Command("speed", ".speed <speed>",
            HelpText = "Sets your speed value.")]
        public void Speed(float speed)
        {
            if (speed < 0.0f)
            {
                Utils.ShowErrorMessage($"Invalid speed '{speed}'.");
                return;
            }

            SpeedValue = speed;
            Utils.ShowSuccessMessage($"Set speed to '{speed}'.");
        }

        [Command("usetime", ".usetime <use-time>",
            HelpText = "Sets your selected item's use time.",
            Alias = "ut")]
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
