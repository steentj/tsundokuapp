using SQLite;
using Tsundoku.Models;
using Tsundoku.Services;

namespace Tsundoku.Data;

public sealed class TsundokuDb(IAppPaths appPaths)
{
	private SQLiteAsyncConnection? _connection;
	private readonly SemaphoreSlim _initLock = new(1, 1);

	public SQLiteAsyncConnection Connection => _connection ?? throw new InvalidOperationException("Database not initialized");

	public async Task InitializeAsync()
	{
		if (_connection is not null)
			return;

		await _initLock.WaitAsync().ConfigureAwait(false);
		try
		{
			if (_connection is not null)
				return;

			var dbPath = Path.Combine(appPaths.AppDataDirectory, "tsundoku.db3");
			_connection = new SQLiteAsyncConnection(dbPath);
			await _connection.CreateTableAsync<Book>().ConfigureAwait(false);
		}
		finally
		{
			_initLock.Release();
		}
	}
}
