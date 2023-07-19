using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReviewService;

/// <summary>
/// Implementation of <see cref="IReviewService{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public sealed class ReviewService<TReviewSettings> : IReviewService<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	private readonly ILogger _logger;
	private readonly IReviewPrompter _reviewPrompter;
	private readonly IReviewSettingsSource<TReviewSettings> _reviewSettingsSource;
	private readonly IList<IReviewCondition<TReviewSettings>> _reviewConditions;

	/// <summary>
	/// Initializes a new instance of the <see cref="ReviewService{TReviewSettings}"/> class.
	/// </summary>
	/// <param name="logger">The service logger.</param>
	/// <param name="reviewPrompter">The native review prompter.</param>
	/// <param name="reviewSettingsSource">The review settings source (Read and write).</param>
	/// <param name="reviewConditionsBuilder">The review conditions builder.</param>
	public ReviewService(
		ILogger<ReviewService<TReviewSettings>>? logger,
		IReviewPrompter reviewPrompter,
		IReviewSettingsSource<TReviewSettings> reviewSettingsSource,
		IReviewConditionsBuilder<TReviewSettings>? reviewConditionsBuilder
	)
	{
		_logger = logger ?? NullLogger<ReviewService<TReviewSettings>>.Instance;
		_reviewPrompter = reviewPrompter;
		_reviewSettingsSource = reviewSettingsSource;
		_reviewConditions = reviewConditionsBuilder?.Conditions ?? ReviewConditionsBuilder.Default<TReviewSettings>().Conditions;
	}

	/// <summary>
	/// Tracks that a review was requested.
	/// </summary>
	/// <param name="ct">The cancellation token.</param>
	/// <returns><see cref="Task"/>.</returns>
	private async Task TrackReviewRequested(CancellationToken ct)
	{
		await UpdateReviewSettings(ct, reviewSettings =>
		{
			return reviewSettings with
			{
				LastRequest = DateTimeOffset.Now,
				RequestCount = reviewSettings.RequestCount + 1
			};
		});
	}

	/// <inheritdoc/>
	public async Task TryRequestReview(CancellationToken ct)
	{
		_logger.LogDebug("Trying to request a review.");

		if (await GetAreConditionsSatisfied(ct))
		{
			await _reviewPrompter.TryPrompt();
			await TrackReviewRequested(ct);

			_logger.LogInformation("Review requested.");
		}
		else
		{
			_logger.LogInformation("Did not request a review because one or more conditions were not satisfied.");
		}
	}

	/// <inheritdoc/>
	public async Task<bool> GetAreConditionsSatisfied(CancellationToken ct)
	{
		_logger.LogDebug("Evaluating conditions.");

		var currentSettings = await _reviewSettingsSource.Read(ct);
		var reviewConditionTasks = _reviewConditions.Select(async condition => await condition.Validate(ct, currentSettings, DateTimeOffset.Now));
		var result = (await Task.WhenAll(reviewConditionTasks)).All(x => x is true);

		if (result)
		{
			_logger.LogInformation("Evaluated conditions and all conditions are satisfied.");
		}
		else
		{
			_logger.LogInformation("Evaluated conditions and one or more conditions were not satisfied.");
		}

		return result;
	}

	/// <inheritdoc/>
	public async Task UpdateReviewSettings(CancellationToken ct, Func<TReviewSettings, TReviewSettings> updateFunction)
	{
		_logger.LogDebug("Updating review settings.");

		var currentSettings = await _reviewSettingsSource.Read(ct);

		try
		{
			await _reviewSettingsSource.Write(ct, updateFunction(currentSettings));

			_logger.LogInformation("Updated review settings.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to update review settings.");
		}
	}
}
