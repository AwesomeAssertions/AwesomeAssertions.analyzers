using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AwesomeAssertions.Analyzers.TestUtils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AwesomeAssertions.Analyzers.Tests.Tips;

[TestClass]
public sealed class Xunit3Tests
{
    [TestMethod]
    [DataRow("string reason", "Assert.Skip(reason);")]
    [DataRow("bool condition, string reason", "Assert.SkipWhen(condition, reason);")]
    [DataRow("bool condition, string reason", "Assert.SkipUnless(condition, reason);")]
    [Implemented]
    public void SkipMethod_NoDiagnostic(string methodArguments, string assertion)
        => VerifyCSharpNoDiagnostic(methodArguments, assertion);

    private void VerifyCSharpNoDiagnostic(string methodArguments, string assertion)
    {
        var source = GenerateCode.XunitAssertion(methodArguments, assertion);
        DiagnosticVerifier.VerifyDiagnostic(new DiagnosticVerifierArguments()
            .WithAllAnalyzers()
            .WithSources(source)
            .WithPackageReferences(PackageReference.AwesomeAssertions_latest, PackageReference.Xunit3Assert_3_0_0)
            .WithExpectedDiagnostics()
        );
    }
}
