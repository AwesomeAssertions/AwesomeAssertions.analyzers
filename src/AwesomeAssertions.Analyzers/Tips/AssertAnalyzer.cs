/*
 * MIT License
 *
 * Copyright (c) 2023 Gérald Barré (https://www.meziantou.net)
 * Modifications copyright (c) Meir Blachman.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Immutable;
using System.Linq;
using AwesomeAssertions.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace AwesomeAssertions.Analyzers.Tips;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AssertAnalyzer : DiagnosticAnalyzer
{
    public static readonly string Message = "Use AwesomeAssertions equivalent";

    public static readonly DiagnosticDescriptor XunitRule = new(
       RuleIdentifiers.XunitRule,
       title: "Replace Xunit assertion with Fluent Assertions equivalent",
       messageFormat: Message,
       category: Constants.Tips.Category,
       defaultSeverity: DiagnosticSeverity.Info,
       isEnabledByDefault: true,
       description: "");

    public static readonly DiagnosticDescriptor MSTestsRule = new(
       RuleIdentifiers.MsTestRule,
       title: "Replace MSTests assertion with Fluent Assertions equivalent",
       messageFormat: Message,
       category: Constants.Tips.Category,
       defaultSeverity: DiagnosticSeverity.Info,
       isEnabledByDefault: true,
       description: "");

    public static readonly DiagnosticDescriptor NUnitRule = new(
       RuleIdentifiers.NunitRule,
       title: "Replace NUnit assertion with Fluent Assertions equivalent",
       messageFormat: Message,
       category: Constants.Tips.Category,
       defaultSeverity: DiagnosticSeverity.Info,
       isEnabledByDefault: true,
       description: "");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(XunitRule, MSTestsRule, NUnitRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(context =>
        {
            if (context.Compilation.GetTypeByMetadataName("AwesomeAssertions.AssertionExtensions") is null)
            {
                return;
            }

            var analyzerContext = new AnalyzerContext(context.Compilation);

            if (analyzerContext.IsXUnitAvailable)
            {
                context.RegisterOperationAction(analyzerContext.AnalyzeXunitInvocation, OperationKind.Invocation);
            }

            if (analyzerContext.IsMSTestsAvailable)
            {
                context.RegisterOperationAction(analyzerContext.AnalyzeMsTestInvocation, OperationKind.Invocation);
                context.RegisterOperationAction(analyzerContext.AnalyzeMsTestThrow, OperationKind.Throw);
            }

            if (analyzerContext.IsNUnitAvailable)
            {
                context.RegisterOperationAction(analyzerContext.AnalyzeNunitInvocation, OperationKind.Invocation);
                context.RegisterOperationAction(analyzerContext.AnalyzeNunitDynamicInvocation, OperationKind.DynamicInvocation);
                context.RegisterOperationAction(analyzerContext.AnalyzeNunitThrow, OperationKind.Throw);
            }
        });
    }

    private sealed class AnalyzerContext(Compilation compilation)
    {
        private readonly INamedTypeSymbol xunitAssertSymbol = compilation.GetTypeByMetadataName("Xunit.Assert");

        private readonly INamedTypeSymbol msTestsAssertSymbol = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.Assert");
        private readonly INamedTypeSymbol msTestsStringAssertSymbol = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.StringAssert");
        private readonly INamedTypeSymbol msTestsCollectionAssertSymbol = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert");
        private readonly INamedTypeSymbol msTestsUnitTestAssertExceptionSymbol = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.UnitTestAssertException");
        private readonly INamedTypeSymbol msTestsAssertInconclusiveExceptionSymbol = compilation.GetTypeByMetadataName("Microsoft.VisualStudio.TestTools.UnitTesting.AssertInconclusiveException");

        private readonly INamedTypeSymbol nunitAssertionExceptionSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.AssertionException");
        private readonly INamedTypeSymbol nunitAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.Assert");
        private readonly INamedTypeSymbol nunitCollectionAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.CollectionAssert") ?? compilation.GetTypeByMetadataName("NUnit.Framework.Legacy.CollectionAssert");
        private readonly INamedTypeSymbol nunitDirectoryAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.DirectoryAssert");
        private readonly INamedTypeSymbol nunitFileAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.FileAssert");
        private readonly INamedTypeSymbol nunitStringAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.StringAssert");
        private readonly INamedTypeSymbol nunitClassicAssertSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.Legacy.ClassicAssert");
        private readonly INamedTypeSymbol nunitResultStateExceptionSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.ResultStateException");
        private readonly INamedTypeSymbol nunitSuccessExceptionSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.SuccessException");
        private readonly INamedTypeSymbol nunitIgnoreExceptionSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.IgnoreException");
        private readonly INamedTypeSymbol nunitInconclusiveExceptionSymbol = compilation.GetTypeByMetadataName("NUnit.Framework.InconclusiveException");

        public bool IsMSTestsAvailable => msTestsAssertSymbol is not null;

        public bool IsNUnitAvailable => nunitAssertionExceptionSymbol is not null;

        public bool IsXUnitAvailable => xunitAssertSymbol is not null;

        private static readonly char[] SymbolsSeparators = ['|'];

        private bool IsMethodExcluded(AnalyzerOptions options, IInvocationOperation operation)
        {
            SyntaxTree location = operation.Syntax.GetLocation().SourceTree;
            if (location is null)
            {
                return false;
            }

            AnalyzerConfigOptions fileOptions = options.AnalyzerConfigOptionsProvider.GetOptions(location);
            if (fileOptions is null)
            {
                return false;
            }

            if (!fileOptions.TryGetValue("ffa_excluded_methods", out var symbolDocumentationIds))
            {
                return false;
            }

            return symbolDocumentationIds.Split(SymbolsSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Any(part => DocumentationCommentId.GetSymbolsForDeclarationId(part, compilation).Any(operation.TargetMethod.EqualsSymbol));
        }

        public void AnalyzeXunitInvocation(OperationAnalysisContext context)
        {
            var op = (IInvocationOperation)context.Operation;

            if (op.TargetMethod.ContainingType.EqualsSymbol(xunitAssertSymbol) && !IsMethodExcluded(context.Options, op))
            {
                if (op.TargetMethod.Name is "Skip" or "SkipWhen" or "SkipUnless")
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(XunitRule, op.Syntax.GetLocation()));
            }
        }

        public void AnalyzeMsTestInvocation(OperationAnalysisContext context)
        {
            var op = (IInvocationOperation)context.Operation;
            if (IsMsTestAssertClass(op.TargetMethod.ContainingType) && !IsMethodExcluded(context.Options, op))
            {
                if (op.TargetMethod.Name is "Fail" or "Inconclusive" && op.TargetMethod.ContainingType.EqualsSymbol(msTestsAssertSymbol))
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(MSTestsRule, op.Syntax.GetLocation()));
            }
        }

        public void AnalyzeMsTestThrow(OperationAnalysisContext context)
        {
            var op = (IThrowOperation)context.Operation;
            ITypeSymbol exceptionSymbol = op.Exception?.UnwrapConversion().Type;

            if (exceptionSymbol is not null &&
                exceptionSymbol.IsOrInheritsFrom(msTestsUnitTestAssertExceptionSymbol) &&
                !exceptionSymbol.EqualsSymbol(msTestsAssertInconclusiveExceptionSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(MSTestsRule, op.Syntax.GetLocation()));
            }
        }

        public void AnalyzeNunitInvocation(OperationAnalysisContext context)
        {
            var op = (IInvocationOperation)context.Operation;
            if (IsNunitAssertClass(op.TargetMethod.ContainingType) && !IsMethodExcluded(context.Options, op))
            {
                if (op.TargetMethod.Name is "Inconclusive" or "Ignore" or "Pass" or "Warn" or "Charlie" && op.TargetMethod.ContainingType.EqualsSymbol(nunitAssertSymbol))
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(NUnitRule, op.Syntax.GetLocation()));
            }
        }

        public void AnalyzeNunitDynamicInvocation(OperationAnalysisContext context)
        {
            var op = (IDynamicInvocationOperation)context.Operation;

            if (op.Arguments.Length < 2)
            {
                return;
            }

            ITypeSymbol containingType = ((op.Arguments[1]
                        .Parent as IDynamicInvocationOperation)?
                        .Operation as IDynamicMemberReferenceOperation)?
                        .ContainingType;
            if (IsNunitAssertClass(containingType))
            {
                context.ReportDiagnostic(Diagnostic.Create(NUnitRule, op.Syntax.GetLocation()));
            }
        }

        public void AnalyzeNunitThrow(OperationAnalysisContext context)
        {
            var op = (IThrowOperation)context.Operation;
            ITypeSymbol exceptionType = op.Exception?.UnwrapConversion().Type;

            // some NUnit exception don't have have an AA equivalent. Though they might probably
            // better used with e.g. Assert.Ignore, it is valid to use them.
            if (exceptionType.IsOrInheritsFrom(nunitResultStateExceptionSymbol)
                && !exceptionType.IsOrInheritsFrom(nunitIgnoreExceptionSymbol)
                && !exceptionType.IsOrInheritsFrom(nunitInconclusiveExceptionSymbol)
                && !exceptionType.IsOrInheritsFrom(nunitSuccessExceptionSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(NUnitRule, op.Syntax.GetLocation()));
            }
        }

        private bool IsMsTestAssertClass(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is null)
            {
                return false;
            }

            return typeSymbol.EqualsSymbol(msTestsAssertSymbol)
                || typeSymbol.EqualsSymbol(msTestsStringAssertSymbol)
                || typeSymbol.EqualsSymbol(msTestsCollectionAssertSymbol);
        }

        private bool IsNunitAssertClass(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is null)
            {
                return false;
            }

            return typeSymbol.EqualsSymbol(nunitAssertSymbol)
                || typeSymbol.EqualsSymbol(nunitCollectionAssertSymbol)
                || typeSymbol.EqualsSymbol(nunitDirectoryAssertSymbol)
                || typeSymbol.EqualsSymbol(nunitFileAssertSymbol)
                || typeSymbol.EqualsSymbol(nunitStringAssertSymbol)
                || typeSymbol.EqualsSymbol(nunitClassicAssertSymbol);
        }
    }
}
