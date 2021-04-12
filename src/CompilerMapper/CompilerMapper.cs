using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CompilerMapper
{
    public class CompilerMapper : ISyntaxReceiver
    {
        public List<string> Interfaces { get; } = new();
        public List<(TypeSyntax returnType, TypeSyntax inputType)> MappersRequired { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax
                {
                    AttributeLists: { Count: >= 1 } attributes,
                } interfaceDeclaration && attributes.First().Attributes.First().Name.GetText().ToString().Contains("MapperInterface"))
            {
                Interfaces.Add(interfaceDeclaration.Identifier.ValueText);

                foreach (var method in interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    MappersRequired.Add((method.ReturnType, method.ParameterList.Parameters.First().Type));
                }
            }
        }
    }
}
