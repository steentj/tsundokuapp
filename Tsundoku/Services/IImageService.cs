namespace Tsundoku.Services;

public interface IImageService
{
	Task<string?> PickPhotoAsync(CancellationToken cancellationToken = default);
	Task<string?> PickImageFileAsync(CancellationToken cancellationToken = default);
	Task<string?> CapturePhotoAsync(CancellationToken cancellationToken = default);
	Task<string?> DownloadAndStoreAsync(string imageUrl, CancellationToken cancellationToken = default);
}
