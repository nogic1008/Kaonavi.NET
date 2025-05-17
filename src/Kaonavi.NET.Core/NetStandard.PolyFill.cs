using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_1
/// <summary>
/// .NET Standard 2.1用のポリフィル(.NET 8.0以降で追加された機能の簡易実装)を提供します。
/// </summary>
internal static class NetStandardPolyFillExtensions
{
    extension(ArgumentNullException)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
                Throw(paramName);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Throw(string? paramName) => throw new ArgumentNullException(paramName);
        }
    }

    extension(ArgumentOutOfRangeException)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value > 0)
                Throw(paramName);

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Throw(string? paramName) => throw new ArgumentOutOfRangeException(paramName);
        }
    }

    extension(ObjectDisposedException)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIf(bool disposed, Type type)
        {
            if (disposed)
                Throw(type.ToString());

            [DoesNotReturn]
            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Throw(string objectName) => throw new ObjectDisposedException(objectName);
        }
    }

    extension(HttpMethod)
    {
        public static HttpMethod Patch => new("PATCH");
    }

    extension(HttpContent content)
    {
        public Task<string> ReadAsStringAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return content.ReadAsStringAsync();
        }
    }
}
#endif
