namespace Crackdown_Installer
{

	// modified from:
	// https://stackoverflow.com/questions/20661652/progress-bar-with-httpclient

	public class HttpClientDownloadWithProgress : IDisposable
	{
		private readonly HttpRequestMessage _sendMessage;
		private readonly string _destinationFilePath;

		private HttpClient _httpClient;

		public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

		public event ProgressChangedHandler? ProgressChanged;

		public HttpClientDownloadWithProgress(HttpClient client, HttpRequestMessage sendMessage, string destinationFilePath)
		{
			_httpClient = client;
			_sendMessage = sendMessage;
			_destinationFilePath = destinationFilePath;
		}

		public async Task StartDownload()
		{
			using (var response = await _httpClient.SendAsync(_sendMessage, HttpCompletionOption.ResponseHeadersRead))
				await DownloadFileFromHttpResponseMessage(response);
		}

		private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
		{
			response.EnsureSuccessStatusCode();

			var totalBytes = response.Content.Headers.ContentLength;

			using (var contentStream = await response.Content.ReadAsStreamAsync())
				await ProcessContentStream(totalBytes, contentStream);
		}

		private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
		{
			var totalBytesRead = 0L;
			var readCount = 0L;
			var buffer = new byte[8192];
			var isMoreToRead = true;

			using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
			{
				do
				{
					var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
					if (bytesRead == 0)
					{
						isMoreToRead = false;
						TriggerProgressChanged(totalDownloadSize, totalBytesRead);
						continue;
					}

					await fileStream.WriteAsync(buffer, 0, bytesRead);

					totalBytesRead += bytesRead;
					readCount += 1;

					if (readCount % 100 == 0)
						TriggerProgressChanged(totalDownloadSize, totalBytesRead);
				}
				while (isMoreToRead);
			}
		}

		private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
		{
			if (ProgressChanged == null)
				return;

			double? progressPercentage = null;
			if (totalDownloadSize.HasValue)
				progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

			ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
		}

		public void Dispose()
		{
			//_httpClient?.Dispose();
		}
	}
}