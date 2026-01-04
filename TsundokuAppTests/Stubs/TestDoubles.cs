using System.Runtime.CompilerServices;
using Tsundoku.Services;

namespace TsundokuAppTests.Stubs;

internal sealed class TestAppPaths(string appDataDirectory) : IAppPaths
{
	public string AppDataDirectory { get; } = appDataDirectory;
}

internal sealed class ImmediateMainThreadInvoker : IMainThreadInvoker
{
	public void BeginInvokeOnMainThread(Action action) => action();
}

internal static class TestBootstrapper
{
	[ModuleInitializer]
	internal static void Initialize()
	{
		// sqlite-net-pcl depends on SQLitePCLRaw initialization.
		SQLitePCL.Batteries_V2.Init();
	}
}
