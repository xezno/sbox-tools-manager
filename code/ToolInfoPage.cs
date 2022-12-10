namespace Editor;

internal class ToolInfoPage : Widget
{
	private ToolInfoHeader Header;
	private ToolBar ToolBar;

	private Label LatestReleaseName;
	private Label LatestReleaseBody;

	private Manifest Manifest;
	private LocalProject Project;

	private bool HasFetched;

	public ToolInfoPage( LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		SetLayout( LayoutMode.TopToBottom );

		Layout.Spacing = 8;
		Layout.Margin = 24;

		UpdateFromProject( project );
	}

	private void UpdateFromProject( LocalProject project )
	{
		DestroyChildren();

		Project = project;
		Manifest = project.GetManifest();

		if ( Manifest != null )
			AddManifestWidgets();
		else
			AddNoManifestWidgets();
	}

	/// <summary>
	/// Displays information about this tool, along with release info
	/// </summary>
	private void AddManifestWidgets()
	{
		var config = Project.Config;

		// Basic repo info
		Header = Layout.Add( new ToolInfoHeader( config.Title ) );
		Layout.AddSpacingCell( 8 );

		ToolBar = new ToolBar( this );
		ToolBar.SetIconSize( 16 );

		ToolBar.AddOption( "Open in Explorer", "folder", () => Utility.OpenFolder( Path.GetDirectoryName( Project.GetRootPath() ) ) );
		ToolBar.AddOption( "Open on GitHub", "open_in_new", () => Utility.OpenFolder( $"https://github.com/{Manifest.Repo}" ) );
		Layout.Add( ToolBar );

		Layout.AddSpacingCell( 8f );

		Layout.Add( new Label( $"{Manifest.Description}" ) { WordWrap = true } );

		// Installed release info
		{
			var scroll = new ScrollArea( this );
			var canvas = new Widget( this );
			canvas.SetLayout( LayoutMode.TopToBottom );

			canvas.Layout.Add( new Subheading( $"{Manifest.ReleaseName}" ) );
			canvas.Layout.Add( new Label( $"{Manifest.ReleaseDescription}" ) { WordWrap = true } );
			canvas.Layout.AddStretchCell();

			scroll.Canvas = canvas;

			Layout.Add( scroll );
		}

		// Update info (if available)
		if ( Manifest.CheckUpdateAvailable() )
		{
			var group = new Container( this );
			group.SetLayout( LayoutMode.TopToBottom );
			group.Layout.Margin = 10;
			Layout.Add( group );

			{
				group.Layout.Add( new Heading( "Update Available" ) );

				var scroll = new ScrollArea( this );
				var canvas = new Widget( this );
				canvas.SetLayout( LayoutMode.TopToBottom );

				LatestReleaseName = canvas.Layout.Add( new Subheading( $"Loading..." ) );
				LatestReleaseBody = canvas.Layout.Add( new Label( $"Loading..." ) );

				scroll.Canvas = canvas;
				group.Layout.Add( scroll );
			}

			group.Layout.AddSpacingCell( 8f );
			group.Layout.Add( new Button( "Download Update", "download" ) { Clicked = DownloadUpdate } );
		}
	}

	/// <summary>
	/// Downloads the latest update for this page's project using the manifest
	/// </summary>
	private void DownloadUpdate()
	{
		var release = GithubApi.FetchLatestRelease( $"{Manifest.Repo}" ).Result;

		var folder = Project.GetRootPath();

		Log.Trace( folder );

		using var progress = Progress.Start( $"Updating {Project.Config.Title}" );

		Progress.Update( "Removing local changes", 5, 100 );
		_ = GitUtils.Git( $"reset --hard HEAD" );
		_ = Task.Delay( 50 );


		Progress.Update( "Pulling remote changes", 25, 100 );
		_ = GitUtils.Git( $"pull" );
		_ = Task.Delay( 50 );

		Progress.Update( "Checking out release...", 90, 100 );
		_ = GitUtils.Git( $"checkout \"{release.TagName}\" --force", folder );
		_ = Task.Delay( 50 );

		// Update Manifest
		Manifest.SetRelease( release );
		Manifest.WriteToFolder( folder );

		// Refresh UI
		UpdateFromProject( Project );
	}

	/// <summary>
	/// Displays text telling the user to add the tool properly
	/// </summary>
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
