using System.Runtime.CompilerServices;

namespace Get.Data.Properties;

public static class AutoTyper
{
    /// <summary>
    /// Shorthand and type-infered way to create <see cref="Property{T}"/>
    /// </summary>
    /// <typeparam name="T">The type to create, hopefully filled by compiler for you</typeparam>
    /// <param name="defaultValue">The default value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IProperty<T> Auto<T>(T defaultValue)
        => new Property<T>(defaultValue);

    /// <summary>
    /// Shprthand and type-infered way to create <see cref="ReadOnlyPropertyConstant{T}"/>
    /// </summary>
    /// <typeparam name="T">The type to create, hopefully filled by compiler for you</typeparam>
    /// <param name="defaultValue">The default value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyProperty<T> AutoReadOnly<T>(T defaultValue)
        => new ReadOnlyPropertyConstant<T>(defaultValue);

    /// <summary>
    /// Shorthand and type-infered way to create <see cref="ReadOnlyProperty{T}"/>
    /// </summary>
    /// <typeparam name="T">The type to create, hopefully filled by compiler for you</typeparam>
    /// <param name="defaultValue">The default value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyProperty<T> AutoReadOnly<T>(IProperty<T> defaultValue)
        => new ReadOnlyProperty<T>(defaultValue);
    /// <summary>
    /// Shorthand and type-infered way to create <see cref="AsyncWriteProperty{T}"/>
    /// </summary>
    /// <typeparam name="T">The type to create, hopefully filled by compiler for you</typeparam>
    /// <param name="defaultValue">The default value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncWriteProperty<T> AutoAsync<T>(T defaultValue)
        => new AsyncWriteProperty<T>(defaultValue);
}
