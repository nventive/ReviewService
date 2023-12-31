﻿using System;

namespace ReviewService;

/// <summary>
/// The review prompt settings used for prompt conditions.
/// </summary>
public record ReviewSettings
{
	/// <summary>
	/// Gets or sets the number of primary actions completed.
	/// </summary>
	public int PrimaryActionCompletedCount { get; init; }

	/// <summary>
	/// Gets or sets the number of secondary actions completed.
	/// </summary>
	public int SecondaryActionCompletedCount { get; init; }

	/// <summary>
	/// Gets or sets the number of times the application has been launched.
	/// </summary>
	public int ApplicationLaunchCount { get; init; }

	/// <summary>
	/// Gets or sets when the application first started.
	/// </summary>
	public DateTimeOffset? FirstApplicationLaunch { get; init; }

	/// <summary>
	/// Gets or sets the number of review requested.
	/// </summary>
	public int RequestCount { get; init; }

	/// <summary>
	/// Gets or sets when the last review was requested.
	/// </summary>
	public DateTimeOffset? LastRequest { get; init; }
}
