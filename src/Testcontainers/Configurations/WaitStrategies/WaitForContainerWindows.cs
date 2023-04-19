namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <inheritdoc cref="IWaitForContainerOS" />
  internal sealed class WaitForContainerWindows : WaitForContainerOS
  {
    public override IWaitForContainerOS UntilCommandIsCompleted(string command, Action<IWaitStrategyOption> option)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(string command)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilCommandIsCompleted(params string[] command)
    {
      return AddCustomWaitStrategy(new UntilWindowsCommandIsCompleted(command));
    }

    /// <inheritdoc />
    public override IWaitForContainerOS UntilPortIsAvailable(int port)
    {
      return AddCustomWaitStrategy(new UntilWindowsPortIsAvailable(port));
    }
  }
}
