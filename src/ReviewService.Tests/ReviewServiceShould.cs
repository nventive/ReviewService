using Moq;

namespace ReviewService.Tests;

/// <summary>
/// Tests for <see cref="ReviewService{TReviewSettings}"/>.
/// </summary>
public sealed class ReviewServiceShould
{
	private readonly Mock<IReviewPrompter> _reviewPrompterMock;

	public static IList<object[]> ReviewSettingsAndConditionsSatisfiedMapping =>
		new List<object[]>
		{
			new object[]
			{ 
				new ReviewSettings(),
				false
			},
			new object[]
			{
				new ReviewSettings
				{
					FirstApplicationLaunch = DateTimeOffset.MinValue,
					ApplicationLaunchCount = int.MaxValue,
					PrimaryActionCompletedCount = int.MaxValue,
					RequestCount = 1,
					LastRequest = DateTimeOffset.Now
				},
				false
			},
			new object[]
			{ 
				new ReviewSettings
				{
					FirstApplicationLaunch = DateTimeOffset.MinValue,
					ApplicationLaunchCount = int.MaxValue,
					PrimaryActionCompletedCount = int.MaxValue
				},
				true
			}
		};

	/// <summary>
	/// Initializes a new instance of the <see cref="ReviewServiceShould"/> class.
	/// </summary>
	public ReviewServiceShould()
	{
		_reviewPrompterMock = new Mock<IReviewPrompter>();
	}

	[Theory, MemberData(nameof(ReviewSettingsAndConditionsSatisfiedMapping))]
	public async Task Prompt_Review_When_Conditions_Are_Satisfied(ReviewSettings reviewSettings, bool areConditionsSatisfied)
	{
		// Arrange.
		var reviewConditionsBuilder = ReviewConditionsBuilder.Default();
		var reviewSettingsSource = new MemoryReviewSettingsSource<ReviewSettings>();

		await reviewSettingsSource.Write(CancellationToken.None, reviewSettings);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock.Object,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act.
		await reviewService.TryRequestReview(CancellationToken.None);

		// Assert.
		_reviewPrompterMock.Verify(x => x.TryPrompt(), areConditionsSatisfied ? Times.Once : Times.Never);
	}
}