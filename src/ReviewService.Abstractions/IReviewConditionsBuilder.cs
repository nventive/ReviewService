using System.Collections.Generic;

namespace ReviewService;

/// <summary>
/// Provide a way to gather the review conditions.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public interface IReviewConditionsBuilder<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	/// <summary>
	/// Gets the review conditions.
	/// </summary>
	IList<IReviewCondition<TReviewSettings>> Conditions { get; }
}
