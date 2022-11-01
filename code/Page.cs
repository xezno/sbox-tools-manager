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

	private Label LatestRelease;
	private Label LatestReleaseName;
	private Label LatestReleaseBody;

	private bool HasFetched = false;

	public Page( Sandbox.LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		SetLayout( LayoutMode.TopToBottom );

		Layout.Spacing = 8;
		Layout.Margin = 24;

		var config = project.Config;
		Header = Layout.Add( new Header( config.Title ) );
		Layout.AddSpacingCell( 8 );

		{
			ToolBar = new ToolBar( this );
			ToolBar.SetIconSize( 16 );
			ToolBar.AddOption( "Update", "download", () => Log.Trace( "download" ) );
			ToolBar.AddOption( "Open in Explorer", "folder", () => Utility.OpenFolder( System.IO.Path.GetDirectoryName( project.GetRootPath() ) ) );
			ToolBar.AddOption( "Delete Tool", "delete", () => Log.Trace( "download" ) );
			ToolBar.AddOption( "Open in GitHub", "open_in_new" );
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
		LatestRelease = Layout.Add( new Label( $"Loading..." ) );
		LatestReleaseName = Layout.Add( new Label( $"Loading..." ) );
		LatestReleaseBody = Layout.Add( new Label( $"Loading..." ) );
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( HasFetched )
			return;

		GithubApi.FetchLatestRelease( "xezno/sbox-quixel-bridge" ).ContinueWith( t =>
		{
			var latestRelease = t.Result ?? default;

			LatestRelease.Text = $"Latest release: {latestRelease.TagName}";
			LatestReleaseName.Text = latestRelease.Name;
			LatestReleaseBody.Text = latestRelease.Body;
		} );

		HasFetched = true;
	}

	protected override void DoLayout()
	{
		base.DoLayout();

		Header.Position = 0;
		Header.Height = 512;

		ToolBar.Position = new( 16, 44 );
	}
}
