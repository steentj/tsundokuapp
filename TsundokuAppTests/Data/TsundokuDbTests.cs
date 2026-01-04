using Tsundoku.Data;

namespace TsundokuAppTests.Data;

public class TsundokuDbTests
{
	[Fact]
	public async Task InitializeAsync_CreatesConnection()
	{
		// Arrange
		var db = new TsundokuDb();

		// Act
		await db.InitializeAsync();

		// Assert
		Assert.NotNull(db.Connection);
	}

	[Fact]
	public async Task InitializeAsync_WhenCalledMultipleTimes_InitializesOnlyOnce()
	{
		// Arrange
		var db = new TsundokuDb();

		// Act
		await db.InitializeAsync();
		var firstConnection = db.Connection;
		
		await db.InitializeAsync();
		var secondConnection = db.Connection;

		// Assert
		Assert.Same(firstConnection, secondConnection);
	}

	[Fact]
	public async Task InitializeAsync_IsConcurrencySafe()
	{
		// Arrange
		var db = new TsundokuDb();

		// Act
		var tasks = Enumerable.Range(0, 10)
			.Select(_ => Task.Run(async () => await db.InitializeAsync()))
			.ToArray();

		await Task.WhenAll(tasks);

		// Assert
		Assert.NotNull(db.Connection);
	}

	[Fact]
	public void Connection_BeforeInitialization_ThrowsInvalidOperationException()
	{
		// Arrange
		var db = new TsundokuDb();

		// Act & Assert
		Assert.Throws<InvalidOperationException>(() => db.Connection);
	}
}
