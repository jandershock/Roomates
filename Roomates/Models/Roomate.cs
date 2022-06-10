using System;
using System.Collections.Generic;

namespace Roommates.Models
{
    // C# representation of the Roommate table
    public class Roomate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RentPortion { get; set; }
        public DateTime MovedInDate { get; set; }
        public Room Room { get; set; }

        public List<Chore> assignedChores { get; set;}
    }
}