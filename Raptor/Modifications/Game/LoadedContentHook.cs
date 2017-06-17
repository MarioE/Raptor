using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;
using static Mono.Cecil.Cil.Instruction;

namespace Raptor.Modifications.Game
{
    internal sealed class LoadedContentHook : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        public override void Apply(AssemblyDefinition assembly)
        {
            var main = assembly.GetType("Main");
            var module = main.Module;
            var loadContent = main.GetMethod("LoadContent");
            loadContent.InjectEndings(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Callvirt, module.Import(typeof(Microsoft.Xna.Framework.Game).GetMethod("get_Content"))),
                Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeLoadedContent", Flags))));
            loadContent.ReplaceShortBranches();
        }
    }
}
