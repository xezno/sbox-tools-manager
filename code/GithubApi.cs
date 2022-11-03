using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tools;

public static class GithubApi
{
	private static HttpClient httpClient;
	private static RealTimeUntil TimeUntilRatelimitReset;

	private static Dictionary<string, string> Cache = new();

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

	private static string GetHeader( this HttpResponseMessage message, string header )
	{
		if ( message.Headers.TryGetValues( header, out var values ) )
		{
			var resetTime = values.FirstOrDefault();
			return resetTime;
		}

		return null;
	}

	private static async Task<string> FetchText( string url )
	{
		if ( TimeUntilRatelimitReset > 0 )
		{
			Log.Trace( $"Still need to wait {TimeUntilRatelimitReset}s until rate limit resets.." );
			return "{}";
		}

		if ( Cache.TryGetValue( url, out var cachedResponse ) )
		{
			return cachedResponse;
		}

		var content = await HttpClient.GetAsync( url );
		var result = await content.Content.ReadAsStringAsync();

		var resetTime = int.Parse( content.GetHeader( "x-ratelimit-reset" ) );
		var limit = int.Parse( content.GetHeader( "x-ratelimit-limit" ) );
		var remaining = int.Parse( content.GetHeader( "x-ratelimit-remaining" ) );
		var used = int.Parse( content.GetHeader( "x-ratelimit-used" ) );

		Log.Trace( $"Rate limit: reset time: {resetTime}; limit: {limit}; remaining: {remaining}; used: {used}" );

		if ( remaining <= 0 )
		{
			var timestamp = DateTime.UnixEpoch.AddSeconds( resetTime );
			TimeUntilRatelimitReset = (float)(timestamp - DateTime.Now).TotalSeconds;
		}
		else
		{
			Cache[url] = result;
		}

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
