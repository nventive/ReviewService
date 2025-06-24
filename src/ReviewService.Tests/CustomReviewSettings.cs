namespace ReviewService.Tests;

/// <summary>
/// Custom review settings for testing purposes.
/// </summary>
public sealed record CustomReviewSettings : ReviewSettings
{
    /// <summary>
    /// Gets or sets if the application onboarding has been completed.
    /// </summary>
    public bool HasCompletedOnboarding { get; init; }
}
