using Sandbox;
using System.IO;

namespace Tools;

internal class ToolInfoPage : Widget
{
	private LocalTool Tool;

	private ToolInfoHeader Header;
	private ToolBar ToolBar;

	private Label LatestReleaseName;
	private Label LatestReleaseBody;

	private bool HasFetched;

	public ToolInfoPage( LocalProject project, Widget parent = null, bool isDarkWindow = false ) : base( parent, isDarkWindow )
	{
		SetLayout( LayoutMode.TopToBottom );

		Layout.Spacing = 8;
		Layout.Margin = 24;

		UpdateFromTool( new LocalTool( project ) );
	}

	private void UpdateFromTool( LocalTool tool )
	{
		Tool = tool;

		DestroyChildren();

		if ( Tool.IsValid() )
			AddWidgets();
		else
			AddErrorWidgets();
	}

	/// <summary>
	/// Displays information about this tool, along with release info
	/// </summary>
	private void AddWidgets()
	{
		// Basic repo info
		Header = Layout.Add( new ToolInfoHeader( Tool.Title ) );
		Layout.AddSpacingCell( 8 );

		ToolBar = new ToolBar( this );
		ToolBar.SetIconSize( 16 );

		ToolBar.AddOption( "Open in Explorer", "folder", () => Utility.OpenFolder( Path.GetDirectoryName( Tool.RootPath ) ) );
		ToolBar.AddOption( "Open on GitHub", "open_in_new", () => Utility.OpenFolder( Tool.Url ) );
		Layout.Add( ToolBar );

		Layout.AddSpacingCell( 8f );

		Layout.Add( new Label( $"{Tool.Description}" ) { WordWrap = true } );

		// Installed release info
		{
			var scroll = new ScrollArea( this );
			var canvas = new Widget( this );
			canvas.SetLayout( LayoutMode.TopToBottom );

			canvas.Layout.Add( new Subheading( $"{Tool.CurrentRelease.Name}" ) );
			canvas.Layout.Add( new Label( $"{Tool.CurrentRelease.Body}" ) { WordWrap = true } );
			canvas.Layout.AddStretchCell();

			scroll.Canvas = canvas;

			Layout.Add( scroll );
		}

		// Update info (if available)
		if ( Tool.NeedsUpdate() )
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
		if ( Tool.Update() )
		{
			// Refresh UI
			UpdateFromTool( Tool );
		}
	}

	/// <summary>
	/// Displays text telling the user to add the tool properly
	/// </summary>
	private void AddErrorWidgets()
	{
		Layout.Add( new Label.Title( "😔" ) ).SetStyles( "font-size: 64px;" );
		Layout.Add( new Label.Body( $"'{Tool.Title}' does not have a manifest file, because you didn't import " +
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

		var latestRelease = Tool.LatestRelease;

		if ( LatestReleaseName == null || LatestReleaseBody == null )
			return;

		LatestReleaseName.Text = latestRelease.Name;
		LatestReleaseBody.Text = latestRelease.Body;

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
