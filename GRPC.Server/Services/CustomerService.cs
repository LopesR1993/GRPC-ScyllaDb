using Grpc.Core;

namespace GRPC.Server.Services
{
    public class CustomerService : Customer.CustomerBase
    {
        private readonly Cassandra.ISession _connection;

        public CustomerService(Cassandra.ISession connection)
        {
            _connection = connection;
        }

        public override async Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            var result = _connection.Execute($"SELECT * FROM list WHERE ID = {request.Id};");

            var user = new CustomerModel();

            foreach (var item in result)
            {
                user.Id = item.GetValue<Guid>("id").ToString();
                user.Name = item.GetValue<string>("name");
            };

            return await Task.FromResult(user);
        }

        public override async Task GetNewCustomers(NewCustomerRequest request, IServerStreamWriter<CustomerModel> responseStream, ServerCallContext context)
        {
            var result = _connection.Execute("SELECT * FROM list;");

            var output = new List<CustomerModel>();

            foreach (var item in result)
            {
                output.Add(new CustomerModel()
                {
                    Id = item.GetValue<Guid>("id").ToString(),
                    Name = item.GetValue<string>("name"),
                });
            }

            foreach (var customer in output)
            {
                await responseStream.WriteAsync(customer);
            }
        }

        public override async Task<CustomerModel> CreateCustomerInfo(CreateCustomerRequest request, ServerCallContext context)
        {
            var result = _connection.Execute($"INSERT INTO list (id,name) VALUES (uuid(),'{request.Name}');");

            var output = new CustomerModel();
            foreach(var item in result)
            {
                output.Id = item.GetValue<Guid>("id").ToString();
                output.Id = item.GetValue<string>("name");
            }
            return await Task.FromResult(output);
        }

        public override async Task<CustomerModel> PutCustomerInfo(EditCustomerRequest request, ServerCallContext context)
        {
            var result = _connection.Execute($"UPDATE list " +
                $"SET name = '{request.Name}'" +
                $"WHERE id = {request.Id};");

            var output = new CustomerModel()
            {
                Id = request.Id,
                Name = request.Name
            };

            return await Task.FromResult(output);
        }

        public override async Task<CustomerModel> DeleteCustomerInfo(DeleteCustomerRequest request, ServerCallContext context)
        {
            var result = _connection.Execute($"DELETE FROM list WHERE id = {request.Id};");

            var output = new CustomerModel();

            return await Task.FromResult(output);
        }
    }
}

