#if WINDOWS
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReviewService;

/// <summary>
/// Windows implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public class ReviewPrompter : IReviewPrompter
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="ReviewPrompter"/> class.
	/// </summary>
	/// <param name="logger">The service logger.</param>
	public ReviewPrompter(ILogger<ReviewPrompter> logger)
	{
		_logger = logger ?? NullLogger<ReviewPrompter>.Instance;
	}

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		_logger.LogWarning("Prompting for a review is not implemented on Windows.");

		return Task.CompletedTask;
	}
}
#endif
