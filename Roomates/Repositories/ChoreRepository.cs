
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
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Chore.Id, Chore.Name
                                        FROM Chore
                                        LEFT JOIN RoommateChore ON Chore.Id = RoommateChore.ChoreId
                                        WHERE RoommateChore.Id IS NULL;";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while (reader.Read())
                        {
                            chores.Add(new Chore()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            });
                        }
                        return chores;
                    }
                }
            }
        }
        public int AssignChore(int roomateId, int choreId)
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
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        public List<RoomateChoreCount> GetChoreCounts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Roommate.Id, Roommate.FirstName, Roommate.LastName, Roommate.RentPortion, Roommate.MoveInDate, Roommate.RoomId, Room.Name AS RoomName, Room.MaxOccupancy, Count(DISTINCT RoommateChore.ChoreId) AS ChoreCount
                                        FROM Roommate
                                        JOIN RoommateChore ON RoommateChore.RoommateId = Roommate.Id
                                        JOIN Room ON Room.Id = Roommate.RoomId
                                        GROUP BY Roommate.Id, Roommate.FirstName, Roommate.LastName, Roommate.RentPortion, Roommate.MoveInDate, Roommate.RoomId, Room.Name, Room.MaxOccupancy;";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<RoomateChoreCount> choreCounts = new List<RoomateChoreCount>();
                        while (reader.Read())
                        {
                            choreCounts.Add(new RoomateChoreCount()
                            {
                                roomate = new Roomate()
                                {
                                    Id=reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName=reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName=reader.GetString(reader.GetOrdinal("LastName")),
                                    RentPortion=reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                    MovedInDate=reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                                    Room = new Room()
                                    {
                                        Id=reader.GetInt32(reader.GetOrdinal("RoomId")),
                                        Name=reader.GetString(reader.GetOrdinal("RoomName")),
                                        MaxOccupancy=reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                                    }
                                },
                                choreCount = reader.GetInt32(reader.GetOrdinal("ChoreCount"))
                            });
                        }
                        return choreCounts;
                    }
                }
            }
        }

        public int Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                        SET Name=@name
                                        WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@id", chore.Id);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Delete(int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Chore
                                        WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", choreId);
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
