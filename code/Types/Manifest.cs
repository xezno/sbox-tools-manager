using System.Text.Json.Serialization;

namespace Tools;

public class Manifest
{
	[JsonPropertyName( "repo" )]
	public string Repo { get; set; }

	[JsonPropertyName( "version" )]
	public string Version { get; set; }

	[JsonPropertyName( "autoUpdate" )]
	public bool AutoUpdate { get; set; }
}
