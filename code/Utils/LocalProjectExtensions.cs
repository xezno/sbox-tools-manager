using System.IO;
using System.Text.Json;

namespace Tools;

public static class LocalProjectExtensions
{
	public static string GetManifestPath( this Sandbox.LocalProject localProject )
	{
		return Path.Combine( localProject.GetRootPath(), "tm-manifest.json" );
	}

	public static Manifest GetManifest( this Sandbox.LocalProject localProject )
	{
		if ( localProject.Config.PackageType != Sandbox.Package.Type.Tool )
			return null;

		if ( !File.Exists( localProject.GetManifestPath() ) )
			return null;

		var manifestStr = File.ReadAllText( localProject.GetManifestPath() );
		var manifest = JsonSerializer.Deserialize<Manifest>( manifestStr );

		return manifest;
	}
}
