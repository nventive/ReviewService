using Moq;
using ReviewService.Abstractions;

namespace ReviewService.Tests;

/// <summary>
/// Tests for <see cref="ReviewService{TReviewSettings}"/>.
/// </summary>
public sealed class ReviewServiceShould
{
	private readonly Mock<IReviewPrompter> _reviewPrompterMock;
	private readonly Mock<IReviewSettingsSource<ReviewSettings>> _reviewSettingsSourceMock;

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
					ApplicationFirstLaunched = DateTimeOffset.MinValue,
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
		_reviewSettingsSourceMock = new Mock<IReviewSettingsSource<ReviewSettings>>();
	}

	[Theory, MemberData(nameof(ReviewSettingsAndConditionsSatisfiedMapping))]
	public async Task Prompt_Review_When_Conditions_Are_Satisfied(ReviewSettings reviewSettings, bool areConditionsSatisfied)
	{
		// Arrange.
		var reviewConditionsBuilder = ReviewConditionsBuilder.Default();

		_reviewSettingsSourceMock.Setup(x => x.Read(It.IsAny<CancellationToken>())).ReturnsAsync(reviewSettings);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock.Object,
			reviewSettingsSource: _reviewSettingsSourceMock.Object,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act.
		await reviewService.TryRequestReview(ct: CancellationToken.None);

		// Assert.
		_reviewPrompterMock.Verify(x => x.TryPrompt(), areConditionsSatisfied ? Times.Once : Times.Never);
	}
}