using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using CoffeeShop.Models;
using System.Data.SqlClient;

namespace CoffeeShop.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly string _connectionString;

        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            using (var conn = Connection)
            {
                using (var conn = Connection)
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Id, Title, BeanVarietyId FROM Coffee;";
                        var reader = cmd.ExecuteReader();
                        var coffees = new List<Coffee>();
                        while (reader.Read())
                        {
                            var coffee = new Coffee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                BeanVarietyId = reader.GetInt32(reader.GetOrdinal())
                            };
                            coffees.Add(coffee);
                        }
                    }
                }
            }
        }

        public Coffee Get(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT c.Id, Title, BeanVarietyId, b.Notes, b.[Name] AS BeanVarietyName
                          FROM Coffee c
                          LEFT JOIN BeanVariety b ON b.Id = c.beanVarietyId
                          WHERE c.Id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    Coffee coffee = null;
                    if (reader.Read())
                    {
                        coffee = new Coffee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),

                            BeanVariety = new BeanVariety
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Notes = reader.GetString(reader.GetOrdinal("Notes"))
                            }
                        };

                    }
                }
            }
        }
    }
}
