using Tsundoku.Models;

namespace TsundokuAppTests.Models;

public class BookStatusTests
{
	[Fact]
	public void BookStatus_Bought_HasExpectedValue()
	{
		// Arrange & Act
		var status = BookStatus.Bought;

		// Assert
		Assert.Equal(0, (int)status);
	}

	[Fact]
	public void BookStatus_StartedReading_HasExpectedValue()
	{
		// Arrange & Act
		var status = BookStatus.StartedReading;

		// Assert
		Assert.Equal(1, (int)status);
	}

	[Fact]
	public void BookStatus_Read_HasExpectedValue()
	{
		// Arrange & Act
		var status = BookStatus.Read;

		// Assert
		Assert.Equal(2, (int)status);
	}

	[Fact]
	public void BookStatus_GivenUp_HasExpectedValue()
	{
		// Arrange & Act
		var status = BookStatus.GivenUp;

		// Assert
		Assert.Equal(3, (int)status);
	}

	[Fact]
	public void BookStatus_AllValuesAreDefined()
	{
		// Arrange
		var expectedValues = new[] { BookStatus.Bought, BookStatus.StartedReading, BookStatus.Read, BookStatus.GivenUp };

		// Act
		var actualValues = Enum.GetValues<BookStatus>();

		// Assert
		Assert.Equal(expectedValues.Length, actualValues.Length);
		foreach (var expected in expectedValues)
		{
			Assert.Contains(expected, actualValues);
		}
	}

	[Theory]
	[InlineData(BookStatus.Bought)]
	[InlineData(BookStatus.StartedReading)]
	[InlineData(BookStatus.Read)]
	[InlineData(BookStatus.GivenUp)]
	public void BookStatus_CanConvertToInt(BookStatus status)
	{
		// Act
		var intValue = (int)status;

		// Assert
		Assert.True(intValue >= 0 && intValue <= 3);
	}

	[Theory]
	[InlineData(0, BookStatus.Bought)]
	[InlineData(1, BookStatus.StartedReading)]
	[InlineData(2, BookStatus.Read)]
	[InlineData(3, BookStatus.GivenUp)]
	public void BookStatus_CanConvertFromInt(int intValue, BookStatus expectedStatus)
	{
		// Act
		var status = (BookStatus)intValue;

		// Assert
		Assert.Equal(expectedStatus, status);
	}
}
