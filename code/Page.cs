using System.Net.Http;
using System.Text.Json;

namespace Tools;

internal class Page : Widget
{
	private Header Header;
	private ToolBar ToolBar;

	// TODO: Make and import a shared tools library, because maintaining these mini-widgets
	// across separate tools is going to become a mare
	private Label AddTitle( string text, float size = 16 )
	{
		var label = new Label( text );
		label.SetStyles( $"font-size: {size}px; font-weight: 600; font-family: Poppins;" );

		return label;
	}

	public Page( Sandbox.LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		var req = new HttpClient();
		req.DefaultRequestHeaders.UserAgent.ParseAdd( "s&box" );
		var content = req.GetAsync( "https://api.github.com/repos/xezno/sbox-quixel-bridge/releases/latest" ).Result;
		var result = content.Content.ReadAsStringAsync().Result;

		var release = JsonSerializer.Deserialize<Release>( result );
		Log.Trace( release );

		SetLayout( LayoutMode.TopToBottom );

		Layout.Spacing = 8;
		Layout.Margin = 24;

		var config = project.Config;
		Header = Layout.Add( new Header( config.Title ) );
		Layout.AddSpacingCell( 8 );

		{
			ToolBar = new ToolBar( this );
			ToolBar.SetIconSize( 16 );
			ToolBar.AddOption( null, "download", () => Log.Trace( "download" ) );
			ToolBar.AddOption( null, "folder", () => Utility.OpenFolder( System.IO.Path.GetDirectoryName( project.GetRootPath() ) ) );
			ToolBar.AddOption( null, "delete", () => Log.Trace( "download" ) );
			Layout.Add( ToolBar );
		}

		Layout.AddSpacingCell( 8f );

		var desc = Layout.Add( new Label( "This tool adds editor integrations for Quixel Bridge, allowing you to access your Quixel assets from within s&box." ) );
		desc.WordWrap = true;

		Layout.AddSpacingCell( 8f );

		Layout.Add( AddTitle( "Installed" ) );
		Layout.Add( new Label( "Version: 0.0.1" ) );
		Layout.Add( new Label( "Lorem ipsum dolor sit amet" ) );

		Layout.AddStretchCell();

		Layout.Add( AddTitle( "GitHub" ) );
		Layout.Add( new Label( $"Latest release: {release.TagName}" ) );
		Layout.Add( new Label( $"{release.Name}" ) );
		Layout.Add( new Label( $"{release.Body}" ) );
	}

	protected override void DoLayout()
	{
		base.DoLayout();

		Header.Position = 0;
		Header.Height = 512;

		ToolBar.Position = new( 16, 44 );
	}
}
