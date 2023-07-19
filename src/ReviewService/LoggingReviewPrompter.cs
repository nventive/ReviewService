using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReviewService;

/// <summary>
/// Implementation of <see cref="IReviewPrompter"/> that only logs the <see cref="TryPrompt"/> invocation.
/// </summary>
public sealed class LoggingReviewPrompter : IReviewPrompter
{
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="LoggingReviewPrompter"/> class.
	/// </summary>
	/// <param name="logger">The logger in which to log the <see cref="TryPrompt"/> invocation.</param>
	public LoggingReviewPrompter(ILogger<LoggingReviewPrompter>? logger)
	{
		_logger = logger ?? NullLogger<LoggingReviewPrompter>.Instance;
	}

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		_logger.LogInformation("TryPrompt was invoked.");

		return Task.CompletedTask;
	}
}
