using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace CompilerMapper
{
    [Generator]
    public class CompilerMapperGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var defaultNamespace = context.Compilation.AssemblyName;
            var mapperInterface = $@"using System;

namespace {defaultNamespace}
{{
    [AttributeUsage(AttributeTargets.Interface)]
    public class MapperInterfaceAttribute : Attribute
    {{

    }}
}}";
            context.AddSource("MapperInterface", SourceText.From(mapperInterface, Encoding.UTF8));

            if (context.SyntaxReceiver is CompilerMapper compilerMapper)
            {
                var interfacesToInherit = string.Join(", ", compilerMapper.Interfaces);
                var mapperClass = $@"using System;

namespace {defaultNamespace}
{{
    public class Mapper : {interfacesToInherit}
    {{
        __replaceableBody__
    }}
}}";
                var sb = new StringBuilder();
                foreach (var mapperRequired in compilerMapper.MappersRequired)
                {
                    BuildMapperMethod(sb, mapperRequired, context.Compilation);
                }

                mapperClass = mapperClass.Replace("__replaceableBody__", sb.ToString());

                context.AddSource("MapperGenerated", SourceText.From(mapperClass, Encoding.UTF8));
            }
        }

        private static void BuildMapperMethod(StringBuilder sb, (TypeSyntax, TypeSyntax) mapperRequired, Compilation compilation)
        {
            var (returnType, inputType) = mapperRequired;
            var returnModel = compilation.GetSemanticModel(returnType.SyntaxTree);
            var inputModel = compilation.GetSemanticModel(inputType.SyntaxTree);
            var returnModelSymbol = returnModel.GetTypeInfo(returnType).Type;
            var inputModelSymbol = inputModel.GetTypeInfo(inputType).Type;
            var returnTypeValue = returnModelSymbol.Name.Trim();
            var sourceProperties = inputModelSymbol.GetMembers().OfType<IPropertySymbol>();
            var destinationProperties = returnModelSymbol.GetMembers().OfType<IPropertySymbol>();

            var propertiesToAssign = destinationProperties
                .Join(sourceProperties, x => (x.Name, x.Type), x => (x.Name, x.Type), (s, d) => d)
                .Select(x => $"{x.Name} = input.{x.Name}");

            var props = string.Join(", ", propertiesToAssign);
            sb.Append(@$"public {returnTypeValue} Map({inputType.GetText().ToString().Trim()} input)
        {{
            return new {returnTypeValue}
            {{
                {props}
            }};
        }}");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}
            context.RegisterForSyntaxNotifications(() => new CompilerMapper());
        }
    }
}
