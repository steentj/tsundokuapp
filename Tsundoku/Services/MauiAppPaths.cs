namespace Tsundoku.Services;

public sealed class MauiAppPaths : IAppPaths
{
	public string AppDataDirectory => FileSystem.AppDataDirectory;
}
