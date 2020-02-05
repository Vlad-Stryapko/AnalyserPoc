using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using AnalyserPoc;
using LibWithAttribute;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AnalyserPoc.Test
{

    [AttributeUsage(AttributeTargets.All)]
    public class MySecondAttribute : Attribute { }

    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        [TestMethod]
        public void DiagnosticTest()
        {
            var source = @"
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using System.Text;
                using System.Threading.Tasks;
                using System.Diagnostics;
                using System.Runtime.Serialization;
                using AnalyserPoc.Test;
                using AnalyserPoc;
                using LibWithAttribute;

                namespace ConsoleApplication1 {
                    class TypeName {
                        [MyThird]
                        [MySecond]
                        [My]
                        public void Stuff() {
                        }
                    }
                }";


            var tree = CSharpSyntaxTree.ParseText(source);
            var compilation = CSharpCompilation.Create("ConsoleApplication1")
            .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
            .AddReferences(MetadataReference.CreateFromFile(typeof(MyAttribute).Assembly.Location))
            .AddSyntaxTrees(tree)
            ;

            var type = compilation.GetTypeByMetadataName("LibWithAttribute.MyAttribute"); //Named Type

            var expected = new DiagnosticResult
            {
                Id = "AnalyserPoc",
                Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(source, expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AnalyserPocCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AnalyserPocAnalyzer();
        }
    }
}
