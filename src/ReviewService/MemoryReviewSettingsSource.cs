using System.Threading;
using System.Threading.Tasks;

namespace ReviewService;

/// <summary>
/// In-memory implementation of <see cref="IReviewSettingsSource{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the persisted object.</typeparam>
public sealed class MemoryReviewSettingsSource<TReviewSettings> : IReviewSettingsSource<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	private TReviewSettings _reviewSettings = default;

	/// <inheritdoc/>
	public Task<TReviewSettings> Read(CancellationToken ct)
	{
		return Task.FromResult(_reviewSettings);
	}

	/// <inheritdoc/>
	public Task Write(CancellationToken ct, TReviewSettings reviewSettings)
	{
		_reviewSettings = reviewSettings;

		return Task.CompletedTask;
	}
}
