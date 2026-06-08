using System;
using Xunit;
// usando FluentAssertions removido — utilizar xUnit Assert

namespace LaunchesService.Tests;

public class LaunchCreationTests
{
    [Fact]
    public void SimpleAmount_ShouldBePositiveOrNegative()
    {
        var amount = 100.5m;
        Assert.True(amount > 0);
    }
}
