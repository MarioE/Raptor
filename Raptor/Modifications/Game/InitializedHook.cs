using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;
using static Mono.Cecil.Cil.Instruction;

namespace Raptor.Modifications.Game
{
    internal sealed class InitializedHook : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;
        
        public override void Apply(AssemblyDefinition assembly)
        {
            var main = assembly.GetType("Main");
            var module = main.Module;
            var initialize = main.GetMethod("Initialize");
            initialize.InjectEndings(
                Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeInitialized", Flags))));
            initialize.ReplaceShortBranches();
        }
    }
}
