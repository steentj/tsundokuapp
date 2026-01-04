using System.Globalization;
using Tsundoku.Converters;
using Tsundoku.Models;

namespace TsundokuAppTests.Converters;

public class BookStatusToLocalizedStringConverterTests
{
	private readonly BookStatusToLocalizedStringConverter _converter = new();

	[Fact]
	public void Convert_WithBoughtStatus_ReturnsLocalizedString()
	{
		// Arrange
		var status = BookStatus.Bought;

		// Act
		var result = _converter.Convert(status, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		// The actual string depends on the current culture, so we just verify it's not empty
		Assert.False(string.IsNullOrEmpty((string)result));
	}

	[Fact]
	public void Convert_WithStartedReadingStatus_ReturnsLocalizedString()
	{
		// Arrange
		var status = BookStatus.StartedReading;

		// Act
		var result = _converter.Convert(status, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		Assert.False(string.IsNullOrEmpty((string)result));
	}

	[Fact]
	public void Convert_WithReadStatus_ReturnsLocalizedString()
	{
		// Arrange
		var status = BookStatus.Read;

		// Act
		var result = _converter.Convert(status, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		Assert.False(string.IsNullOrEmpty((string)result));
	}

	[Fact]
	public void Convert_WithGivenUpStatus_ReturnsLocalizedString()
	{
		// Arrange
		var status = BookStatus.GivenUp;

		// Act
		var result = _converter.Convert(status, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		Assert.False(string.IsNullOrEmpty((string)result));
	}

	[Fact]
	public void Convert_WithNonBookStatusValue_ReturnsEmptyString()
	{
		// Arrange
		var value = "Not a BookStatus";

		// Act
		var result = _converter.Convert(value, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.Equal(string.Empty, result);
	}

	[Fact]
	public void Convert_WithNull_ReturnsEmptyString()
	{
		// Arrange
		object? value = null;

		// Act
		var result = _converter.Convert(value, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.Equal(string.Empty, result);
	}

	[Fact]
	public void ConvertBack_ThrowsNotSupportedException()
	{
		// Arrange, Act & Assert
		Assert.Throws<NotSupportedException>(() =>
			_converter.ConvertBack("Some string", typeof(BookStatus), null, CultureInfo.CurrentUICulture));
	}

	[Theory]
	[InlineData(BookStatus.Bought)]
	[InlineData(BookStatus.StartedReading)]
	[InlineData(BookStatus.Read)]
	[InlineData(BookStatus.GivenUp)]
	public void Convert_WithAllValidStatuses_ReturnsNonEmptyString(BookStatus status)
	{
		// Act
		var result = _converter.Convert(status, typeof(string), null, CultureInfo.CurrentUICulture);

		// Assert
		Assert.NotNull(result);
		Assert.IsType<string>(result);
		Assert.False(string.IsNullOrEmpty((string)result));
	}
}
