using System;
using System.Threading.Tasks;
using System.Threading;

namespace ReviewService;

/// <summary>
/// Extensions for <see cref="IReviewConditionsBuilder{TReviewSettings}"/>.
/// </summary>
public static partial class ReviewConditionBuilderExtensions
{
	/// <summary>
	/// The number of completed primary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumPrimaryActionsCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumActionCompleted)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.PrimaryActionCompletedCount >= minimumActionCompleted)
		);
		return builder;
	}

	/// <summary>
	/// The number of completed secondary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumSecondaryActionsCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumActionCompleted)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.SecondaryActionCompletedCount >= minimumActionCompleted)
		);
		return builder;
	}

	/// <summary>
	/// The number of times the application has been launched must be at least <paramref name="minimumCount"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="minimumCount">The minimum number of times the application has been launched.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumApplicationLaunchCount<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumCount)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.ApplicationLaunchCount >= minimumCount)
		);
		return builder;
	}

	/// <summary>
	/// The elapsed time since the first application launch must be at least <paramref name="minimumTimeElapsed"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="minimumTimeElapsed">The minimum time elapsed since the first application launch.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumElapsedTimeSinceApplicationFirstLaunch<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, TimeSpan minimumTimeElapsed)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.FirstApplicationLaunch.HasValue && reviewSettings.FirstApplicationLaunch.Value + minimumTimeElapsed <= currentDateTime)
		);
		return builder;
	}

	/// <summary>
	/// The time elapsed since the last review requested must be at least <paramref name="minimumTimeElapsed"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="minimumTimeElapsed">The minimum time elapsed since the last review requested.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumElapsedTimeSinceLastReviewRequest<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, TimeSpan minimumTimeElapsed)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => !reviewSettings.LastRequest.HasValue || reviewSettings.LastRequest.Value + minimumTimeElapsed <= currentDateTime)
		);
		return builder;
	}

	/// <summary>
	/// Adds a custom synchronous condition.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="condition">The condition.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> Custom<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, Func<TReviewSettings, DateTimeOffset, bool> condition)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(condition));
		return builder;
	}

	/// <summary>
	/// Adds a custom asynchronous condition.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <param name="condition">The condition.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> CustomAsync<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, Func<CancellationToken, TReviewSettings, DateTimeOffset, Task<bool>> condition)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new AsynchronousReviewCondition<TReviewSettings>(condition));
		return builder;
	}
}
