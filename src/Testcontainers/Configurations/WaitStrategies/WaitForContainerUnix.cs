namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerUnix : WaitForContainerOS
  {
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategyOption> option)
    {
      // TODO: We can move this to AddCustomWaitStrategy(IWaitUntil).
      var waitUntil = new WaitStrategyOption(new UntilUnixCommandIsCompleted(command));
      option(waitUntil);
      return AddCustomWaitStrategy(waitUntil);
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return AddCustomWaitStrategy(new UntilUnixCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      return AddCustomWaitStrategy(new UntilUnixPortIsAvailable(port));
    }
  }
}
