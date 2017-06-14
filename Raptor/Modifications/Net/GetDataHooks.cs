using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;

namespace Raptor.Modifications.Net
{
    using static Instruction;

    /// <summary>
    ///     Represents a modification that adds SendData hooks.
    /// </summary>
    public sealed class GetDataHooks : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        protected override void ApplyImpl(AssemblyDefinition assembly)
        {
            var messageBuffer = assembly.GetType("MessageBuffer");
            var module = messageBuffer.Module;
            var getData = messageBuffer.GetMethod("GetData");
            getData.InjectBeginning(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_1),
                Create(OpCodes.Ldarg_2),
                Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeGetData", Flags))),
                Create(OpCodes.Brfalse_S, getData.Body.Instructions[0]),
                Create(OpCodes.Ret));
            getData.InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Ldarg_1),
                Create(OpCodes.Ldarg_2),
                Create(OpCodes.Call, module.Import(typeof(NetHooks).GetMethod("InvokeGotData", Flags))));
            getData.ReplaceShortBranches();
        }
    }
}
