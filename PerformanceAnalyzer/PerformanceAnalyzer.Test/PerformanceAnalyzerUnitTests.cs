using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = PerformanceAnalyzer.Test.CSharpCodeFixVerifier<
    PerformanceAnalyzer.PerformanceAnalyzerAnalyzer,
    PerformanceAnalyzer.PerformanceAnalyzerCodeFixProvider>;

namespace PerformanceAnalyzer.Test
{
    [TestClass]
    public class PerformanceAnalyzerUnitTests
    {
        // Test for Class Naming Rule
        [TestMethod]
        public async Task Test_ClassNamingRule()
        {
            var test = @"
namespace TestNamespace
{
    class {|#0:testClass|} // Class name should be capitalized
    {
    }
}";

            var fixedCode = @"
namespace TestNamespace
{
    class TESTCLASS
    {
    }
}";

            var expected = new DiagnosticResult(PerformanceAnalyzerAnalyzer.ClassNamingRule)
                .WithLocation(0)
                .WithArguments("testClass");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedCode);
        }

        // Test for Uncached Count Rule
        [TestMethod]
        public async Task Test_UncachedCountRule()
        {
            var test = @"
using System.Collections.Generic;

namespace TestNamespace
{
    class TestClass
    {
        public void TestMethod()
        {
            var list = new List<int>();
            for (int i = 0; i < list.Count; i++) // Count is not cached
            {
            }
        }
    }
}";

            var fixedCode = @"
using System.Collections.Generic;

namespace TestNamespace
{
    class TestClass
    {
        public void TestMethod()
        {
            var list = new List<int>();
            var cachedCount = list.Count; // Cached Count
            for (int i = 0; i < cachedCount; i++)
            {
            }
        }
    }
}";

            var expected = new DiagnosticResult(PerformanceAnalyzerAnalyzer.UncachedCountRule)
                .WithLocation(0);

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedCode);
        }

        // Test for Excessive Parameters Rule
        [TestMethod]
        public async Task Test_ExcessiveParametersRule()
        {
            var test = @"
namespace TestNamespace
{
    class TestClass
    {
        public void TestMethod(int a, int b, int c, int d, int e, int f) // Too many parameters
        {
        }
    }
}";

            var fixedCode = @"
namespace TestNamespace
{
    class TestClassParameters
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
        public int F { get; set; }
    }

    class TestClass
    {
        public void TestMethod(TestClassParameters parameters)
        {
        }
    }
}";

            var expected = new DiagnosticResult(PerformanceAnalyzerAnalyzer.ExcessiveParametersRule)
                .WithLocation(0);

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedCode);
        }

        // Test for Missing XML Documentation Rule
        [TestMethod]
        public async Task Test_MissingXmlDocumentationRule()
        {
            var test = @"
namespace TestNamespace
{
    class TestClass
    {
        public void TestMethod() // No XML documentation
        {
        }
    }
}";

            var fixedCode = @"
namespace TestNamespace
{
    class TestClass
    {
        /// <summary>
        /// Describe the purpose of this method.
        /// </summary>
        public void TestMethod()
        {
        }
    }
}";

            var expected = new DiagnosticResult(PerformanceAnalyzerAnalyzer.MissingXmlDocRule)
                .WithLocation(0)
                .WithArguments("TestMethod");

            await VerifyCS.VerifyCodeFixAsync(test, expected, fixedCode);
        }
    }
}
