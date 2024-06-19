namespace MiniZinc.Core;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Subset of the CommunityExtensions.Guard API
/// </summary>
public static class Guard
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsEqualTo<T>(
        T value,
        T target,
        [CallerArgumentExpression(nameof(value))] string name = ""
    )
        where T : IEquatable<T>
    {
        if (value.Equals(target))
        {
            return;
        }
        throw new ArgumentException(
            $"Parameter {name} ({typeof(T)}) must be equal to {target}, was {value}.",
            name
        );
    }

    /// <summary>
    /// Asserts that the input value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of reference value type being tested.</typeparam>
    /// <param name="value">The input value to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsNotNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string name = ""
    )
    {
        if (value is not null)
        {
            return;
        }

        throw new ArgumentNullException(name, $"Parameter {name} ({typeof(T)}) must be not null).");
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsTrue(
        [DoesNotReturnIf(false)] bool value,
        [CallerArgumentExpression(nameof(value))] string name = ""
    )
    {
        if (value)
        {
            return;
        }

        throw new ArgumentException($"Parameter {name} must be true, was false.", name);
    }

    /// <summary>
    /// Asserts that the input value must be <see langword="true"/>.
    /// </summary>
    /// <param name="value">The input <see cref="bool"/> to test.</param>
    /// <param name="name">The name of the input parameter being tested.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="false"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IsFalse(
        [DoesNotReturnIf(false)] bool value,
        [CallerArgumentExpression(nameof(value))] string name = ""
    )
    {
        if (!value)
        {
            return;
        }

        throw new ArgumentException($"Parameter {name} must be false, was true.", name);
    }
}
