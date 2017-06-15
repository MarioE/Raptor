using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
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
        private static readonly string ConfigPath = Path.Combine("config", "raptorshock.json");

        private readonly RaptorShockCommands _commands = new RaptorShockCommands();
        private readonly Config _config = new Config();
        private readonly ILog _log = LogManager.GetLogger("RaptorShock");

        private KeyboardState _keyboard;
        private KeyboardState _lastKeyboard;

        /// <summary>
        ///     Initializes the <see cref="RaptorShockPlugin" /> class.
        /// </summary>
        public RaptorShockPlugin()
        {
            Instance = this;

            Directory.CreateDirectory("config");
            if (File.Exists(ConfigPath))
            {
                _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
            }

            CommandManager.Register(_commands);
            GameHooks.Initialized += OnGameInitialized;
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
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(_config, Formatting.Indented));

                CommandManager.Deregister(_commands);
                GameHooks.Initialized -= OnGameInitialized;
                GameHooks.Update -= OnGameUpdate;
                PlayerHooks.Hurting -= OnPlayerHurting;
                PlayerHooks.Kill -= OnPlayerKill;
                PlayerHooks.Update2 -= OnPlayerUpdate2;
                PlayerHooks.Updated2 -= OnPlayerUpdated2;
            }

            base.Dispose(disposing);
        }

        private bool IsKeyDown(Keys key) => _keyboard.IsKeyDown(key);
        private bool IsKeyTapped(Keys key) => _keyboard.IsKeyDown(key) && !_lastKeyboard.IsKeyDown(key);

        private void OnGameInitialized(object sender, EventArgs e)
        {
            Main.showSplash = _config.ShowSplashScreen;
        }

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
                    if (chatText.StartsWith(".") && !chatText.StartsWith(".."))
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
                        if (chatText.StartsWith("."))
                        {
                            chatText = chatText.Substring(1);
                        }

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
            if (e.IsLocal)
            {
                var player = e.Player;
                if (_commands.IsGodMode)
                {
                    player.breath = player.breathMax - 1;
                    player.statLife = player.statLifeMax2;
                    player.statMana = player.statManaMax2;
                }
                if (_commands.IsInfiniteMana)
                {
                    player.statMana = player.statManaMax2;
                }
                if (_commands.IsInfiniteWings)
                {
                    player.wingTime = 2;
                }
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
