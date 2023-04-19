namespace Testcontainers.Tests
{
    public sealed class WaitStrategyProposal
    {
        [Fact]
        public void Proposal1()
        {
            var configuration = new ContainerBuilder()
                .WithImage(CommonImages.Alpine)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("foo", o => o.WithRetries(3).WithTimeout(TimeSpan.FromSeconds(5))))
                .Build();
        }
    }
}