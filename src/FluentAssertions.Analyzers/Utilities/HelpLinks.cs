﻿using System;
using System.Collections.Generic;

namespace FluentAssertions.Analyzers;

public static class HelpLinks
{
    private static readonly Dictionary<Type, string> TypesHelpLinks;
    private static string GetHelpLink(string id) => $"https://fluentassertions.com/tips/#{id}";

    static HelpLinks()
    {
        TypesHelpLinks = new Dictionary<Type, string>
        {
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyWhichMessageShouldContain)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyAndMessageShouldContain)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowWhichMessageShouldContain)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowAndMessageShouldContain)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyWhichMessageShouldBe)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowWhichMessageShouldBe)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyAndMessageShouldBe)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowAndMessageShouldBe)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyWhichMessageShouldStartWith)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowWhichMessageShouldStartWith)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyAndMessageShouldStartWith)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowAndMessageShouldEndWith)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyWhichMessageShouldEndWith)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowWhichMessageShouldEndWith)] = GetHelpLink("Exceptions-2"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowExactlyAndMessageShouldEndWith)] = GetHelpLink("Exceptions-1"),
            [typeof(ExceptionShouldThrowWithMessageAnalyzer.ShouldThrowAndMessageShouldStartWith)] = GetHelpLink("Exceptions-2"),
        };
    }

    public static string Get(Type type)
        => TypesHelpLinks.TryGetValue(type, out var value) ? value : string.Empty; 
}
