using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Raptor.Hooks;

namespace Raptor.Modifications.Game
{
    using static Instruction;

    internal sealed class LightingHook : Modification
    {
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Static;

        private static void InjectHooks(MethodDefinition method)
        {
            var module = method.Module;
            var body = method.Body;
            var instructions = body.Instructions;
            for (var i = instructions.Count - 1; i >= 0; --i)
            {
                // Check for an invocation of some action involving LightingSwipeData.
                var instruction = instructions[i];
                if (instruction.OpCode != OpCodes.Callvirt)
                {
                    continue;
                }
                var method2 = (MethodReference)instruction.Operand;
                var type = method2.DeclaringType;
                if (method2.Name != "Invoke" || !type.IsGenericInstance ||
                    ((GenericInstanceType)type).GenericArguments[0].Name != "LightingSwipeData")
                {
                    continue;
                }

                // The function field of LightingSwipeData has already been loaded. Thus, we need to move back two
                // instructions if we want our hooks to be able to modify function.
                var target = instruction.Previous.Previous;
                method.InjectBefore(target,
                    Create(OpCodes.Dup),
                    Create(OpCodes.Call, module.Import(typeof(GameHooks).GetMethod("InvokeLighting", Flags))),
                    Create(OpCodes.Brfalse_S, target),
                    Create(OpCodes.Pop),
                    Create(OpCodes.Br_S, instruction.Next));
                i -= 5;
            }
        }

        public override void Apply(AssemblyDefinition assembly)
        {
            var lighting = assembly.GetType("Lighting");

            // Single-core lighting
            var doColors = lighting.GetMethod("doColors");
            InjectHooks(lighting.GetMethod("doColors"));
            doColors.ReplaceShortBranches();

            // Multi-core lighting
            var callbackLightingSwipe = lighting.GetMethod("callback_LightingSwipe");
            InjectHooks(callbackLightingSwipe);
            callbackLightingSwipe.ReplaceShortBranches();
        }
    }
}
