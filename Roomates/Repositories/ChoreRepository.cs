
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    internal class ChoreRepository : BaseRepository, IGenericRepository<Chore>
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT id, Name FROM Chore";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> list = new List<Chore>();

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("Id"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));

                            Chore addChore = new Chore()
                            {
                                Id = id,
                                Name = name
                            };
                            list.Add(addChore);
                        }

                        return list;
                    }
                }
            }
        }

        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;

                        if (reader.Read())
                        {
                            chore = new Chore()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                        }
                        return chore;
                    }
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                } 
            }
        }

        public List<Chore> GetUnassignedChores()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Chore.Id, Chore.Name
                                        FROM Chore
                                        LEFT JOIN RoommateChore ON Chore.Id = RoommateChore.ChoreId
                                        WHERE RoommateChore.Id IS NULL;";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while(reader.Read())
                        {
                            chores.Add(new Chore()
                            {
                                Id=reader.GetInt32(reader.GetOrdinal("Id")),
                                Name=reader.GetString(reader.GetOrdinal("Name")),
                            });
                        }
                        return chores;
                    }
                }
            }
        }
        public void AssignChore(int roomateId, int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@roommateId, @choreId)";
                    cmd.Parameters.AddWithValue("@roommateId", roomateId);
                    cmd.Parameters.AddWithValue("@choreId", choreId);
                }
            }
        }
    }
}
