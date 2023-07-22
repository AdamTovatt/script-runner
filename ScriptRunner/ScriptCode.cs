using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ScriptRunner.Providers;
using System.Reflection;

namespace ScriptRunner
{
    public class ScriptCode
    {
        public string Code { get; set; }

        public ScriptCode(string code)
        {
            Code = code;
        }

        public ScriptCompileResult Compile()
        {
            ScriptCompileResult result = new ScriptCompileResult();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(Code);

            CSharpCompilation compilation = CSharpCompilation.Create(
                "runtimeCompiledAssembly",
                new[] { syntaxTree },
                ReferenceProvider.Instance.GetDefault(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithUsings(NamespaceProvider.Instance.GetDefault()));

            using (MemoryStream dllStream = new MemoryStream())
            using (MemoryStream pdbStream = new MemoryStream())
            {
                EmitResult emitResult = compilation.Emit(dllStream, pdbStream);

                foreach (Diagnostic diagnostic in emitResult.Diagnostics) // add errors and warnings
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error)
                    {
                        if (result.Errors == null)
                            result.Errors = new List<string>();

                        result.Errors.Add(diagnostic.GetMessage());
                    }
                    else
                    {
                        if(result.Warnings == null)
                            result.Warnings = new List<string>();

                        result.Warnings.Add(diagnostic.GetMessage());
                    }
                }

                if (emitResult.Success)
                {
                    dllStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = Assembly.Load(dllStream.ToArray(), pdbStream.ToArray());
                    
                    foreach(Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(CompiledScript)))
                        {
                            CompiledScript? compiledScript = (CompiledScript?)Activator.CreateInstance(type);

                            if (compiledScript != null)
                            {
                                result.CompiledScript = compiledScript;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}