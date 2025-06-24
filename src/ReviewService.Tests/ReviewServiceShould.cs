using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ReviewService.Tests;

/// <summary>
/// Tests for <see cref="ReviewService{TReviewSettings}"/>.
/// </summary>
public sealed class ReviewServiceShould
{
	private readonly IReviewPrompter _reviewPrompterMock;

	public static IList<object[]> ReviewSettingsAndConditionsSatisfiedMapping =>
	[
		[
			new ReviewSettings(),
			false
		],
		[
			new ReviewSettings
			{
				FirstApplicationLaunch = DateTimeOffset.MinValue,
				ApplicationLaunchCount = int.MaxValue,
				PrimaryActionCompletedCount = int.MaxValue,
				RequestCount = 1,
				LastRequest = DateTimeOffset.Now
			},
			false
		],
		[
			new ReviewSettings
			{
				FirstApplicationLaunch = DateTimeOffset.MinValue,
				ApplicationLaunchCount = int.MaxValue,
				PrimaryActionCompletedCount = int.MaxValue
			},
			true
		]
	];

	/// <summary>
	/// Initializes a new instance of the <see cref="ReviewServiceShould"/> class.
	/// </summary>
	public ReviewServiceShould()
	{
		_reviewPrompterMock = Substitute.For<IReviewPrompter>();
	}

	[Theory, MemberData(nameof(ReviewSettingsAndConditionsSatisfiedMapping))]
	public async Task Prompt_Review_When_Conditions_Are_Satisfied(ReviewSettings reviewSettings, bool areConditionsSatisfied)
	{
		// Arrange
		var reviewConditionsBuilder = ReviewConditionsBuilder.Default();
		var reviewSettingsSource = new MemoryReviewSettingsSource<ReviewSettings>();

		await reviewSettingsSource.Write(CancellationToken.None, reviewSettings);

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Success);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		if (areConditionsSatisfied)
		{
			await _reviewPrompterMock.Received(1).TryPrompt();
			Assert.Equal(ReviewPromptStatus.Success, result.Status);
			Assert.True(result.IsSuccessful);
		}
		else
		{
			await _reviewPrompterMock.DidNotReceive().TryPrompt();
			Assert.Equal(ReviewPromptStatus.NotAttempted, result.Status);
			Assert.False(result.IsSuccessful);
		}
	}

	[Fact]
	public async Task Always_Satisfy_Conditions_When_Using_Empty_Conditions_Builder()
	{
		// Arrange
		var reviewConditionsBuilder = ReviewConditionsBuilder.Empty();
		var reviewSettingsSource = new MemoryReviewSettingsSource<ReviewSettings>();

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Success);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var areConditionsSatisfied = await reviewService.GetAreConditionsSatisfied(CancellationToken.None);
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		Assert.True(areConditionsSatisfied);
		await _reviewPrompterMock.Received(1).TryPrompt();
		Assert.Equal(ReviewPromptStatus.Success, result.Status);
		Assert.True(result.IsSuccessful);
	}

	[Fact]
	public async Task Handle_Custom_Review_Settings_With_Empty_Conditions()
	{
		// Arrange
		var customSettings = new CustomReviewSettings
		{
			HasCompletedOnboarding = true,
			PrimaryActionCompletedCount = 5
		};

		var reviewConditionsBuilder = ReviewConditionsBuilder.Empty<CustomReviewSettings>();
		var reviewSettingsSource = new MemoryReviewSettingsSource<CustomReviewSettings>();

		await reviewSettingsSource.Write(CancellationToken.None, customSettings);

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Success);

		var reviewService = new ReviewService<CustomReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var areConditionsSatisfied = await reviewService.GetAreConditionsSatisfied(CancellationToken.None);
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		Assert.True(areConditionsSatisfied);
		await _reviewPrompterMock.Received(1).TryPrompt();
		Assert.Equal(ReviewPromptStatus.Success, result.Status);
		Assert.True(result.IsSuccessful);
	}

	[Fact]
	public async Task Handle_Custom_Review_Settings_With_Default_Conditions()
	{
		// Arrange
		var customSettings = new CustomReviewSettings
		{
			HasCompletedOnboarding = true,
			FirstApplicationLaunch = DateTimeOffset.Now.AddDays(-10),
			ApplicationLaunchCount = 5,
			PrimaryActionCompletedCount = 3
		};

		var reviewConditionsBuilder = ReviewConditionsBuilder.Default<CustomReviewSettings>();
		var reviewSettingsSource = new MemoryReviewSettingsSource<CustomReviewSettings>();

		await reviewSettingsSource.Write(CancellationToken.None, customSettings);

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Success);

		var reviewService = new ReviewService<CustomReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var areConditionsSatisfied = await reviewService.GetAreConditionsSatisfied(CancellationToken.None);
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		Assert.True(areConditionsSatisfied);
		await _reviewPrompterMock.Received(1).TryPrompt();
		Assert.Equal(ReviewPromptStatus.Success, result.Status);
		Assert.True(result.IsSuccessful);
	}

	[Fact]
	public async Task Handle_Exception_In_UpdateReviewSettings()
	{
		// Arrange
		var loggerSubstitute = Substitute.For<ILogger<ReviewService<ReviewSettings>>>();
		var reviewSettingsSourceSubstitute = Substitute.For<IReviewSettingsSource<ReviewSettings>>();

		reviewSettingsSourceSubstitute.Write(Arg.Any<CancellationToken>(), Arg.Any<ReviewSettings>())
			.Throws(new InvalidOperationException("Write failed"));

		var reviewService = new ReviewService<ReviewSettings>(
			logger: loggerSubstitute,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSourceSubstitute,
			reviewConditionsBuilder: ReviewConditionsBuilder.Empty()
		);

		// Act
		await reviewService.UpdateReviewSettings(
			CancellationToken.None,
			settings => settings with { PrimaryActionCompletedCount = 1 }
		);

		// Assert
		loggerSubstitute.Received(1).Log(
			LogLevel.Error,
			Arg.Any<EventId>(),
			Arg.Is<object>(o => o.ToString()!.Contains("Failed to update review settings")),
			Arg.Any<Exception>(), Arg.Any<Func<object, Exception?, string>>()
		);
	}

	[Fact]
	public async Task Return_Correct_Status_When_Prompter_Returns_Error()
	{
		// Arrange
		var reviewConditionsBuilder = ReviewConditionsBuilder.Empty();
		var reviewSettingsSource = new MemoryReviewSettingsSource<ReviewSettings>();

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Failed);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		Assert.Equal(ReviewPromptStatus.Failed, result.Status);
		Assert.False(result.IsSuccessful);
	}

	[Fact]
	public async Task Return_Correct_Status_When_Prompter_Is_Canceled_By_User()
	{
		// Arrange
		var reviewConditionsBuilder = ReviewConditionsBuilder.Empty();
		var reviewSettingsSource = new MemoryReviewSettingsSource<ReviewSettings>();

		_reviewPrompterMock.TryPrompt().Returns(ReviewPromptStatus.Canceled);

		var reviewService = new ReviewService<ReviewSettings>(
			logger: null,
			reviewPrompter: _reviewPrompterMock,
			reviewSettingsSource: reviewSettingsSource,
			reviewConditionsBuilder: reviewConditionsBuilder
		);

		// Act
		var result = await reviewService.TryRequestReview(CancellationToken.None);

		// Assert
		Assert.Equal(ReviewPromptStatus.Canceled, result.Status);
		Assert.False(result.IsSuccessful);
	}
}
