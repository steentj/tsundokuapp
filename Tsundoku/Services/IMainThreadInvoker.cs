namespace Tsundoku.Services;

public interface IMainThreadInvoker
{
	void BeginInvokeOnMainThread(Action action);
}
