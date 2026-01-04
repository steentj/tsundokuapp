using System.Globalization;
using Tsundoku.Converters;

namespace TsundokuAppTests.Converters;

public class StringNotNullOrEmptyToBoolConverterTests
{
	private readonly StringNotNullOrEmptyToBoolConverter _converter = new();

	[Fact]
	public void Convert_WithNonEmptyString_ReturnsTrue()
	{
		// Arrange
		var value = "Test String";

		// Act
		var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.True((bool)result);
	}

	[Fact]
	public void Convert_WithEmptyString_ReturnsFalse()
	{
		// Arrange
		var value = "";

		// Act
		var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithWhitespaceString_ReturnsFalse()
	{
		// Arrange
		var value = "   ";

		// Act
		var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithNull_ReturnsFalse()
	{
		// Arrange
		object? value = null;

		// Act
		var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithNonStringValue_ReturnsFalse()
	{
		// Arrange
		var value = 123;

		// Act
		var result = _converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void ConvertBack_ThrowsNotSupportedException()
	{
		// Arrange, Act & Assert
		Assert.Throws<NotSupportedException>(() =>
			_converter.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture));
	}
}
