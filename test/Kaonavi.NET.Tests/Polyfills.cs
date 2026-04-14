#if !NET7_0_OR_GREATER
// Polyfills for compiler-required members added in .NET 7 / C# 11

namespace System.Runtime.CompilerServices
{
    /// <summary>Polyfill for <c>RequiredMemberAttribute</c> (added in .NET 7, needed for <c>required</c> keyword).</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    /// <summary>Polyfill for <c>CompilerFeatureRequiredAttribute</c> (added in .NET 7, needed for <c>required</c> keyword).</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName) { FeatureName = featureName; }
        public string FeatureName { get; }
        public bool IsOptional { get; init; }
        public const string RefStructs = nameof(RefStructs);
        public const string RequiredMembers = nameof(RequiredMembers);
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>Polyfill for <c>StringSyntaxAttribute</c> (added as public API in .NET 7).</summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class StringSyntaxAttribute : Attribute
    {
        public StringSyntaxAttribute(string syntax) { Syntax = syntax; Arguments = Array.Empty<object?>(); }
        public StringSyntaxAttribute(string syntax, params object?[] arguments) { Syntax = syntax; Arguments = arguments; }
        public string Syntax { get; }
        public object?[] Arguments { get; }
        public const string Json = "Json";
        public const string Uri = "Uri";
        public const string Xml = "Xml";
        public const string Regex = "Regex";
    }
}
#endif
