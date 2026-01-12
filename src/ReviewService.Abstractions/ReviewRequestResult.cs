namespace ReviewService;

/// <summary>
/// Represents the result of a review request operation.
/// </summary>
public sealed class ReviewRequestResult
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReviewRequestResult"/> class.
	/// </summary>
	/// <param name="status">The status of the review request.</param>
	public ReviewRequestResult(ReviewPromptStatus status)
	{
		Status = status;
	}

	/// <summary>
	/// Gets the status of the review request.
	/// </summary>
	public ReviewPromptStatus Status { get; }

	/// <summary>
	/// Gets a value indicating whether the review request was successful.
	/// </summary>
	public bool IsSuccessful => Status is ReviewPromptStatus.Success;
}
