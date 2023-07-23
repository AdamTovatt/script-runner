using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using ScriptRunner.Providers;
using System.Reflection;

namespace ScriptRunner.Models
{
    /// <summary>
    /// Class that is used to contain script code so that it can be compiled
    /// </summary>
    public class ScriptCode
    {
        /// <summary>
        /// The code of the script
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Constructor taking the code as a string
        /// </summary>
        /// <param name="code">The code</param>
        public ScriptCode(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Will compile the code into a ScriptCompileResult which contains information about errors or warnings, or a CompiledAssembly if it was successfull
        /// </summary>
        /// <returns>A ScriptCompileResult</returns>
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
                        if (result.Warnings == null)
                            result.Warnings = new List<string>();

                        result.Warnings.Add(diagnostic.GetMessage());
                    }
                }

                if (emitResult.Success)
                {
                    dllStream.Seek(0, SeekOrigin.Begin);
                    pdbStream.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = Assembly.Load(dllStream.ToArray(), pdbStream.ToArray());

                    result.CompiledAssembly = assembly;
                    result.XmlComments = GetXmlComments(syntaxTree, compilation);
                }
            }

            return result;
        }

        private Dictionary<string, XmlComment> GetXmlComments(SyntaxTree syntaxTree, CSharpCompilation compilation)
        {
            Dictionary<string, XmlComment> xmlComments = new Dictionary<string, XmlComment>();

            CompilationUnitSyntax root = (CompilationUnitSyntax)syntaxTree.GetRoot();
            Stack<SyntaxNode> membersToCheck = new Stack<SyntaxNode>(root.Members);

            while(membersToCheck.Count() > 0)
            { 
                SyntaxNode member = membersToCheck.Pop();
                
                if (member is MethodDeclarationSyntax methodDeclaration)
                {
                    SyntaxTrivia trivia = methodDeclaration.GetLeadingTrivia().SingleOrDefault(t => IsCommentTrivia(t));
                    if (trivia != default(SyntaxTrivia))
                    {
                        DocumentationCommentTriviaSyntax? xmlTrivia = (DocumentationCommentTriviaSyntax?)trivia.GetStructure();
                        if (xmlTrivia != null)
                        {
                            string commentText = xmlTrivia.ToFullString().Trim();
                            string methodName = methodDeclaration.Identifier.Text + methodDeclaration.ParameterList.ToString();
                            xmlComments[methodName] = new XmlComment(commentText);
                        }
                    }
                }

                List<SyntaxNode>? membersInMember = GetMembersList(member);

                if (membersInMember != null)
                {
                    foreach (SyntaxNode memberInMember in membersInMember)
                        membersToCheck.Push(memberInMember);
                }
            }

            return xmlComments;
        }

        private List<SyntaxNode>? GetMembersList(SyntaxNode node)
        {
            Type nodeType = node.GetType();

            PropertyInfo? membersProperty = nodeType.GetProperty("Members");

            if (membersProperty != null && typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(membersProperty.PropertyType))
            {
                IEnumerable<SyntaxNode>? membersList = (IEnumerable<SyntaxNode>?)membersProperty.GetValue(node);

                if (membersList != null)
                    return new List<SyntaxNode>(membersList);
            }

            return null;
        }

        private bool IsCommentTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia);
        }
    }
}