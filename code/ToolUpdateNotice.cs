namespace Tools;
internal class ToolUpdateNotice : NoticeWidget
{
	private int Count;

	private ToolUpdateNotice()
	{
		Icon = "download";
		Position = 10;
	}

	/// <summary>
	/// Called when it's about to be re-used by a new compiler
	/// </summary>
	public override void Reset()
	{
		base.Reset();

		SetBodyWidget( null );
		FixedWidth = 320;
		FixedHeight = 76;
		Title = $"Tool Update";
		BorderColor = Theme.Yellow;
		Visible = true;
		IsRunning = false;
		Subtitle = $"{Count} tools have updates.";

		NoticeManager.Remove( this, 10 );
	}

	public static void Open( int updateCount )
	{
		var notice = new ToolUpdateNotice();
		notice.Count = updateCount;
		notice.Reset();
	}
}
