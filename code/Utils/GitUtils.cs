using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tools;

public static class GitUtils
{
	/// <summary>
	/// Perform a git command and wait for it to finish
	/// </summary>
	public static async Task Git( string command, string workingDir = null )
	{
		Log.Trace( $"git {command}" );

		ProcessStartInfo info = new( "git", command );
		info.UseShellExecute = false;
		info.CreateNoWindow = true;

		if ( workingDir != null )
			info.WorkingDirectory = workingDir;

		var process = new Process();
		process.StartInfo = info;

		try
		{
			process.Start();
		}
		catch ( Exception e )
		{
			var test = new PopupWindow( "Error executing git command!",
				$"Failed to initialize git, do you have it installed?\n\nThe error was:\n{e.Message}", "OK",
				new Dictionary<string, Action>() { { "Open download link", () => Utility.OpenFolder( "https://git-scm.com/" ) } } );
			test.Show();
			return;
		}

		await process.WaitForExitAsync();
	}
}
