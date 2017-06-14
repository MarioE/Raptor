using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;

namespace Raptor.Modifications.Player
{
    using static Instruction;

    /// <summary>
    ///     Represents a modification that adds Kill hooks.
    /// </summary>
    public sealed class KillHooks : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        /// <inheritdoc />
        protected override void ApplyImpl(AssemblyDefinition assembly)
        {
            var player = assembly.GetType("Player");
            var module = player.Module;
            var hurt = player.GetMethod("KillMe");
            hurt.InjectBeginning(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_1),
                Create(OpCodes.Ldarg_2),
                Create(OpCodes.Ldarg_S, hurt.Parameters[3]),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeKill", Flags))),
                Create(OpCodes.Brfalse_S, hurt.Body.Instructions[0]),
                Create(OpCodes.Ret));
            hurt.InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_1),
                Create(OpCodes.Ldarg_2),
                Create(OpCodes.Ldarg_S, hurt.Parameters[3]),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeKilled", Flags))));
            hurt.ReplaceShortBranches();
        }
    }
}
