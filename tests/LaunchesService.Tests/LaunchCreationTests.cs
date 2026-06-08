using System;
using Xunit;
using FluentAssertions;

namespace LaunchesService.Tests;

public class LaunchCreationTests
{
    [Fact]
    public void SimpleAmount_ShouldBePositiveOrNegative()
    {
        var amount = 100.5m;
        amount.Should().BeGreaterThan(0);
    }
}
