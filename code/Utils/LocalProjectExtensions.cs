using System.Text.Json;

namespace Editor;

public static class LocalProjectExtensions
{
	/// <summary>
	/// Get the Tools Manager manifest path for this project
	/// </summary>
	public static string GetManifestPath( this Sandbox.LocalProject localProject )
	{
		return Path.Combine( localProject.GetRootPath(), "tm-manifest.json" );
	}

	/// <summary>
	/// Get the Tools Manager manifest for this project
	/// </summary>
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
