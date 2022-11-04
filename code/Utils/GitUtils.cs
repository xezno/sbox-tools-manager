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
		process.Start();

		await process.WaitForExitAsync();
	}
}
