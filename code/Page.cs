using Sandbox;
using System.IO;

namespace Tools;

internal class Page : Widget
{
	private Header Header;
	private ToolBar ToolBar;

	private Label LatestReleaseName;
	private Label LatestReleaseBody;

	private Manifest Manifest;
	private LocalProject Project;

	private bool HasFetched = false;

	public Page( LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		SetLayout( LayoutMode.TopToBottom );

		this.Project = project;

		Layout.Spacing = 8;
		Layout.Margin = 24;

		//
		// Check if we have a tools manager manifest
		//
		Manifest = project.GetManifest();

		if ( Manifest != null )
		{
			AddManifestWidgets();
		}
		else
		{
			// Display sad face
			AddNoManifestWidgets();
		}
	}

	private void AddManifestWidgets()
	{
		var config = Project.Config;
		Header = Layout.Add( new Header( config.Title ) );
		Layout.AddSpacingCell( 8 );

		{
			ToolBar = new ToolBar( this );
			ToolBar.SetIconSize( 16 );

			// var autoUpdateOption = new Option( "Toggle Auto-Updates", Manifest.AutoUpdate ? "file_download" : "file_download_off" );
			// autoUpdateOption.Triggered = () => ToggleAutoUpdates( autoUpdateOption );
			// var option = ToolBar.AddOption( autoUpdateOption );

			ToolBar.AddOption( "Open in Explorer", "folder", () => Utility.OpenFolder( Path.GetDirectoryName( Project.GetRootPath() ) ) );
			ToolBar.AddOption( "Open on GitHub", "open_in_new", () => Utility.OpenFolder( $"https://github.com/{Manifest.Repo}" ) );
			Layout.Add( ToolBar );
		}

		Layout.AddSpacingCell( 8f );

		Layout.Add( new Label.Body( $"{Manifest.Description}" ) );

		Layout.Add( new Subheading( $"{Manifest.ReleaseName}" ) );
		Layout.Add( new Label.Body( $"{Manifest.ReleaseDescription}" ) );

		Layout.AddStretchCell();

		if ( Manifest.CheckUpdateAvailable() )
		{
			var group = new Container( this );
			group.SetLayout( LayoutMode.TopToBottom );
			group.Layout.Margin = 10;
			Layout.Add( group );

			group.Layout.Add( new Heading( "Update Available" ) );
			LatestReleaseName = group.Layout.Add( new Subheading( $"Loading..." ) );
			LatestReleaseBody = group.Layout.Add( new Label( $"Loading..." ) );

			group.Layout.AddSpacingCell( 8f );

			group.Layout.Add( new Button( "Download Update", "download" ) { Clicked = DownloadUpdate } );
		}
	}

	private void DownloadUpdate()
	{
		GithubApi.FetchLatestRelease( $"{Manifest.Repo}" ).ContinueWith( async t =>
		{
			var release = t.Result ?? default;

			var folder = Project.GetRootPath();

			Log.Trace( folder );

			await GitUtils.Git( $"reset --hard HEAD" );
			await GitUtils.Git( $"pull" );
			await GitUtils.Git( $"checkout \"{release.TagName}\" --force", folder );

			//
			// Update Manifest
			//
			Manifest.ReleaseVersion = release.TagName;
			Manifest.ReleaseName = release.Name;
			Manifest.ReleaseDescription = release.Body;

			var manifestPath = Path.Combine( folder, "tm-manifest.json" );
			File.WriteAllText( manifestPath, Manifest.ToJson() );
		} );
	}

	private void ToggleAutoUpdates( Option option )
	{
		Manifest.AutoUpdate = !Manifest.AutoUpdate;
		option.Icon = Manifest.AutoUpdate ? "file_download" : "file_download_off";
	}

	private void AddNoManifestWidgets()
	{
		Layout.Add( new Label.Title( "😔" ) ).SetStyles( "font-size: 64px;" );
		Layout.Add( new Label.Body( $"'{Project.Config.Title}' does not have a manifest file, because you didn't import " +
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

			if ( LatestReleaseName == null || LatestReleaseBody == null )
				return;

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
