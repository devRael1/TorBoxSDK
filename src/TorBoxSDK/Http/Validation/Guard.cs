using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TorBoxSDK.Http.Validation;

/// <summary>
/// Internal guard helpers for argument validation that work across all target frameworks.
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is
    /// <see langword="null"/>, and returns the non-null value otherwise.
    /// </summary>
    /// <typeparam name="T">The type of the value to validate.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">
    /// The name of the calling parameter. Automatically captured via
    /// the <c>CallerArgumentExpression</c> compiler feature when not provided.
    /// </param>
    /// <returns>The validated, non-null <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    internal static T ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is
    /// <see langword="null"/> or empty.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="paramName">The name of the calling parameter.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is empty.
    /// </exception>
    internal static void ThrowIfNullOrEmpty(string? value, string? paramName = null)
    {
#if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(value, paramName);
#else
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentException("The value cannot be an empty string.", paramName);
        }
#endif
    }
}
