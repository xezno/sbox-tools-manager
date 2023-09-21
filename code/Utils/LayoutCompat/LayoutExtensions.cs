namespace Editor;

/// <summary>
/// Contains extension methods for <see cref="Layout"/>.
/// </summary>
internal static class LayoutExtensions
{
	/// <summary>
	/// Adds a child <see cref="Layout"/>.
	/// </summary>
	/// <param name="parentLayout">The parent <see cref="Layout"/> to add the newly created one to.</param>
	/// <param name="layoutMode">The mode that the layout should use.</param>
	/// <param name="stretch">The stretch value to pass to QT.</param>
	/// <returns>The newly created <see cref="Layout"/>.</returns>
	internal static Layout Add( this Layout parentLayout, LayoutMode layoutMode, int stretch = default )
	{
		var childLayout = layoutMode.CreateLayout();
		parentLayout.Add( childLayout, stretch );
		return childLayout;
	}
}
