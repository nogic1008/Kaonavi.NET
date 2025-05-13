using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

internal static class NetStandardPolyFillExtensions
{
    extension(ArgumentException)
    {
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);
        }
    }

    extension(ArgumentOutOfRangeException)
    {
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value > 0)
                throw new ArgumentOutOfRangeException(paramName);
        }
    }

    extension(ObjectDisposedException)
    {
        public static void ThrowIf(bool disposed, Type type)
        {
            if (disposed)
                throw new ObjectDisposedException(type.ToString());
        }
    }

    extension(HttpMethod)
    {
        public static HttpMethod Patch => new("PATCH");
    }
}
