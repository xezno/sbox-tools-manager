using Sandbox;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tools;

public static class GithubApi
{
	private static HttpClient httpClient;

	private static HttpClient HttpClient
	{
		get
		{
			if ( httpClient == null )
			{
				httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.UserAgent.ParseAdd( "s&box" );
			}

			return httpClient;
		}
	}

	private static async Task<string> FetchText( string url )
	{
		var content = await HttpClient.GetAsync( url );
		var result = await content.Content.ReadAsStringAsync();

		Log.Trace( $"{url} query: {result}" );

		return result;
	}

	/// <summary>
	/// standard search query format
	/// </summary>
	public static async Task<Search> FetchSearch( string searchQuery )
	{
		var result = await FetchText( $"https://api.github.com/search/repositories?q={searchQuery}" );
		return Json.Deserialize<Search>( result );
	}

	/// <summary>
	/// in format "author/repo"
	/// </summary>
	public static async Task<Release> FetchLatestRelease( string repository )
	{
		var result = await FetchText( $"https://api.github.com/repos/{repository}/releases/latest" );
		return Json.Deserialize<Release>( result );
	}
}
