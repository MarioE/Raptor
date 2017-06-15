using System.Linq;
using Mono.Cecil;

namespace Raptor.Modifications
{
    internal sealed class MakeTypesPublic : Modification
    {
        private static void MakePublic(TypeDefinition type)
        {
            if (type.IsNested)
            {
                type.IsNestedPublic = true;
            }
            else
            {
                type.IsPublic = true;
            }

            foreach (var field in type.Fields)
            {
                // Ignore backing fields for events.
                if (field.IsPrivate && type.Events.Any(x => x.Name == field.Name))
                {
                    continue;
                }

                field.IsPublic = true;
            }
            foreach (var method in type.Methods)
            {
                method.IsPublic = true;
            }
            foreach (var property in type.Properties)
            {
                if (property.GetMethod != null)
                {
                    property.GetMethod.IsPublic = true;
                }
                if (property.SetMethod != null)
                {
                    property.SetMethod.IsPublic = true;
                }
            }
            foreach (var nestedType in type.NestedTypes)
            {
                MakePublic(nestedType);
            }
        }
        
        public override void Apply(AssemblyDefinition assembly)
        {
            foreach (var type in assembly.Modules.SelectMany(m => m.Types))
            {
                MakePublic(type);
            }
        }
    }
}
