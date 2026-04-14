#if NETSTANDARD2_1
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace System.Runtime.CompilerServices
{
    /// <summary>Polyfill for <see cref="CallerArgumentExpressionAttribute"/> (added in .NET 6).</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute(string parameterName) : Attribute
    {
        /// <inheritdoc cref="CallerArgumentExpressionAttribute"/>
        public string ParameterName { get; } = parameterName;
    }

    /// <summary>Polyfill required to support <c>init</c> accessors (C# 9) on .NET Standard 2.1.</summary>
    internal static class IsExternalInit { }
}

namespace Kaonavi.Net
{
    internal static class ArgumentNullExceptionPolyfills
    {
        extension(ArgumentNullException)
        {
            /// <summary>Polyfill for <c>ArgumentNullException.ThrowIfNull</c> (added in .NET 7).</summary>
            public static void ThrowIfNull(
                [NotNullWhen(false)] object? argument,
                [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                    throw new ArgumentNullException(paramName);
            }
        }
    }

    internal static class ObjectDisposedExceptionPolyfills
    {
        extension(ObjectDisposedException)
        {
            /// <summary>Polyfill for <c>ObjectDisposedException.ThrowIf</c> (added in .NET 7).</summary>
            public static void ThrowIf([DoesNotReturnIf(true)] bool condition, Type type)
            {
                if (condition)
                    throw new ObjectDisposedException(type.FullName);
            }
        }
    }

    internal static class ArgumentOutOfRangeExceptionPolyfills
    {
        extension(ArgumentOutOfRangeException)
        {
            /// <summary>Polyfill for <c>ArgumentOutOfRangeException.ThrowIfNegative</c> (added in .NET 8).</summary>
            public static void ThrowIfNegative(
                int value,
                [CallerArgumentExpression(nameof(value))] string? paramName = null)
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(paramName, value, null);
            }
        }
    }

    internal static class HttpContentPolyfills
    {
        extension(HttpContent content)
        {
            /// <summary>Polyfill for <c>HttpContent.ReadAsStringAsync(CancellationToken)</c> (added in .NET 5).</summary>
            public System.Threading.Tasks.Task<string> ReadAsStringAsync(System.Threading.CancellationToken cancellationToken)
                => content.ReadAsStringAsync();
        }
    }
}

namespace Kaonavi.Net.Json
{
    internal static class JsonMetadataServicesPolyfills
    {
        extension(JsonMetadataServices)
        {
            /// <summary>Polyfill for <c>JsonMetadataServices.DateOnlyConverter</c> (added in .NET 8).</summary>
            public static JsonConverter<DateOnly> DateOnlyConverter => new DateOnlyConverter();
        }
    }
}
#endif
