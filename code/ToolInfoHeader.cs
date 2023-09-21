namespace Editor;

//
// Copied this from base inspector stuff so that things stay consistent,
// but this isn't ideal.. could do with our own proper widget for stuff
// like this
//
internal class ToolInfoHeader : Widget
{
	const float HeaderHeight = 64 + 8;
	private string Title;

	public ToolInfoHeader( string title, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		Title = title;
		Height = HeaderHeight;

		this.SetLayout( LayoutMode.TopToBottom );
	}

	protected override void OnPaint()
	{
		Paint.SetBrushRadial( new Vector2( Width * 0.25f, 0 ), Width * 0.75f, Theme.Primary.WithAlpha( 0.2f ), Theme.Primary.WithAlpha( 0.01f ) );
		Paint.ClearPen();
		Paint.DrawRect( new Rect( new Vector2( 0, 0 ), new Vector2( Width, HeaderHeight ) ) );

		Paint.SetBrushRadial( 0, Width, Theme.White.WithAlpha( 0.2f ), Theme.Primary.WithAlpha( 0.0f ) );
		Paint.ClearPen();
		Paint.DrawRect( new Rect( new Vector2( 0, HeaderHeight - 26 ), new Vector2( Width, 26 ) ) );

		Paint.RenderMode = RenderMode.Screen;
		var pos = new Vector2( 24, 8 );
		Paint.SetPen( Theme.White );
		Paint.SetFont( "Poppins", 13, 450 );
		var r = Paint.DrawText( pos, Title );
		pos.y = r.Bottom;

		//r.Left = r.Right;
		//r.Width = 32;
		//r.Top -= 4;
		//Paint.DrawIcon( r, "update", 16 );
	}
}
