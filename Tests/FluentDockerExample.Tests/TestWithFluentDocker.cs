using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using Xunit;

namespace FluentDockerExample.Tests;

public class DockerFixture : IDisposable
{
    private ICompositeService? CompositeService { get; }

    public DockerFixture()
    {
        CompositeService = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile("./docker-compose.yml")
            .RemoveOrphans()
            .Build()
            .Start();
    }

    public void Dispose()
    {
        CompositeService?.Stop();
        CompositeService?.Remove(true);
    }
}

public class TestWithFluentDocker : IClassFixture<DockerFixture>
{
    #region fields

    private static readonly ConnectionMultiplexer Redis =
        ConnectionMultiplexer.Connect(
        new ConfigurationOptions
        {
            EndPoints = {"localhost:7000"}
        });

    #endregion

    #region

    [Fact]
    public void TestRedis()
    {
        var db = Redis.GetDatabase();
        var pong = db.Ping();

        Assert.IsNotNull(db);
    }

    #endregion
}