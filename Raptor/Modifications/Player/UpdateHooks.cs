using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;

namespace Raptor.Modifications.Player
{
    using static Instruction;

    internal sealed class UpdateHooks : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        /// <inheritdoc />
        public override void Apply(AssemblyDefinition assembly)
        {
            var player = assembly.GetType("Player");
            var module = player.Module;
            var update = player.GetMethod("Update");
            update.InjectBeginning(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeUpdate", Flags))),
                Create(OpCodes.Brfalse_S, update.Body.Instructions[0]),
                Create(OpCodes.Ret));
            update.InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeUpdated", Flags))));
            player.GetMethod("ResetEffects").InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeUpdate2", Flags))));
            // TODO: check for a better target for Updated2.
            player.GetMethod("UpdateJumpHeight").InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Call, module.Import(typeof(PlayerHooks).GetMethod("InvokeUpdated2", Flags))));
        }
    }
}
