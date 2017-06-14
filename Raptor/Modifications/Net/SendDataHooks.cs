using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;
using static Mono.Cecil.Cil.Instruction;

namespace Raptor.Modifications.Net
{
    /// <summary>
    ///     Represents a modification that adds SendData hooks.
    /// </summary>
    public sealed class SendDataHooks : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        /// <inheritdoc />
        protected override void ApplyImpl(AssemblyDefinition assembly)
        {
            var netMessage = assembly.GetType("NetMessage");
            var module = netMessage.Module;
            var sendData = netMessage.GetMethod("SendData");
            sendData.InjectBeginning(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_3),
                Create(OpCodes.Ldarg_S, sendData.Parameters[4]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[5]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[6]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[7]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[8]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[9]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[10]),
                Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeSendData", Flags))),
                Create(OpCodes.Brfalse_S, sendData.Body.Instructions[0]),
                Create(OpCodes.Ret));
            sendData.InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_3),
                Create(OpCodes.Ldarg_S, sendData.Parameters[4]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[5]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[6]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[7]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[8]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[9]),
                Create(OpCodes.Ldarg_S, sendData.Parameters[10]),
                Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeSentData", Flags))));
            sendData.ReplaceShortBranches();
        }
    }
}
