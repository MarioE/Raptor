using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using RaptorShock.Commands.Parsers;
using Terraria;

namespace RaptorShock.Commands
{
    /// <summary>
    ///     Manages commands.
    /// </summary>
    [PublicAPI]
    public sealed class CommandManager
    {
        private static readonly Regex CamelCaseRegex = new Regex("(?<=[a-z])([A-Z])");

        private readonly List<Command> _commands = new List<Command>();

        private readonly Dictionary<Type, Parser> _parsers = new Dictionary<Type, Parser>
        {
            [typeof(byte)] = new ByteParser(),
            [typeof(float)] = new FloatParser(),
            [typeof(int)] = new IntParser(),
            [typeof(Item)] = new ItemParser(),
            [typeof(Projectile)] = new ProjectileParser(),
            [typeof(string)] = new StringParser()
        };

        /// <summary>
        ///     Gets the commands.
        /// </summary>
        public IEnumerable<Command> Commands => _commands.AsReadOnly();

        private static string GetNextArgument(string s, out int nextIndex)
        {
            var i = 1;
            if (s.StartsWith("\""))
            {
                while (i < s.Length && (s[i] != '"' || s[i] == '"' && s[i - 1] == '\\'))
                {
                    ++i;
                }
                if (i == 1 || s[i] != '"')
                {
                    throw new FormatException("Unclosed quotes.");
                }

                nextIndex = i;
                return s.Substring(1, i - 2);
            }

            while (i < s.Length && !char.IsWhiteSpace(s[i]))
            {
                ++i;
            }

            nextIndex = i;
            return s.Substring(0, i);
        }

        private static string PrettifyCamelCase(string camelCase) =>
            CamelCaseRegex.Replace(camelCase, " $1").ToLower().Trim();

        /// <summary>
        ///     Adds the specified parser.
        /// </summary>
        /// <param name="type">The type, which must not be <c>null</c>.</param>
        /// <param name="parser">The parser, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="type" /> or <paramref name="parser" /> is <c>null</c>.
        /// </exception>
        public void AddParser([NotNull] Type type, [NotNull] Parser parser)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _parsers[type] = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        /// <summary>
        ///     Deregisters the commands marked in the specified object.
        /// </summary>
        /// <param name="obj">The object, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is <c>null</c>.</exception>
        public void Deregister([NotNull] object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var method in obj.GetType().GetMethods())
            {
                var commandAttribute = (CommandAttribute)method.GetCustomAttribute(typeof(CommandAttribute));
                if (commandAttribute == null)
                {
                    continue;
                }

                _commands.RemoveAll(c => c.Name == commandAttribute.Name);
            }
        }

        /// <summary>
        ///     Registers the commands marked in the specified object.
        /// </summary>
        /// <param name="obj">The object, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">A command contains a type which cannot be parsed.</exception>
        public void Register([NotNull] object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var method in obj.GetType().GetMethods())
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                {
                    continue;
                }

                var syntax = commandAttribute.Syntax;
                var reducers = new List<CommandReducer>();
                foreach (var parameter in method.GetParameters())
                {
                    var parameterType = parameter.ParameterType;
                    if (!_parsers.TryGetValue(parameterType, out var parser))
                    {
                        throw new InvalidOperationException($"Type '{parameterType.Name}' cannot be parsed.");
                    }

                    reducers.Add((s, p) =>
                    {
                        s = s.Trim();
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            if (parameter.HasDefaultValue)
                            {
                                p.Add(parameter.DefaultValue);
                                return s;
                            }
                            throw new FormatException($"Syntax: {syntax}");
                        }

                        var argument = GetNextArgument(s, out var nextIndex);
                        var result = parser.Parse(argument);
                        if (result == null)
                        {
                            var parameterName = PrettifyCamelCase(parameter.Name);
                            throw new FormatException($"Invalid {parameterName} '{argument}'.");
                        }

                        p.Add(result);
                        return s.Substring(nextIndex);
                    });
                }

                void Action(string s)
                {
                    var parameters = new List<object>();
                    s = reducers.Aggregate(s, (s2, reducer) => reducer(s2, parameters));
                    // Ensure that all arguments were consumed.
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        throw new FormatException($"Syntax: {syntax}");
                    }

                    method.Invoke(obj, parameters.ToArray());
                }

                _commands.Add(new Command(commandAttribute, Action));
            }
        }

        /// <summary>
        ///     Runs the specified string as a command.
        /// </summary>
        /// <param name="s">The string, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="s" /> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s" /> failed to be parsed properly.</exception>
        public void Run([NotNull] string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            Command command = null;
            var longestCommandName = "";
            foreach (var command2 in _commands)
            {
                var commandNames = new List<string> {command2.Name};
                if (command2.Alias != null)
                {
                    commandNames.Add(command2.Alias);
                }

                foreach (var commandName in commandNames)
                {
                    if (s.StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (commandName.Length > longestCommandName.Length)
                        {
                            command = command2;
                            longestCommandName = commandName;
                        }
                    }
                }
            }
            if (command == null)
            {
                throw new FormatException("Invalid command.");
            }

            command.Invoke(s.Substring(longestCommandName.Length));
        }

        private delegate string CommandReducer(string s, List<object> parameters);
    }
}
