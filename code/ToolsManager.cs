using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools;

[Tool( "Tools Manager", "hardware", "Manages your tools." )]
public class ToolsManager : BaseWindow
{
	public ToolsManager()
	{
		Size = new Vector2( 600, 400 );
		MinimumSize = Size;
		WindowTitle = "Tools Manager";

		SetWindowIcon( "hardware" );

		WriteDummyManifest();

		CreateUI();
		Show();
	}

	/// <summary>
	/// Tools Manager needs to be able to manage itself, so we create a 'dummy' manifest just
	/// in case one wasn't packaged with the GitHub release.
	/// </summary>
	private void WriteDummyManifest()
	{
		var project = Utility.Projects.GetAll().First( x => x.Config.Ident == "tools_manager" );
		var folder = project.GetRootPath();

		var manifestPath = Path.Combine( folder, "tm-manifest.json" );

		if ( File.Exists( manifestPath ) )
			return;

		var manifest = new Manifest();
		manifest.ReleaseName = "None";
		manifest.ReleaseVersion = "0";
		manifest.ReleaseDescription = "Invalid release";

		manifest.Repo = "xezno/tools-manager";
		manifest.Description = "Manages your tools.";
		manifest.AutoUpdate = true;

		manifest.WriteToFile( manifestPath );
	}

	public void CreateUI()
	{
		SetLayout( LayoutMode.LeftToRight );
		var layout = Layout;

		var toolsList = layout.Add( new NavigationView( this ) );

		foreach ( var project in Utility.Projects.GetAll() )
		{
			var config = project.Config;

			if ( config.PackageType == Sandbox.Package.Type.Tool )
			{
				var option = toolsList.AddPage( config.Title, "hardware", new ToolInfoPage( project ) );
				var manifest = project.GetManifest();

				option.OnPaintOverride = () => PaintPageOption( option, manifest );
			}
		}

		var footer = toolsList.MenuBottom.AddRow();
		footer.Spacing = 4;

		var add = footer.Add( new Button.Primary( "Add Tool...", "add" ), 1 );
		add.Clicked = () => AddToolDialog.Open();
	}

	private bool PaintPageOption( NavigationView.Option option, Manifest manifest )
	{
		var fg = Theme.White.WithAlpha( 0.5f );

		if ( option.IsSelected )
		{
			fg = Theme.White;
		}

		Paint.ClearPen();
		Paint.SetBrush( Theme.WidgetBackground.WithAlpha( 0.0f ) );

		if ( Paint.HasMouseOver )
		{
			fg = Theme.White.WithAlpha( 0.8f );
		}

		Paint.TextAntialiasing = true;
		Paint.Antialiasing = true;

		Paint.DrawRect( option.LocalRect.Shrink( 0 ) );

		var inner = option.LocalRect.Shrink( 8, 0, 0, 0 );

		Paint.SetPen( fg.WithAlphaMultiplied( 0.8f ) );
		Paint.SetFont( "Poppins", 8, 440 );

		Paint.DrawText( inner, option.Title, TextFlag.LeftCenter );

		var iconRect = inner;
		iconRect.Left = inner.Right - 32;
		iconRect.Top -= 2;

		if ( manifest.CheckUpdateAvailable() )
		{
			Paint.SetPen( fg );
			Paint.DrawIcon( iconRect, "update", 14, TextFlag.Center );

			inner.Left += iconRect.Width + 4;
		}

		return true;
	}

	private static bool HasCheckedForUpdates = false;

	//
	// I'm going to use this event because it's a decent one
	// that runs when the editor starts plus infrequently
	// while developing, making it quite useful
	//
	[Sandbox.Event( "tools.compilemgr.start" )]
	public static void OnCompileMgrStart()
	{
		if ( HasCheckedForUpdates )
			return;

		var count = CheckForUpdates().Result;

		if ( count > 0 )
			ToolUpdateNotice.Open( count );

		HasCheckedForUpdates = true;
	}

	/// <summary>
	/// Check every tools project for any updates (if it has a valid manifest)
	/// </summary>
	private static async Task<int> CheckForUpdates()
	{
		int count = 0;

		foreach ( var project in Utility.Projects.GetAll() )
		{
			var manifest = project.GetManifest();

			if ( manifest == null )
				continue;

			Log.Trace( manifest );

			var latest = await GithubApi.FetchLatestRelease( manifest.Repo );

			if ( manifest.CheckUpdateAvailable( latest ) )
				count++;
		}

		return count;
	}
}
