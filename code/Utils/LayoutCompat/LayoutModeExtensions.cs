using System;

namespace Editor;

/// <summary>
/// Contains extension methods for <see cref="LayoutMode"/>.
/// </summary>
internal static class LayoutModeExtensions
{
	/// <summary>
	/// Creates a new <see cref="Layout"/> based on the mode.
	/// </summary>
	/// <param name="layoutMode">The mode for the <see cref="Layout"/> to follow.</param>
	/// <returns>The newly created <see cref="Layout"/>.</returns>
	/// <exception cref="ArgumentException">Thrown when the <see cref="LayoutMode"/> provided is invalid.</exception>
	internal static Layout CreateLayout( this LayoutMode layoutMode ) => layoutMode switch
	{
		LayoutMode.TopToBottom => Layout.Column(),
		LayoutMode.BottomToTop => Layout.Column( true ),
		LayoutMode.LeftToRight => Layout.Row(),
		LayoutMode.RightToLeft => Layout.Row( true ),
		_ => throw new ArgumentException( $"Unrecognized {nameof( LayoutMode )} \"{layoutMode}\"", nameof( layoutMode ) )
	};
}
