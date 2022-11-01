using Tools;

public class ToolsManager : BaseWindow
{
	[Menu( "Editor", "Tools Manager/Tools Manager" )]
	public static void Open()
	{
		_ = new ToolsManager();
	}

	public ToolsManager()
	{
		Size = new Vector2( 600, 400 );
		MinimumSize = Size;
		WindowTitle = "Tools Manager";

		SetWindowIcon( "hardware" );

		CreateUI();
		Show();
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
				toolsList.AddPage( config.Title, "hardware", new Page( project ) );
			}
		}

		var footer = toolsList.MenuBottom.AddRow();
		var button = footer.Add( new Button.Primary( "Add from GitHub...", "add" ) );
		button.Clicked = () => ToolUpdateNotice.Show( 4 );
	}
}
