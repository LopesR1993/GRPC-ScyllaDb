using Cassandra;
using GRPC.Server.Services;

var builder = WebApplication.CreateBuilder(args);

var cluster = Cluster
            .Builder()
            .AddContactPoints("192.168.50.92")
            .Build();
var connection = cluster.Connect("customers");

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<Cassandra.ISession>(connection);
var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<CustomerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
