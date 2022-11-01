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
		footer.Spacing = 4;

		var add = footer.Add( new Button.Primary( "Add Tool...", "add" ), 1 );
		add.Clicked = () => AddToolDialog.Open();

		var update = footer.Add( new Button( null, "download" ) );
		update.Clicked = () => ToolUpdateNotice.Open( 4 );
	}
}
