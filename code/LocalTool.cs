using Sandbox;
using System.Threading.Tasks;

namespace Tools;

public class LocalTool
{
	public Release CurrentRelease { get; set; }
	public Release LatestRelease { get; set; }
	public Repository Repository { get; set; }

	private LocalProject Project { get; set; }
	private Manifest Manifest { get; set; }

	public string Title => Project.Config.Title;
	public string Description => Manifest.Description;

	public string Url => $"https://github.com/{Manifest.Repo}/";
	public string RootPath => Project.GetRootPath();

	public LocalTool( LocalProject project )
	{
		if ( project.Config.PackageType != Package.Type.Tool )
		{
			Log.Warning( "Tried to create a LocalTool from a non-tools project" );
			return;
		}

		Project = project;
		Manifest = GetManifest();

		CurrentRelease = GetCurrentRelease();
		LatestRelease = GetLatestRelease();
		Repository = GetRepository();
	}

	public bool IsValid()
	{
		return Manifest != null;
	}

	public bool NeedsUpdate()
	{
		return false;
	}

	public bool Update()
	{
		if ( !NeedsUpdate() )
			return false;

		var folder = Project.GetRootPath();
		using var progress = Progress.Start( $"Updating {Project.Config.Title}" );

		Progress.Update( "Removing local changes", 5, 100 );
		_ = GitUtils.Git( $"reset --hard HEAD" );
		_ = Task.Delay( 50 );


		Progress.Update( "Pulling remote changes", 25, 100 );
		_ = GitUtils.Git( $"pull" );
		_ = Task.Delay( 50 );

		Progress.Update( "Checking out release...", 90, 100 );
		_ = GitUtils.Git( $"checkout \"{LatestRelease.TagName}\" --force", folder );
		_ = Task.Delay( 50 );

		// Update Manifest
		Manifest.SetRelease( LatestRelease );
		WriteManifest();

		return true;
	}

	private Release GetCurrentRelease()
	{
		// TODO: Make it so manifest holds all release info
		return new Release()
		{
			TagName = Manifest.ReleaseVersion,
			Name = Manifest.ReleaseName,
			Body = Manifest.ReleaseDescription
		};
	}

	private Release GetLatestRelease()
	{
		// TODO: async
		var release = GithubApi.FetchLatestRelease( $"{Manifest.Repo}" ).Result;
		return release;
	}

	private Repository GetRepository()
	{
		return new();
	}

	private Manifest GetManifest()
	{
		return Project.GetManifest();
	}

	public void SetRelease( Release release )
	{
		// TODO
	}

	public void WriteManifest()
	{
		var folder = Project.GetRootPath();
		Manifest.WriteToFolder( folder );
	}
}
