using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.DotNetTest.Helpers;
using TestUtility;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.DotNetTest.Tests
{
    public class TestDiscoveryFacts : AbstractTestFixture
    {
        private readonly TestAssets _testAssets;

        public TestDiscoveryFacts(ITestOutputHelper output)
            : base(output)
        {
            this._testAssets = TestAssets.Instance;
        }

        [Theory]
        [InlineData("BasicTestProjectSample01", "TestProgram.cs", 8, 23, true, "XunitTestMethod")]
        [InlineData("BasicTestProjectSample01", "TestProgram.cs", 15, 26, true, "XunitTestMethod")]
        [InlineData("BasicTestProjectSample01", "TestProgram.cs", 21, 28, false, "")]
        [InlineData("BasicTestProjectSample02", "TestProgram.cs", 8, 23, true, "NUnitTestMethod")]
        [InlineData("BasicTestProjectSample02", "TestProgram.cs", 15, 26, true, "NUnitTestMethod")]
        [InlineData("BasicTestProjectSample02", "TestProgram.cs", 21, 26, true, "NUnitTestMethod")]
        [InlineData("BasicTestProjectSample02", "TestProgram.cs", 25, 35, false, "")]
        public async Task FoundFactsBasedTest(string projectName, string fileName, int line, int column, bool found, string expectedFeatureName)
        {
            using (var testProject = await this._testAssets.GetTestProjectAsync(projectName))
            using (var omnisharp = CreateOmniSharpHost(testProject.Directory))
            {
                var filePath = Path.Combine(testProject.Directory, fileName);
                var solution = omnisharp.Workspace.CurrentSolution;
                var documentId = solution.GetDocumentIdsWithFilePath(filePath).First();
                var document = solution.GetDocument(documentId);

                var semanticModel = await document.GetSemanticModelAsync();

                var text = semanticModel.SyntaxTree.GetText();
                var position = text.Lines.GetPosition(new LinePosition(line, column));

                var root = semanticModel.SyntaxTree.GetRoot();
                var methodDeclaration = root.DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .Single(node => node.Span.Contains(position));

                var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);

                var discover = new TestFeaturesDiscover();
                var features = discover.Discover(methodDeclaration, semanticModel);

                if (found)
                {
                    var feature = features.Single();
                    Assert.Equal(expectedFeatureName, feature.Name);

                    var symbolName = methodSymbol.ToDisplayString();
                    symbolName = symbolName.Substring(0, symbolName.IndexOf('('));
                    Assert.Equal(symbolName, feature.Data);
                }
                else
                {
                    Assert.Empty(features);
                }
            }
        }
    }
}
