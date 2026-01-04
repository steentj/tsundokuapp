using System.Net;
using Moq;
using Moq.Protected;
using Tsundoku.Services;

namespace TsundokuAppTests.Services;

public class ImageServiceTests
{
	[Fact]
	public async Task DownloadAndStoreAsync_WithValidUrl_ReturnsFilePath()
	{
		// Arrange
		var imageUrl = "https://example.com/image.jpg";
		
		// Create a simple 1x1 JPEG image in memory
		var jpegBytes = CreateMinimalJpegImage();
		
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ByteArrayContent(jpegBytes)
			});

		var httpClient = new HttpClient(mockHandler.Object);
		var service = new ImageService(httpClient);

		// Act
		var result = await service.DownloadAndStoreAsync(imageUrl);

		// Assert
		Assert.NotNull(result);
		Assert.True(File.Exists(result));
		Assert.EndsWith(".jpg", result);
		
		// Cleanup
		if (result != null && File.Exists(result))
		{
			File.Delete(result);
		}
	}

	[Fact]
	public async Task DownloadAndStoreAsync_WithFailedRequest_ReturnsNull()
	{
		// Arrange
		var imageUrl = "https://example.com/nonexistent.jpg";
		
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.NotFound
			});

		var httpClient = new HttpClient(mockHandler.Object);
		var service = new ImageService(httpClient);

		// Act
		var result = await service.DownloadAndStoreAsync(imageUrl);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task DownloadAndStoreAsync_WithCancellationToken_PassesToken()
	{
		// Arrange
		var imageUrl = "https://example.com/image.jpg";
		var cts = new CancellationTokenSource();
		var jpegBytes = CreateMinimalJpegImage();
		
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.Is<CancellationToken>(ct => ct == cts.Token)
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new ByteArrayContent(jpegBytes)
			});

		var httpClient = new HttpClient(mockHandler.Object);
		var service = new ImageService(httpClient);

		// Act
		var result = await service.DownloadAndStoreAsync(imageUrl, cts.Token);

		// Assert
		mockHandler.Protected().Verify(
			"SendAsync",
			Times.Once(),
			ItExpr.IsAny<HttpRequestMessage>(),
			ItExpr.Is<CancellationToken>(ct => ct == cts.Token)
		);
		
		// Cleanup
		if (result != null && File.Exists(result))
		{
			File.Delete(result);
		}
	}

	[Fact]
	public async Task DownloadAndStoreAsync_WithInternalServerError_ReturnsNull()
	{
		// Arrange
		var imageUrl = "https://example.com/error.jpg";
		
		var mockHandler = new Mock<HttpMessageHandler>();
		mockHandler.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.InternalServerError
			});

		var httpClient = new HttpClient(mockHandler.Object);
		var service = new ImageService(httpClient);

		// Act
		var result = await service.DownloadAndStoreAsync(imageUrl);

		// Assert
		Assert.Null(result);
	}

	/// <summary>
	/// Creates a minimal valid JPEG image (1x1 red pixel) for testing purposes.
	/// This is a real JPEG that can be processed by SkiaSharp.
	/// </summary>
	private static byte[] CreateMinimalJpegImage()
	{
		// This is a valid minimal 1x1 red pixel JPEG image
		return new byte[]
		{
			0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x01, 0x00, 0x48,
			0x00, 0x48, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00,
			0x01, 0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xC4, 0x00,
			0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01, 0x00, 0x00, 0x3F, 0x00, 0x7F, 0xFF, 0xD9
		};
	}
}
