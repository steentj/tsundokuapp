using System.Globalization;
using Tsundoku.Converters;

namespace TsundokuAppTests.Converters;

public class NullToBoolConverterTests
{
	[Fact]
	public void Convert_WithNull_WhenInvertFalse_ReturnsTrue()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = false };
		object? value = null;

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.True((bool)result);
	}

	[Fact]
	public void Convert_WithNonNull_WhenInvertFalse_ReturnsFalse()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = false };
		var value = "Not null";

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithNull_WhenInvertTrue_ReturnsFalse()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = true };
		object? value = null;

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithNonNull_WhenInvertTrue_ReturnsTrue()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = true };
		var value = "Not null";

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.True((bool)result);
	}

	[Fact]
	public void Convert_WithObject_WhenInvertFalse_ReturnsFalse()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = false };
		var value = new object();

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void Convert_WithZero_WhenInvertFalse_ReturnsFalse()
	{
		// Arrange
		var converter = new NullToBoolConverter { Invert = false };
		var value = 0;

		// Act
		var result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

		// Assert
		Assert.False((bool)result);
	}

	[Fact]
	public void ConvertBack_ThrowsNotSupportedException()
	{
		// Arrange
		var converter = new NullToBoolConverter();

		// Act & Assert
		Assert.Throws<NotSupportedException>(() =>
			converter.ConvertBack(true, typeof(object), null, CultureInfo.InvariantCulture));
	}

	[Fact]
	public void Invert_DefaultsToFalse()
	{
		// Arrange & Act
		var converter = new NullToBoolConverter();

		// Assert
		Assert.False(converter.Invert);
	}

	[Fact]
	public void Invert_CanBeSet()
	{
		// Arrange
		var converter = new NullToBoolConverter();

		// Act
		converter.Invert = true;

		// Assert
		Assert.True(converter.Invert);
	}
}
