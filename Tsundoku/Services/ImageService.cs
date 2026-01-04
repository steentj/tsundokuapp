using SkiaSharp;
using System.Linq;

namespace Tsundoku.Services;

public sealed class ImageService(HttpClient httpClient, IAppPaths appPaths) : IImageService
{
	private const int MaxWidth = 320;
	private const int MaxHeight = 480;

	public async Task<string?> PickPhotoAsync(CancellationToken cancellationToken = default)
	{
		if (!MediaPicker.Default.IsCaptureSupported)
		{
			// Picking can still be supported even if capture is not.
		}

		var files = await MediaPicker.Default.PickPhotosAsync().ConfigureAwait(false);
		var file = files?.FirstOrDefault();
		return file is null ? null : await SaveResizedAsync(file, cancellationToken).ConfigureAwait(false);
	}

	public async Task<string?> PickImageFileAsync(CancellationToken cancellationToken = default)
	{
		var file = await FilePicker.Default.PickAsync(new PickOptions
		{
			FileTypes = FilePickerFileType.Images,
		}).ConfigureAwait(false);

		return file is null ? null : await SaveResizedAsync(file, cancellationToken).ConfigureAwait(false);
	}

	public async Task<string?> CapturePhotoAsync(CancellationToken cancellationToken = default)
	{
		if (!MediaPicker.Default.IsCaptureSupported)
			return null;

		var file = await MediaPicker.Default.CapturePhotoAsync().ConfigureAwait(false);
		return file is null ? null : await SaveResizedAsync(file, cancellationToken).ConfigureAwait(false);
	}

	public async Task<string?> DownloadAndStoreAsync(string imageUrl, CancellationToken cancellationToken = default)
	{
		using var response = await httpClient.GetAsync(imageUrl, cancellationToken).ConfigureAwait(false);
		if (!response.IsSuccessStatusCode)
			return null;

		await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
		return await SaveResizedAsync(stream, cancellationToken).ConfigureAwait(false);
	}

	private string EnsureImagesDir()
	{
		var dir = Path.Combine(appPaths.AppDataDirectory, "images");
		Directory.CreateDirectory(dir);
		return dir;
	}

	private string NewImagePath()
	{
		return Path.Combine(EnsureImagesDir(), $"{Guid.NewGuid():N}.jpg");
	}

	private async Task<string?> SaveResizedAsync(FileResult file, CancellationToken cancellationToken)
	{
		await using var stream = await file.OpenReadAsync().ConfigureAwait(false);
		return await SaveResizedAsync(stream, cancellationToken).ConfigureAwait(false);
	}

	private async Task<string?> SaveResizedAsync(Stream input, CancellationToken cancellationToken)
	{
		using var ms = new MemoryStream();
		await input.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
		var bytes = ms.ToArray();

		using var codec = SKCodec.Create(new SKMemoryStream(bytes));
		if (codec is null)
			return null;

		using var original = SKBitmap.Decode(codec);
		if (original is null)
			return null;

		var (targetW, targetH) = ComputeTargetSize(original.Width, original.Height);
		using var resized = original.Resize(new SKImageInfo(targetW, targetH), SKFilterQuality.High);
		if (resized is null)
			return null;

		using var image = SKImage.FromBitmap(resized);
		using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
		var outPath = NewImagePath();
		await File.WriteAllBytesAsync(outPath, data.ToArray(), cancellationToken).ConfigureAwait(false);
		return outPath;
	}

	private static (int w, int h) ComputeTargetSize(int srcW, int srcH)
	{
		if (srcW <= 0 || srcH <= 0)
			return (MaxWidth, MaxHeight);

		var scale = Math.Min((double)MaxWidth / srcW, (double)MaxHeight / srcH);
		if (scale >= 1.0)
			return (srcW, srcH);

		var w = Math.Max(1, (int)Math.Round(srcW * scale));
		var h = Math.Max(1, (int)Math.Round(srcH * scale));
		return (w, h);
	}
}
