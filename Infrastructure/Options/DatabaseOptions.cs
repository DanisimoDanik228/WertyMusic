namespace Infrastructure.Options;

public class MongoDbOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}

public class PostgresOptions
{
    public string ConnectionString { get; set; }
}

public class DatabaseOptions
{
    public PostgresOptions Postgres { get; set; }
    public MongoDbOptions MongoDb { get; set; }
}