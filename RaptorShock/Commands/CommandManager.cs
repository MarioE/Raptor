using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

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
        private readonly Dictionary<Type, Parser> _parsers = new Dictionary<Type, Parser>();

        /// <summary>
        ///     Gets a read-only view of the commands.
        /// </summary>
        public IEnumerable<Command> Commands => _commands.AsReadOnly();

        private static string GetNextArgument(string s, out int nextIndex)
        {
            var inQuotes = s[0] == '"';
            var i = inQuotes ? 1 : 0;
            var result = new StringBuilder();
            for (; i < s.Length; ++i)
            {
                var c = s[i];
                if (c == '\\' && ++i < s.Length)
                {
                    var nextC = s[i];
                    if (nextC != '"' && nextC != ' ' && nextC != '\\')
                    {
                        result.Append(c);
                    }
                    result.Append(nextC);
                }
                else if (c == '"' && inQuotes || char.IsWhiteSpace(c) && !inQuotes)
                {
                    ++i;
                    break;
                }
                else
                {
                    result.Append(c);
                }
            }

            nextIndex = i;
            return result.ToString();
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

            var commandAttributes = from m in obj.GetType().GetMethods()
                                    let a = m.GetCustomAttribute<CommandAttribute>()
                                    where a != null
                                    select a;
            foreach (var commandAttribute in commandAttributes)
            {
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
                    if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        parameterType = parameterType.GenericTypeArguments[0];
                    }
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
                        var result = parser(argument);
                        if (result == null)
                        {
                            var parameterName = PrettifyCamelCase(parameter.Name);
                            throw new FormatException($"Invalid {parameterName} '{argument}'.");
                        }

                        p.Add(result);
                        return s.Substring(nextIndex);
                    });
                }

                _commands.Add(new Command(commandAttribute, s =>
                {
                    var parameters = new List<object>();
                    s = reducers.Aggregate(s, (s2, reducer) => reducer(s2, parameters));
                    // Ensure that all arguments were consumed.
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        throw new FormatException($"Syntax: {syntax}");
                    }

                    method.Invoke(obj, parameters.ToArray());
                }));
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

            var commandName = s.Split(' ')[0];
            var command = _commands.FirstOrDefault(
                c => c.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase) ||
                     (c.Alias?.Equals(commandName, StringComparison.OrdinalIgnoreCase) ?? false));
            if (command == null)
            {
                throw new FormatException("Invalid command.");
            }

            command.Invoke(s.Substring(commandName.Length));
        }

        private delegate string CommandReducer(string s, List<object> parameters);
    }
}
