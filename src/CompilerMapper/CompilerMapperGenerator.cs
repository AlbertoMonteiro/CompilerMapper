using System;
using Microsoft.CodeAnalysis;

namespace CompilerMapper
{
    [Generator]
    public class CompilerMapperGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new CompilerMapper());
    }

    public class CompilerMapper : ISyntaxReceiver
    {
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            throw new NotImplementedException();
        }
    }
}
