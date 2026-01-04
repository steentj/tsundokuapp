namespace Tsundoku.Services;

public sealed class MauiMainThreadInvoker : IMainThreadInvoker
{
	public void BeginInvokeOnMainThread(Action action)
		=> MainThread.BeginInvokeOnMainThread(action);
}
