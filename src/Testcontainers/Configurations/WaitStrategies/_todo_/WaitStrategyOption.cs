namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;

  internal sealed class WaitStrategyOption : IWaitUntil, IWaitStrategyOption
  {
    private readonly IWaitUntil _waitUntil;

    public WaitStrategyOption(IWaitUntil waitUntil)
    {
      _waitUntil = waitUntil;
    }

    public ushort Retries { get; private set; }

    public TimeSpan Interval { get; private set; }

    public TimeSpan Timeout { get; private set; }

    public IWaitStrategyOption WithRetries(ushort retries)
    {
      Retries = retries;
      return this;
    }

    public IWaitStrategyOption WithInterval(TimeSpan interval)
    {
      Interval = interval;
      return this;
    }

    public IWaitStrategyOption WithTimeout(TimeSpan timeout)
    {
      Timeout = timeout;
      return this;
    }

    public async Task<bool> UntilAsync(IContainer container)
    {
      // TODO: Implement the "wait strategy" that supports the backoff strategy etc.

      // This will avoid casting in DockerContainer. Maybe there is also another approach?
      await WaitStrategy.WaitUntilAsync(() => _waitUntil.UntilAsync(container), Interval, Timeout)
        .ConfigureAwait(false);

      return true;
    }
  }
}
