using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;
using static Mono.Cecil.Cil.Instruction;

namespace Raptor.Modifications.Game
{
    internal sealed class UpdateHooks : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        public override void Apply(AssemblyDefinition assembly)
        {
            var main = assembly.GetType("Main");
            var module = main.Module;
            var update = main.GetMethod("Update");
            update.InjectBeginning(
                Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeUpdate", Flags))),
                Create(OpCodes.Brfalse_S, update.Body.Instructions[0]),
                Create(OpCodes.Ret));
            update.InjectEndings(
                Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeUpdated", Flags))));
            update.ReplaceShortBranches();
        }
    }
}
