using APBD_Task_6.Models;
using System.Data.SqlClient;

namespace Zadanie5.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IConfiguration _configuration;

        public WarehouseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> AddProduct(ProductWarehouse productWarehouse)
        {
            var connectionString = _configuration.GetConnectionString("Database");
            using var connection = new SqlConnection(connectionString);
            using var cmd = new SqlCommand();

            cmd.Connection = connection;

            await connection.OpenAsync();

            cmd.CommandText = "SELECT TOP 1 [Order].IdOrder FROM " +
                "LEFT JOIN Product_Warehouse ON [Order].IdOrder = Product_Warehouse.IdOrder " +
                "WHERE [Order].IdProduct = @IdProduct" +
                "AND [Order].Amount = @Amount " +
                "AND Product_Warehouse.IdProductWarehouse IS NULL " +
                "AND [Order].CreatedAt < @CreatedAt";

            cmd.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);
            cmd.Parameters.AddWithValue("Amount", productWarehouse.Amount);
            cmd.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);

            var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception();

            await reader.ReadAsync();

            int idOrder = int.Parse(reader["InOrder"].ToString());
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            cmd.Parameters.AddWithValue("IdProduct", productWarehouse.IdProduct);

            reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception();
            await reader.ReadAsync();
           
            double price = double.Parse(reader["Price"].ToString());
            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT IdWarehouse FROM Warehouse WHERE IdWarehouse = @IdWarehouse";
            cmd.Parameters.AddWithValue("IdWarehouse", productWarehouse.IdWarehouse);

            reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows) throw new Exception();
            await reader.ReadAsync();
            cmd.Parameters.Clear();

            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            cmd.Transaction = transaction;

            try {
                cmd.CommandText = "UPDATE [Order] SET FullfilledAt = @CreatedAt WHERE IdOrder = @IdOrder";
                cmd.Parameters.AddWithValue("CreatedAt", productWarehouse.CreatedAt);

            }

        }
    }
}
