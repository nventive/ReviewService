namespace ReviewService;

/// <summary>
/// The status of a review prompt attempt.
/// </summary>
public enum ReviewPromptStatus
{
	/// <summary>
	/// The review prompt was successfully shown to the user, and the user has completed the review process.
	/// </summary>
	Success,

	/// <summary>
	/// The review prompt was shown to the user, but the user did not complete the review process.
	/// </summary>
	Canceled,

	/// <summary>
	/// The review prompt was attempted but failed, typically due to an exception or an error in the prompting process.
	/// </summary>
	Failed,

	/// <summary>
	/// The review prompt was not attempted because the conditions were not satisfied.
	/// </summary>
	NotAttempted,

	/// <summary>
	/// The review prompt status is unknown, typically used when the platform does not support prompting or the implementation is not available.
	/// </summary>
	Unknown,
}
