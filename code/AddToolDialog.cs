using Sandbox;

namespace Tools;

public class AddToolDialog : Dialog
{
	public static void Open()
	{
		_ = new AddToolDialog( null );
	}

	public AddToolDialog( Widget parent ) : base( null )
	{
		Window.Title = "Add Tool From GitHub";
		Window.SetWindowIcon( "add_box" );
		Window.Size = new Vector2( 500, 630 );
		Window.MaximumSize = Size;

		CreateUI();
		Show();
	}

	public void CreateUI()
	{
		SetLayout( LayoutMode.TopToBottom );
		Layout.Spacing = 0;
		Layout.Margin = 0;


		// Filtering
		{
			var filter = Layout.Add( LayoutMode.TopToBottom );
			filter.Margin = new( 20, 20, 20, 0 );

			filter.Add( new LineEdit( "" ) { PlaceholderText = "Search GitHub..." } );
		}

		// body
		{
			Layout.AddSpacingCell( 8 );
			var repoList = Layout.Add( new ListView(), 1 );
			repoList.ItemPaint = PaintAddonItem;
			repoList.ItemSize = new Vector2( 0, 38 );
			Layout.AddSpacingCell( 8 );

			GithubApi.FetchSearch( "topic:sbox-tool" ).ContinueWith( t =>
			{
				var searchResults = t.Result;

				searchResults.Items.ForEach( x => repoList.AddItem( x ) );
			} );
		}

		Layout.AddSeparator();

		// Output
		{
			var lo = Layout.Add( LayoutMode.TopToBottom, 0 );
			lo.Margin = 20;
			lo.Spacing = 4;

			lo.Add( new Label( "Check-out Location" ) );
			var Location = lo.Add( new FolderProperty( null ) );
			Location.Text = EditorPreferences.AddonLocation.NormalizeFilename( false );
			Location.ToolTip = "This is where the addon will be downloaded.\n The folder will be created if it doesn't exist.";
		}

		Layout.AddSeparator();

		// Footer buttons
		{
			var lo = Layout.Add( LayoutMode.LeftToRight );
			lo.Margin = 20;
			lo.Spacing = 4;

			lo.AddStretchCell();
			var OkayButton = lo.Add( new Button.Primary( "OK" ) { Clicked = DownloadTool } );
			lo.Add( new Button( "Cancel" ) { Clicked = Window.Close } );
		}
	}

	private void DownloadTool()
	{

	}

	private void PaintAddonItem( VirtualWidget v )
	{
		var rect = v.Rect;
		rect = rect.Shrink( 16, 0, 8, 0 );

		if ( v.Object is not Item repo )
			return;

		Paint.Antialiasing = true;

		Color fg = Theme.White.Darken( 0.2f );

		if ( Paint.HasSelected )
		{
			fg = Theme.White;
			Paint.ClearPen();
			Paint.SetBrush( Theme.Primary.WithAlpha( 0.9f ) );
			Paint.DrawRect( rect, 2 );

			Paint.SetPen( Theme.White );
		}
		else
		{
			Paint.SetDefaultFont();
			Paint.SetPen( Theme.Grey );
		}

		var textRect = rect.Shrink( 4 );

		Paint.SetFont( "Poppins", 10, 450 );
		Paint.SetPen( fg );
		Paint.DrawText( textRect, repo.Name, TextFlag.LeftTop );

		Paint.SetDefaultFont();
		Paint.SetPen( fg.WithAlpha( 0.6f ) );
		Paint.DrawText( textRect, repo.HtmlUrl, TextFlag.LeftBottom );
	}
}
