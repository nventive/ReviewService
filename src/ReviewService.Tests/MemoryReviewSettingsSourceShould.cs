using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewService.Tests;

public sealed class MemoryReviewSettingsSourceShould
{
	[Fact]
	public async Task Not_return_null_when_reading_from_a_new_instance()
	{
		// Arrange
		var source = new MemoryReviewSettingsSource<ReviewSettings>();
		var cancellationToken = new CancellationToken();

		// Act
		var result = await source.Read(cancellationToken);

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task Return_the_same_value_that_was_written_when_reading()
	{
		// Arrange
		var source = new MemoryReviewSettingsSource<ReviewSettings>();
		var cancellationToken = new CancellationToken();
		var value = new ReviewSettings
		{
			RequestCount = 10
		};

		await source.Write(cancellationToken, value);
		
		// Act
		var result = await source.Read(cancellationToken);

		// Assert
		Assert.Equal(value, result);
	}
}
