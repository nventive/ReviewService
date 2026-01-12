namespace ReviewService.Tests;

public sealed class MemoryReviewSettingsSourceShould
{
	[Fact]
	public async Task Not_Return_Null_When_Reading_From_A_New_Instance()
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
	public async Task Return_The_Same_Value_That_Was_Written_When_Reading()
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
