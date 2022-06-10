using System.Collections.Generic;

namespace Roommates.Models
{
    // C# representation of the Chore table
    public class Chore
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Roomate> Assignees { get; set; }
    }
}