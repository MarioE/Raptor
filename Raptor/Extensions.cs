using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Raptor
{
    using static Instruction;

    /// <summary>
    ///     Provides extension methods.
    /// </summary>
    [CLSCompliant(false)]
    public static class Extensions
    {
        private static readonly Dictionary<OpCode, OpCode> ShortToLong = new Dictionary<OpCode, OpCode>
        {
            {OpCodes.Beq_S, OpCodes.Beq},
            {OpCodes.Bge_S, OpCodes.Bge},
            {OpCodes.Bge_Un_S, OpCodes.Bge_Un},
            {OpCodes.Bgt_S, OpCodes.Bgt},
            {OpCodes.Bgt_Un_S, OpCodes.Bgt_Un},
            {OpCodes.Ble_S, OpCodes.Ble},
            {OpCodes.Ble_Un_S, OpCodes.Ble_Un},
            {OpCodes.Blt_S, OpCodes.Blt},
            {OpCodes.Blt_Un_S, OpCodes.Blt_Un},
            {OpCodes.Bne_Un_S, OpCodes.Bne_Un},
            {OpCodes.Br_S, OpCodes.Br},
            {OpCodes.Brfalse_S, OpCodes.Brfalse},
            {OpCodes.Brtrue_S, OpCodes.Brtrue},
            {OpCodes.Leave_S, OpCodes.Leave}
        };

        /// <summary>
        ///     Blanks out the method, causing it to do nothing or return a default value.
        /// </summary>
        /// <param name="method">The method, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <c>null</c>.</exception>
        public static void BlankOut([NotNull] this MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (method.ReturnType != method.Module.TypeSystem.Void)
            {
                method.InjectBeginning(Create(OpCodes.Initobj, method.ReturnType));
            }
            method.InjectBeginning(Create(OpCodes.Ret));
        }

        /// <summary>
        ///     Gets the method with the specified name and, optionally, parameter type names.
        /// </summary>
        /// <param name="type">The type, which must not be <c>null</c>.</param>
        /// <param name="methodName">The method name, which must not be <c>null</c>.</param>
        /// <param name="parameterTypeNames">The parameter type names.</param>
        /// <returns>The method.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="type" /> or <paramref name="methodName" /> is <c>null</c>.
        /// </exception>
        public static MethodDefinition GetMethod([NotNull] this TypeDefinition type, [NotNull] string methodName,
            [CanBeNull] string[] parameterTypeNames = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            return type.Methods.First(
                m => m.Name == methodName &&
                     (parameterTypeNames == null ||
                      m.Parameters.Select(p => p.ParameterType.Name).SequenceEqual(parameterTypeNames)));
        }

        /// <summary>
        ///     Gets the type with the specified name.
        /// </summary>
        /// <param name="assembly">The assembly, which must not be <c>null</c>.</param>
        /// <param name="typeName">The type name, which must not be <c>null</c>.</param>
        /// <returns>The type.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="assembly" /> or <paramref name="typeName" /> is <c>null</c>.
        /// </exception>
        public static TypeDefinition GetType([NotNull] this AssemblyDefinition assembly, [NotNull] string typeName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return assembly.Modules.SelectMany(m => m.Types).First(t => t.Name == typeName);
        }

        /// <summary>
        ///     Injects the specified instructions to the beginning.
        /// </summary>
        /// <param name="method">The method, which must not be <c>null</c>.</param>
        /// <param name="instructions">The instructions, which must not be <c>null</c> or contain <c>null</c>.</param>
        /// <exception cref="ArgumentException"><paramref name="instructions" /> contains <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="method" /> or <paramref name="instructions" /> is <c>null</c>.
        /// </exception>
        public static void InjectBeginning([NotNull] this MethodDefinition method,
            [ItemNotNull] [NotNull] params Instruction[] instructions)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (instructions == null)
            {
                throw new ArgumentNullException(nameof(instructions));
            }
            if (instructions.Contains(null))
            {
                throw new ArgumentException("Instructions cannot contain null.", nameof(instructions));
            }

            var body = method.Body;
            var processor = body.GetILProcessor();
            var target = body.Instructions[0];
            foreach (var instruction in instructions)
            {
                processor.InsertBefore(target, instruction);
            }
        }

        /// <summary>
        ///     Injects the specified instructions to the endings.
        /// </summary>
        /// <param name="method">The method, which must not be <c>null</c>.</param>
        /// <param name="instructions">The instructions, which must not be <c>null</c> or contain <c>null</c>.</param>
        /// <exception cref="ArgumentException"><paramref name="instructions" /> contains <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">
        ///     Either <paramref name="method" /> or <paramref name="instructions" /> is <c>null</c>.
        /// </exception>
        public static void InjectEndings([NotNull] this MethodDefinition method,
            [ItemNotNull] [NotNull] params Instruction[] instructions)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            if (instructions == null)
            {
                throw new ArgumentNullException(nameof(instructions));
            }
            if (instructions.Contains(null))
            {
                throw new ArgumentException("Instructions cannot contain null.", nameof(instructions));
            }

            var body = method.Body;
            var processor = body.GetILProcessor();
            for (var i = body.Instructions.Count - 1; i >= 0; --i)
            {
                // Replace the rets with nops because there may be branches that go directly to the ret.
                var target = body.Instructions[i];
                if (target.OpCode == OpCodes.Ret)
                {
                    processor.InsertAfter(target, Create(OpCodes.Ret));
                    foreach (var instruction in instructions.Reverse())
                    {
                        processor.InsertAfter(target, instruction);
                    }
                    target.OpCode = OpCodes.Nop;
                }
            }
        }

        /// <summary>
        ///     Replaces the short branches with long branches.
        /// </summary>
        /// <param name="method">The method, which must not be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="method" /> is <c>null</c>.</exception>
        public static void ReplaceShortBranches([NotNull] this MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            foreach (var instruction in method.Body.Instructions.Where(
                i => i.OpCode.OperandType == OperandType.ShortInlineBrTarget))
            {
                instruction.OpCode = ShortToLong[instruction.OpCode];
            }
        }
    }
}
