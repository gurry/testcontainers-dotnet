namespace Testcontainers.LocalStack;

public abstract class LocalStackContainerTest : IAsyncLifetime
{
    private const string AwsService = "Service";

    private readonly LocalStackContainer _localStackContainer;

    static LocalStackContainerTest()
    {
        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", CommonCredentials.AwsAccessKey);
        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", CommonCredentials.AwsSecretKey);
    }

    private LocalStackContainerTest(LocalStackContainer localStackContainer)
    {
        _localStackContainer = localStackContainer;
    }

    public Task InitializeAsync()
    {
        return _localStackContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _localStackContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "cloudwatch")]
    public async Task CreateLogReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonCloudWatchLogsConfig();
        config.ServiceURL = _localStackContainer.GetConnectionString();

        using var client = new AmazonCloudWatchLogsClient(config);

        var logGroupRequest = new CreateLogGroupRequest(Guid.NewGuid().ToString("D"));

        // When
        var logGroupResponse = await client.CreateLogGroupAsync(logGroupRequest)
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, logGroupResponse.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "dynamodb")]
    public async Task GetItemReturnsPutItem()
    {
        // Given
        var id = Guid.NewGuid().ToString("D");

        var tableName = Guid.NewGuid().ToString("D");

        var config = new AmazonDynamoDBConfig();
        config.ServiceURL = _localStackContainer.GetConnectionString();

        using var client = new AmazonDynamoDBClient(config);

        var tableRequest = new CreateTableRequest();
        tableRequest.TableName = tableName;
        tableRequest.AttributeDefinitions = new List<AttributeDefinition> { new AttributeDefinition("Id", ScalarAttributeType.S) };
        tableRequest.KeySchema = new List<KeySchemaElement> { new KeySchemaElement("Id", KeyType.HASH) };
        tableRequest.ProvisionedThroughput = new ProvisionedThroughput(10, 5);

        var putItemRequest = new PutItemRequest();
        putItemRequest.TableName = tableName;
        putItemRequest.Item = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = id } } };

        var getItemRequest = new GetItemRequest();
        getItemRequest.TableName = tableName;
        getItemRequest.Key = new Dictionary<string, AttributeValue> { { "Id", new AttributeValue { S = id } } };

        // When
        _ = await client.CreateTableAsync(tableRequest)
            .ConfigureAwait(false);

        _ = await client.PutItemAsync(putItemRequest)
            .ConfigureAwait(false);

        var itemResponse = await client.GetItemAsync(getItemRequest)
            .ConfigureAwait(false);

        // Then
        Assert.Equal(id, itemResponse.Item.Values.Single().S);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "s3")]
    public async Task ListBucketsReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonS3Config();
        config.ServiceURL = _localStackContainer.GetConnectionString();

        using var client = new AmazonS3Client(config);

        // When
        var buckets = await client.ListBucketsAsync()
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, buckets.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "sns")]
    public async Task CreateTopicReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonSimpleNotificationServiceConfig();
        config.ServiceURL = _localStackContainer.GetConnectionString();

        using var client = new AmazonSimpleNotificationServiceClient(config);

        // When
        var topicResponse = await client.CreateTopicAsync(Guid.NewGuid().ToString("D"))
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, topicResponse.HttpStatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    [Trait(AwsService, "sqs")]
    public async Task CreateQueueReturnsHttpStatusCodeOk()
    {
        // Given
        var config = new AmazonSQSConfig();
        config.ServiceURL = _localStackContainer.GetConnectionString();

        using var client = new AmazonSQSClient(config);

        // When
        var queueResponse = await client.CreateQueueAsync(Guid.NewGuid().ToString("D"))
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, queueResponse.HttpStatusCode);
    }

    // 2023-05-17T14:17:05.033  WARN --- [   asgi_gw_0] localstack.utils.archives  : Attempt 1. Failed to download archive from https://s3-us-west-2.amazonaws.com/dynamodb-local/dynamodb_local_latest.zip: MyHTTPSConnectionPool(host='s3-us-west-2.amazonaws.com', port=443): Max retries exceeded with url: /dynamodb-local/dynamodb_local_latest.zip (Caused by NewConnectionError('<urllib3.connection.HTTPSConnection object at 0xffff65444bb0>: Failed to establish a new connection: [Errno -3] Temporary failure in name resolution'))
    // 2023-05-17T14:17:05.037  INFO --- [   asgi_gw_0] localstack.utils.archives  : Unable to extract file, re-downloading ZIP archive /tmp/localstack.ddb.zip: ('Failed to download archive from %s: . Retries exhausted', 'https://s3-us-west-2.amazonaws.com/dynamodb-local/dynamodb_local_latest.zip')
    // 2023-05-17T14:17:25.079  WARN --- [   asgi_gw_0] localstack.utils.archives  : Attempt 1. Failed to download archive from https://s3-us-west-2.amazonaws.com/dynamodb-local/dynamodb_local_latest.zip: MyHTTPSConnectionPool(host='s3-us-west-2.amazonaws.com', port=443): Max retries exceeded with url: /dynamodb-local/dynamodb_local_latest.zip (Caused by NewConnectionError('<urllib3.connection.HTTPSConnection object at 0xffff65444d00>: Failed to establish a new connection: [Errno -3] Temporary failure in name resolution'))
    // 2023-05-17T14:17:25.080  WARN --- [   asgi_gw_0] localstack.utils.functions : error calling function on_before_start: Installation of dynamodb-local failed.    
    // [UsedImplicitly]
    // public sealed class LocalStackDefaultConfiguration : LocalStackContainerTest
    // {
    //     public LocalStackDefaultConfiguration()
    //         : base(new LocalStackBuilder().Build())
    //     {
    //     }
    // }
    //
    // [UsedImplicitly]
    // public sealed class LocalStackV1Configuration : LocalStackContainerTest
    // {
    //     public LocalStackV1Configuration()
    //         : base(new LocalStackBuilder().WithImage("localstack/localstack:1.4").Build())
    //     {
    //     }
    // }
}