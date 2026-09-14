namespace Asm.Domain;

/// <summary>
/// Marks a navigation property that is populated from the data store.
/// </summary>
/// <remarks>
/// The property and its containing types must be declared <see langword="partial"/>; the
/// implementation is generated. Reading the property before the navigation has been loaded throws
/// <see cref="InvalidOperationException"/>, so an unloaded navigation fails where it is used rather
/// than travelling on as a null the type system said could not happen.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class NavigationAttribute : Attribute
{
}
