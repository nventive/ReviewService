using Plugin.StoreReview;

namespace ReviewService;

/// <summary>
/// Extensions on <see cref="ReviewStatus"/>.
/// </summary>
internal static class ReviewStatusExtensions
{
	/// <summary>
	/// Converts a <see cref="ReviewStatus"/> value to its corresponding <see cref="ReviewPromptStatus"/> value.
	/// </summary>
	/// <param name="status">The <see cref="ReviewStatus"/> value to convert.</param>
	/// <returns>A <see cref="ReviewPromptStatus"/> value that represents the result of the review operation.</returns>
	public static ReviewPromptStatus ToReviewPromptStatus(this ReviewStatus status)
	{
		return status switch
		{
			ReviewStatus.Succeeded => ReviewPromptStatus.Success,
			ReviewStatus.Error => ReviewPromptStatus.Failed,
			ReviewStatus.NetworkError => ReviewPromptStatus.Failed,
			ReviewStatus.CanceledByUser => ReviewPromptStatus.Canceled,
			_ => ReviewPromptStatus.Unknown,
		};
	}
}
