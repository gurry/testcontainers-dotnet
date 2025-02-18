namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public static class TestcontainersContainerCancellationTest
  {
    public sealed class Cancel : IClassFixture<AlpineFixture>
    {
      private readonly AlpineFixture _alpineFixture;

      public Cancel(AlpineFixture alpineFixture)
      {
        _alpineFixture = alpineFixture;
      }

      [Fact]
      public async Task Start()
      {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
        {
          var expectedExceptions = new[] { typeof(TaskCanceledException), typeof(OperationCanceledException), typeof(TimeoutException), typeof(IOException) };

          // It depends which part in the StartAsync gets canceled. Catch base exception.
          var exception = await Assert.ThrowsAnyAsync<SystemException>(() => _alpineFixture.Container.StartAsync(cts.Token));
          Assert.Contains(exception.GetType(), expectedExceptions);
        }
      }
    }
  }
}
