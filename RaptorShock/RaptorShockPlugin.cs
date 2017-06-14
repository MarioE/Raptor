using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Microsoft.Xna.Framework.Input;
using Raptor;
using Raptor.Api;
using Raptor.Hooks;
using Raptor.Hooks.Events.Player;
using RaptorShock.Commands;
using Terraria;
using Terraria.Localization;

namespace RaptorShock
{
    /// <summary>
    ///     Represents the RaptorShock plugin.
    /// </summary>
    [ApiVersion(1, 0)]
    public class RaptorShockPlugin : TerrariaPlugin
    {
        private readonly RaptorShockCommands _commands = new RaptorShockCommands();
        private readonly ILog _log = LogManager.GetLogger("RaptorShock");

        private KeyboardState _keyboard;
        private KeyboardState _lastKeyboard;

        /// <summary>
        ///     Initializes the <see cref="RaptorShockPlugin" /> class.
        /// </summary>
        public RaptorShockPlugin()
        {
            Instance = this;

            CommandManager.Register(_commands);
            GameHooks.Update += OnGameUpdate;
            PlayerHooks.Hurting += OnPlayerHurting;
            PlayerHooks.Kill += OnPlayerKill;
            PlayerHooks.Update2 += OnPlayerUpdate2;
            PlayerHooks.Updated2 += OnPlayerUpdated2;
        }

        /// <summary>
        ///     Gets the RaptorShock plugin instance.
        /// </summary>
        public static RaptorShockPlugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Author => "MarioE";

        /// <summary>
        ///     Gets the command manager.
        /// </summary>
        [NotNull]
        public CommandManager CommandManager { get; } = new CommandManager();

        /// <inheritdoc />
        public override string Description => "Provides a command API.";

        /// <inheritdoc />
        public override string Name => "RaptorShock";

        /// <inheritdoc />
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        private bool IsAltDown => IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CommandManager.Deregister(_commands);
                GameHooks.Update -= OnGameUpdate;
            }

            base.Dispose(disposing);
        }

        private bool IsKeyDown(Keys key) => _keyboard.IsKeyDown(key);
        private bool IsKeyTapped(Keys key) => _keyboard.IsKeyDown(key) && !_lastKeyboard.IsKeyDown(key);

        private void OnGameUpdate(object sender, HandledEventArgs e)
        {
            if (!Main.hasFocus)
            {
                return;
            }

            _lastKeyboard = _keyboard;
            _keyboard = Keyboard.GetState();
            Main.chatRelease = false;

            if (Main.gameMenu)
            {
                return;
            }

            if (IsKeyTapped(Keys.Enter) && !IsAltDown)
            {
                Main.drawingPlayerChat = !Main.drawingPlayerChat;
                if (Main.drawingPlayerChat)
                {
                    Main.PlaySound(10);
                }
                else
                {
                    var chatText = Main.chatText;
                    if (chatText.StartsWith("."))
                    {
                        try
                        {
                            CommandManager.Run(chatText.Substring(1));
                            _log.Info($"Executed '{chatText}'.");
                        }
                        catch (FormatException ex)
                        {
                            Utils.ShowErrorMessage(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            _log.Error($"An exception occurred while running the command '{chatText}':");
                            _log.Error(ex);
                        }
                    }
                    else if (!string.IsNullOrEmpty(chatText))
                    {
                        if (Main.netMode == 0)
                        {
                            Main.NewText($"<{Utils.LocalPlayer.name}> {chatText}");
                        }
                        else
                        {
                            NetMessage.SendData((int)PacketTypes.SmartTextMessage, -1, -1,
                                NetworkText.FromLiteral(chatText));
                        }
                    }

                    Main.chatText = "";
                    Main.PlaySound(11);
                }
            }
            if (IsKeyTapped(Keys.Escape) && Main.drawingPlayerChat)
            {
                Main.chatText = "";
                Main.PlaySound(11);
            }
        }

        private void OnPlayerHurting(object sender, HurtingEventArgs e)
        {
            if (e.IsLocal && _commands.IsGodMode)
            {
                e.Handled = true;
            }
        }

        private void OnPlayerKill(object sender, KillEventArgs e)
        {
            if (e.IsLocal && _commands.IsGodMode)
            {
                e.Handled = true;
            }
        }

        private void OnPlayerUpdate2(object sender, UpdatedEventArgs e)
        {
            if (e.IsLocal && _commands.IsGodMode)
            {
                var player = e.Player;
                player.breath = player.breathMax;
                player.statLife = player.statLifeMax2;
                player.statMana = player.statManaMax2;
            }
        }

        private void OnPlayerUpdated2(object sender, UpdatedEventArgs e)
        {
            if (e.IsLocal)
            {
                var player = e.Player;
                if (_commands.SpeedValue != null)
                {
                    var speed = (float)_commands.SpeedValue;
                    player.maxRunSpeed = speed;
                    player.moveSpeed = speed;
                    player.runAcceleration = speed / 12.5f;
                }
            }
        }
    }
}
