namespace Cmms.Server.IntegrationTest;

using System;
using System.Threading.Tasks;
using Orleans.Streams;
using Cmms.Abstractions.Constants;
using Cmms.Abstractions.Grains;
using Cmms.Server.IntegrationTest.Fixtures;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// 
/// </summary>
/// <param name="testOutputHelper"></param>
public class HelloGrainTest(ITestOutputHelper testOutputHelper) : ClusterFixture(testOutputHelper)
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SayHello_PassName_ReturnsGreetingAsync()
    {
        var grain = this.Cluster.GrainFactory.GetGrain<IHelloGrain>(Guid.NewGuid());

        var greeting = await grain.SayHelloAsync("Rehan").ConfigureAwait(false);

        Assert.Equal("Hello Rehan!", greeting);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SayHello_PassName_CountIncrementedAsync()
    {
        var helloGrain = this.Cluster.GrainFactory.GetGrain<IHelloGrain>(Guid.NewGuid());
        var counterGrain = this.Cluster.GrainFactory.GetGrain<ICounterGrain>(Guid.Empty);

        await helloGrain.SayHelloAsync("Rehan").ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        var count = await counterGrain.GetCountAsync().ConfigureAwait(false);

        Assert.Equal(1L, count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task SayHello_PassName_SaidHelloPublishedAsync()
    {
        string? hello = null;
        var helloGrain = this.Cluster.GrainFactory.GetGrain<IHelloGrain>(Guid.NewGuid());
        var streamProvider = this.Cluster.Client.GetStreamProvider(StreamProviderName.Default);
        var stream = streamProvider.GetStream<string>(Guid.Empty, StreamName.SaidHello);
        var subscription = await stream
            .SubscribeAsync(
                (x, token) =>
                {
                    hello = x;
                    return Task.CompletedTask;
                })
            .ConfigureAwait(false);

        await helloGrain.SayHelloAsync("Rehan").ConfigureAwait(false);

        Assert.Equal("Rehan", hello);
    }
}
