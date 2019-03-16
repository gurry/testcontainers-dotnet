namespace DotNet.Testcontainers.Core.Models.Database
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  public sealed class MySqlTestcontainerConfiguration : DatabaseConfiguration
  {
    private readonly IDictionary<string, string> environments = new Dictionary<string, string>();

    public MySqlTestcontainerConfiguration() : base("mysql:8.0.15", 3306)
    {
      this.environments["MYSQL_ALLOW_EMPTY_PASSWORD"] = "yes";
    }

    public override string Database
    {
      get => this.environments["MYSQL_DATABASE"];
      set => this.environments["MYSQL_DATABASE"] = value;
    }

    public override string Username
    {
      get => this.environments["MYSQL_USER"];
      set => this.environments["MYSQL_USER"] = value;
    }

    public override string Password
    {
      get => this.environments["MYSQL_PASSWORD"];
      set => this.environments["MYSQL_PASSWORD"] = value;
    }

    public override IReadOnlyDictionary<string, string> Environments => new ReadOnlyDictionary<string, string>(this.environments);
  }
}