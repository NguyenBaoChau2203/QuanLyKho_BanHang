using Microsoft.Data.SqlClient;
using QuanLyKhoBanHang.DTO.Sales;
using System;
using System.Collections.Generic;
using QuanLyKhoBanHang.DAL.Data;

namespace QuanLyKhoBanHang.DAL
{
    public class CustomerRepository
    {
        private readonly DatabaseOptions _options;
        private string _connStr => _options.ConnectionString;

        public CustomerRepository()
        {
            _options = new DatabaseOptions();
        }

        public CustomerRepository(DatabaseOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public List<CustomerDto> GetAllCustomers()
        {
            var list = new List<CustomerDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Customers WHERE IsActive = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CustomerDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Code = reader["Code"]?.ToString() ?? "",
                            Name = reader["Name"]?.ToString() ?? "",
                            Phone = reader["Phone"]?.ToString() ?? "",
                            Email = reader["Email"]?.ToString() ?? "",
                            Address = reader["Address"]?.ToString() ?? "",
                            IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : true
                        });
                    }
                }
            }
            return list;
        }

        public List<CustomerDto> SearchCustomers(string keyword)
        {
            var list = new List<CustomerDto>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Customers WHERE IsActive = 1 AND (Name LIKE @Kw OR Phone LIKE @Kw OR Code LIKE @Kw)", conn);
                cmd.Parameters.AddWithValue("@Kw", "%" + keyword + "%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CustomerDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Code = reader["Code"]?.ToString() ?? "",
                            Name = reader["Name"]?.ToString() ?? "",
                            Phone = reader["Phone"]?.ToString() ?? "",
                            Email = reader["Email"]?.ToString() ?? "",
                            Address = reader["Address"]?.ToString() ?? "",
                            IsActive = true
                        });
                    }
                }
            }
            return list;
        }

        public CustomerDto GetCustomerById(int id)
        {
            CustomerDto customer = null;
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Code, Name, Phone, Email, Address, IsActive FROM Customers WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        customer = new CustomerDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Code = reader["Code"]?.ToString() ?? "",
                            Name = reader["Name"]?.ToString() ?? "",
                            Phone = reader["Phone"]?.ToString() ?? "",
                            Email = reader["Email"]?.ToString() ?? "",
                            Address = reader["Address"]?.ToString() ?? "",
                            IsActive = reader["IsActive"] != DBNull.Value ? Convert.ToBoolean(reader["IsActive"]) : true
                        };
                    }
                }
            }
            return customer;
        }

        public int CreateCustomer(CustomerDto customer)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO Customers (Code, Name, Phone, Email, Address, IsActive) OUTPUT INSERTED.Id VALUES (@Code, @Name, @Phone, @Email, @Address, 1)", conn);
                cmd.Parameters.AddWithValue("@Code", (object)customer.Code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void UpdateCustomer(CustomerDto customer)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Customers SET Code = @Code, Name = @Name, Phone = @Phone, Email = @Email, Address = @Address WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@Code", (object)customer.Code ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeactivateCustomer(int id)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand("UPDATE Customers SET IsActive = 0 WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
