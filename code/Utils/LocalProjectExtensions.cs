using System.IO;

namespace Tools;

public static class LocalProjectExtensions
{
	public static string GetManifestPath( this Sandbox.LocalProject localProject )
	{
		return Path.Combine( localProject.GetRootPath(), "tm-manifest.json" );
	}
}
