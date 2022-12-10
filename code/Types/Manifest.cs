using System.Text.Json;
using System.Text.Json.Serialization;

namespace Editor;

/// <summary>
/// Contains information about a GitHub repository and the currently downloaded
/// release, allowing these to be linked to (and contained within) a tools
/// project.
/// </summary>
public class Manifest
{
	[JsonPropertyName( "repo" )]
	public string Repo { get; set; }

	[JsonPropertyName( "description" )]
	public string Description { get; set; }

	[JsonPropertyName( "release_version" )]
	public string ReleaseVersion { get; set; }

	[JsonPropertyName( "release_name" )]
	public string ReleaseName { get; set; }

	[JsonPropertyName( "release_description" )]
	public string ReleaseDescription { get; set; }

	[JsonPropertyName( "auto_update" )]
	public bool AutoUpdate { get; set; } = true;

	public Manifest()
	{
	}

	public Manifest( Release release, Repository repo )
	{
		SetRelease( release );
		SetRepo( repo );
	}

	public void SetRelease( Release release )
	{
		ReleaseVersion = release.TagName;
		ReleaseName = release.Name;
		ReleaseDescription = release.Body;
	}

	public void SetRepo( Repository repo )
	{
		Repo = repo.FullName;
		Description = repo.Description;
	}

	public void WriteToFile( string path )
	{
		File.WriteAllText( path, ToJson() );
	}

	public void WriteToFolder( string folder )
	{
		var path = Path.Combine( folder, "tm-manifest.json" );
		WriteToFile( path );
	}

	public string ToJson()
	{
		return JsonSerializer.Serialize( this );
	}

	public bool CheckUpdateAvailable( Release latestRelease )
	{
		return latestRelease.TagName != ReleaseVersion;
	}

	public bool CheckUpdateAvailable()
	{
		var latestRelease = GithubApi.FetchLatestRelease( Repo ).Result;
		return CheckUpdateAvailable( latestRelease );
	}
}
