public static class WidgetExtensions
{
	public static void SetStylesheet( this Widget widget, string path )
	{
		var basePath = EditorUtility.Projects.GetAll().FirstOrDefault( x => x.Config.Ident == "tools_manager" ).GetCodePath();
		var combinedPath = basePath + path;

		void ReadAndSet()
		{
			var stylesheetText = File.ReadAllText( combinedPath );
			widget.SetStyles( stylesheetText );
		}

		ReadAndSet();

		//var watcher = new FileSystemWatcher();
		//watcher.Path = Path.GetDirectoryName( combinedPath );
		//watcher.Filter = Path.GetFileName( combinedPath );
		//watcher.Changed += (_, _) => ReadAndSet();
		//watcher.EnableRaisingEvents = true;
	}
}
