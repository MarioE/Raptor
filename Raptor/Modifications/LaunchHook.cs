using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Raptor.Modifications
{
    using static Instruction;

    internal class LaunchHook : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        public override void Apply(AssemblyDefinition assembly)
        {
            var launchInitializer = assembly.GetType("LaunchInitializer");
            var module = launchInitializer.Module;
            var loadParameters = launchInitializer.GetMethod("LoadParameters");
            loadParameters.InjectBeginning(
                Create(OpCodes.Ldarg_0),
                Create(OpCodes.Call, module.Import(typeof(Program).GetMethod("OnLaunch", Flags))));
        }
    }
}
