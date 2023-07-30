using ScriptRunner;
using ScriptRunner.Providers;
using System;

namespace CustomScripts
{
    public class AddReferenceScript : CompiledScript
    {
        public AddReferenceScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This function will add a reference to a type. This is usefull if a type can't be found when compiling a script. Remember to call the function with the full type name.
        /// </summary>
        /// <param name="typeName">The full type name of the type to add a reference to</param>
        [ScriptStart]
        public string AddReferenceForUser(string typeName)
        {
            try
            {
                // Add the reference using ReferenceProvider.Instance.AddReference
                ReferenceProvider.Instance.AddReference(typeName);

                // Persist the reference using ReferenceProvider.Instance.PersistReference
                ReferenceProvider.Instance.PersistReference(typeName);

                return "Reference successfully added";
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the process
                return $"Error adding reference: {ex.Message}";
            }
        }
    }
}
