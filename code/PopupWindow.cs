using System;
using System.Collections.Generic;

namespace Tools;

public class PopupWindow : BaseWindow
{
	Label Label;

	public PopupWindow( string title, string text, string buttonTxt = "OK", IDictionary<string, Action> extraButtons = null )
	{
		//FixedWidth = 500;
		//FixedHeight = 150;

		SetWindowIcon( "info" );

		WindowTitle = title;

		AdjustSize();

		SetModal( true, true );

		SetLayout( LayoutMode.TopToBottom );
		Layout.Margin = 24;
		Layout.Spacing = 24;

		Label = new Label( this );
		Label.Text = text;
		Layout.Add( Label );

		var okButton = new Button( this );
		okButton.Text = buttonTxt;
		okButton.MinimumWidth = 64;
		okButton.MouseLeftPress += () => this.Destroy();
		okButton.AdjustSize();

		var buttonLayout = Layout.Row( true );
		buttonLayout.Spacing = 5;
		Layout.Add( buttonLayout );
		buttonLayout.Add( okButton );

		if ( extraButtons != null )
		{
			foreach ( var KVs in extraButtons )
			{
				var customBtn = new Button( this );
				customBtn.Text = KVs.Key;
				customBtn.MinimumWidth = 64;
				customBtn.MouseLeftPress += KVs.Value;
				customBtn.MouseLeftPress += () => this.Destroy();
				customBtn.AdjustSize();

				buttonLayout.Add( customBtn );
			}
		}

		buttonLayout.AddStretchCell();
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		Paint.ClearPen();
		Paint.SetBrush( Theme.WidgetBackground );
		Paint.DrawRect( LocalRect, 0.0f );
	}
}
