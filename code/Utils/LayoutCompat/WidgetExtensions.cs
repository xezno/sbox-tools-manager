namespace Editor;

/// <summary>
/// Contains extension methods for <see cref="Widget"/>.
/// </summary>
internal static class WidgetExtensions
{
	/// <summary>
	/// Sets the <see cref="Layout"/> of the <see cref="Widget"/>.
	/// </summary>
	/// <param name="widget">The <see cref="Widget"/> whose <see cref="Layout"/> to set.</param>
	/// <param name="layoutMode">The mode to use on the <see cref="Layout"/>.</param>
	/// <returns>The newly created <see cref="Layout"/>.</returns>
	internal static Layout SetLayout( this Widget widget, LayoutMode layoutMode )
	{
		var layout = layoutMode.CreateLayout();
		widget.Layout = layout;
		return layout;
	}
}
