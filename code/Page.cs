using Sandbox;
using System.IO;
using System.Text.Json;

namespace Tools;

internal class Page : Widget
{
	private Header Header;
	private ToolBar ToolBar;

	private Label LatestReleaseName;
	private Label LatestReleaseBody;

	private Manifest Manifest;

	private bool HasFetched = false;

	public Page( LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		SetLayout( LayoutMode.TopToBottom );

		Layout.Spacing = 8;
		Layout.Margin = 24;

		//
		// Check if we have a tools manager manifest
		//
		var manifestPath = project.GetManifestPath();

		if ( File.Exists( manifestPath ) )
		{
			// Load manifest
			Manifest = JsonSerializer.Deserialize<Manifest>( File.ReadAllText( manifestPath ) );

			AddManifestWidgets( project );
		}
		else
		{
			// Display sad face
			AddNoManifestWidgets( project );
		}
	}

	private void AddManifestWidgets( LocalProject project )
	{
		var config = project.Config;
		Header = Layout.Add( new Header( config.Title ) );
		Layout.AddSpacingCell( 8 );

		{
			ToolBar = new ToolBar( this );
			ToolBar.SetIconSize( 16 );
			ToolBar.AddOption( "Update", "download", () => Log.Trace( "Update" ) );
			ToolBar.AddOption( "Open in Explorer", "folder", () => Utility.OpenFolder( Path.GetDirectoryName( project.GetRootPath() ) ) );
			ToolBar.AddOption( "Open on GitHub", "open_in_new", () => Utility.OpenFolder( $"https://github.com/{Manifest.Repo}" ) );
			Layout.Add( ToolBar );
		}

		Layout.AddSpacingCell( 8f );

		var desc = Layout.Add( new Label( $"{Manifest.Description}" ) );
		desc.WordWrap = true;

		Layout.AddSpacingCell( 8f );

		Layout.Add( new Label.Subtitle( "Installed Release" ) );
		Layout.Add( new Label( $"{Manifest.ReleaseName}" ) );
		Layout.Add( new Label( $"{Manifest.ReleaseDescription}" ) );

		Layout.AddSpacingCell( 32f );

		Layout.Add( new Label.Subtitle( "Latest Release" ) );
		LatestReleaseName = Layout.Add( new Label( $"Loading..." ) );
		LatestReleaseBody = Layout.Add( new Label( $"Loading..." ) );

		Layout.AddStretchCell();
	}

	private void AddNoManifestWidgets( LocalProject project )
	{
		Layout.Add( new Label.Title( "😔" ) ).SetStyles( "font-size: 64px;" );
		Layout.Add( new Label.Body( $"'{project.Config.Title}' does not have a manifest file, because you didn't import " +
			"it through Tools Manager. Remove the tool and re-add it from GitHub by clicking \"Add Tool...\" " +
			"in the bottom left." ) );

		Layout.AddStretchCell();

		HasFetched = true;
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		if ( HasFetched )
			return;

		GithubApi.FetchLatestRelease( $"{Manifest.Repo}" ).ContinueWith( t =>
		{
			var latestRelease = t.Result ?? default;

			LatestReleaseName.Text = latestRelease.Name;
			LatestReleaseBody.Text = latestRelease.Body;
		} );

		HasFetched = true;
	}

	protected override void DoLayout()
	{
		base.DoLayout();

		if ( Header == null || ToolBar == null )
			return;

		Header.Position = 0;
		Header.Height = 512;

		ToolBar.Position = new( 16, 44 );
	}
}
