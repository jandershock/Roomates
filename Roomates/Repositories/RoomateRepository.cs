using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    internal class RoomateRepository : BaseRepository
    {
        public RoomateRepository(string connectionString) : base(connectionString) { }
        public Roomate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Roommate.Id, FirstName, LastName, RentPortion, MoveInDate, Room.Name
                                        FROM Roommate
                                        JOIN Room ON Room.Id = Roommate.RoomId
                                        WHERE Roommate.Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Roomate roomate = null;
                        if (reader.Read())
                        {
                            roomate = new Roomate()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                                Room = new Room()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                }
                            };
                        }
                        return roomate;
                    }
                }
            }
        }
        public List<Roomate> GetAll()
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Roommate.Id, FirstName, LastName, RentPortion, MoveInDate, Room.Id AS RoomId, Room.Name AS RoomName, Room.MaxOccupancy
                                        FROM Roommate
                                        JOIN Room ON Room.Id = Roommate.RoomId;";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Roomate> roomateList = new List<Roomate>();
                        while(reader.Read())
                        {
                            roomateList.Add(new Roomate()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                                Room = new Room()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                    Name = reader.GetString(reader.GetOrdinal("RoomName")),
                                    MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                                }
                            });
                        }
                        return roomateList;
                    }
                }
            }
        }

        public List<Roomate> GetAssigneesForChoreId(int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Roommate.Id, FirstName, LastName, RentPortion, MoveInDate, Roommate.RoomId, Room.Name AS RoomName
                                        FROM RoommateChore
                                        JOIN Roommate ON RoommateChore.RoommateId = Roommate.Id
                                        JOIN Room ON Room.Id = Roommate.RoomId
                                        WHERE RoommateChore.ChoreId = @choreId;";
                    cmd.Parameters.AddWithValue("@choreId", choreId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Roomate> rList = new List<Roomate>();
                        while (reader.Read())
                        {
                            rList.Add(new Roomate()
                            {
                                Id=reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName=reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName=reader.GetString(reader.GetOrdinal("LastName")),
                                RentPortion=reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                MovedInDate=reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                                Room=new Room()
                                {
                                    Id=reader.GetInt32(reader.GetOrdinal("RoomId")),
                                    Name=reader.GetString(reader.GetOrdinal("RoomName"))
                                }
                            });
                        }
                        return rList;
                    }
                }
            }
        }

        public int ReassignChoreToAnotherRoommate(int oldRoommateId, int choreId, int newRoommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE RoommateChore
                                        SET RoommateId = @newRoomateId
                                        WHERE RoommateId = @oldRoomateId AND ChoreId = @choreId";
                    cmd.Parameters.AddWithValue("@newRoomateId", newRoommateId);
                    cmd.Parameters.AddWithValue("@oldRoomateId", oldRoommateId);
                    cmd.Parameters.AddWithValue("@choreId", choreId);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        //public List<Chore> GetAllChoresForRoomate(int roomateId)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT DISTINCT Chore.Id AS choreId, Chore.Name
        //                                FROM Roommate
        //                                JOIN RoommateChore ON RoommateChore.RoommateId = Roommate.Id
        //                                JOIN CHORE ON RoommateChore.ChoreId = Chore.Id
        //                                WHERE Roommate.Id = @roomateId;";
        //            cmd.Parameters.AddWithValue("@roomateId", roomateId);
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                List<Chore> roommateChoreList = new List<Chore>();
        //                while(reader.Read())
        //                {
        //                    roommateChoreList.Add(new Chore()
        //                    {
        //                        Id=reader.GetInt32(reader.GetOrdinal("choreId")),
        //                        Name=reader.GetString(reader.GetOrdinal("Name"))
        //                    });
        //                }
        //                return roommateChoreList;
        //            }
        //        }
        //    }
        //}
    }
}