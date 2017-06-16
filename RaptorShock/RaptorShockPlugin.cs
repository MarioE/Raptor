using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Raptor.Api;
using Raptor.Hooks;
using Raptor.Hooks.Events.Game;
using Raptor.Hooks.Events.Player;
using RaptorShock.Commands;
using Terraria;
using Terraria.Chat;

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

            CommandManager.AddParser(typeof(byte), s => byte.TryParse(s, out var result) ? (object)result : null);
            CommandManager.AddParser(typeof(float), s => float.TryParse(s, out var result) ? (object)result : null);
            CommandManager.AddParser(typeof(int), s => int.TryParse(s, out var result) ? (object)result : null);
            CommandManager.AddParser(typeof(Item), s =>
            {
                var items = Utils.GetItemsByNameOrId(s);
                return items.Count == 1 ? items[0] : null;
            });
            CommandManager.AddParser(typeof(Projectile), s =>
            {
                var projectiles = Utils.GetProjectilesByNameOrId(s);
                return projectiles.Count == 1 ? projectiles[0] : null;
            });
            CommandManager.AddParser(typeof(string), s => s);
            CommandManager.Register(_commands);

            GameHooks.Initialized += OnGameInitialized;
            GameHooks.Lighting += OnGameLighting;
            GameHooks.Update += OnGameUpdate;
            PlayerHooks.Hurting += OnPlayerHurting;
            PlayerHooks.Kill += OnPlayerKill;
            PlayerHooks.Updated2 += OnPlayerUpdated2;
            PlayerHooks.Updated += OnPlayerUpdated;
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
                
                GameHooks.Initialized -= OnGameInitialized;
                GameHooks.Lighting -= OnGameLighting;
                GameHooks.Update -= OnGameUpdate;
                PlayerHooks.Hurting -= OnPlayerHurting;
                PlayerHooks.Kill -= OnPlayerKill;
                PlayerHooks.Updated2 -= OnPlayerUpdated2;
                PlayerHooks.Updated -= OnPlayerUpdated;
            }

            base.Dispose(disposing);
        }

        private bool IsKeyDown(Keys key) => _keyboard.IsKeyDown(key);
        private bool IsKeyTapped(Keys key) => _keyboard.IsKeyDown(key) && !_lastKeyboard.IsKeyDown(key);

        private void OnGameInitialized(object sender, EventArgs e)
        {
            Main.showSplash = _config.ShowSplashScreen;
            Utils.InitializeNames();
        }

        private void OnGameLighting(object sender, LightingEventArgs e)
        {
            if (_commands.IsFullBright)
            {
                e.SwipeData.function = lsd =>
                {
                    foreach (var state in lsd.jaggedArray.SelectMany(s => s))
                    {
                        state.r = state.r2 = state.g = state.g2 = state.b = state.b2 = 1;
                    }
                };
            }
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

            // Don't misinterpret key presses in menus.
            if (Main.gameMenu || Main.editChest || Main.editSign)
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
                            Utils.ShowErrorMessage("An exception occurred. Check the log for more details.");
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
                            NetMessage.SendChatMessageFromClient(new ChatMessage(chatText));
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

        private void OnPlayerUpdated(object sender, UpdatedEventArgs e)
        {
            if (e.IsLocal)
            {
                var player = e.Player;
                if (_commands.IsNoclip)
                {
                    if (player.controlLeft)
                    {
                        _commands.NoclipPosition += new Vector2(-player.moveSpeed, 0);
                    }
                    if (player.controlRight)
                    {
                        _commands.NoclipPosition += new Vector2(player.moveSpeed, 0);
                    }
                    if (player.controlUp)
                    {
                        _commands.NoclipPosition += new Vector2(0, -player.moveSpeed);
                    }
                    if (player.controlDown)
                    {
                        _commands.NoclipPosition += new Vector2(0, player.moveSpeed);
                    }
                    player.gfxOffY = 0;
                    player.position = _commands.NoclipPosition;
                }
            }
        }

        private void OnPlayerUpdated2(object sender, UpdatedEventArgs e)
        {
            if (e.IsLocal)
            {
                var player = e.Player;
                if (_commands.DefenseValue != null)
                {
                    player.statDefense = _commands.DefenseValue.Value;
                }
                if (_commands.IsInfiniteAmmo)
                {
                    foreach (var item in player.inventory)
                    {
                        if (item.ammo != 0 && !item.notAmmo)
                        {
                            item.stack = item.maxStack;
                        }
                    }
                }
                if (_commands.IsInfiniteBreath || _commands.IsGodMode)
                {
                    player.breath = player.breathMax - 1;
                }
                if (_commands.IsInfiniteHealth || _commands.IsGodMode)
                {
                    player.statLife = player.statLifeMax2;
                }
                if (_commands.IsInfiniteMana || _commands.IsGodMode)
                {
                    player.statMana = player.statManaMax2;
                }
                if (_commands.IsInfiniteWings)
                {
                    player.wingTime = 2;
                }
                if (_commands.RangeValue != null)
                {
                    var range = _commands.RangeValue.Value;
                    Player.tileRangeX = range;
                    Player.tileRangeY = range;
                }
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
