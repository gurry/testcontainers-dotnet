namespace DotNet.Testcontainers.Configurations
{
  using System;

  public interface IWaitStrategyOption
  {
    ushort Retries { get; }

    TimeSpan Interval { get; }

    TimeSpan Timeout { get; }

    IWaitStrategyOption WithRetries(ushort retries);

    IWaitStrategyOption WithInterval(TimeSpan interval);

    IWaitStrategyOption WithTimeout(TimeSpan timeout);
  }
}
