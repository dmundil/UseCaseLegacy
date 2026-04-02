using System;
using System.Collections.Generic;

namespace AnyTeller.Models
{
    public class UserRecord
    {
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string ContactNumber { get; set; }
        public string Manager { get; set; }
        public string Email { get; set; }
        public string JobDescription { get; set; }
        public string Level { get; set; }
        public string LeaveReason { get; set; }

        public static List<UserRecord> GetMockData()
        {
            return new List<UserRecord>
            {
                new UserRecord { FirstName = "John", Username = "jdoe", ContactNumber = "555-0101", Manager = "Alice Smith", Email = "jdoe@anyteller.com", JobDescription = "Teller", Level = "L1", LeaveReason = "Sick Leave" },
                new UserRecord { FirstName = "Sarah", Username = "sconnor", ContactNumber = "555-0102", Manager = "Alice Smith", Email = "sconnor@anyteller.com", JobDescription = "Senior Teller", Level = "L2", LeaveReason = "Vacation" },
                new UserRecord { FirstName = "Mike", Username = "mross", ContactNumber = "555-0103", Manager = "Harvey Specter", Email = "mross@anyteller.com", JobDescription = "Associate", Level = "L1", LeaveReason = "Personal" },
                new UserRecord { FirstName = "Rachel", Username = "rzane", ContactNumber = "555-0104", Manager = "Donna Paulsen", Email = "rzane@anyteller.com", JobDescription = "Paralegal", Level = "L2", LeaveReason = "Maternity" },
                new UserRecord { FirstName = "Louis", Username = "llitt", ContactNumber = "555-0105", Manager = "Jessica Pearson", Email = "llitt@anyteller.com", JobDescription = "Partner", Level = "L3", LeaveReason = "Sabbatical" },
                new UserRecord { FirstName = "Dwight", Username = "dschrute", ContactNumber = "555-0106", Manager = "Michael Scott", Email = "dschrute@anyteller.com", JobDescription = "Assistant to the RM", Level = "L2", LeaveReason = "Beet Farming" },
                new UserRecord { FirstName = "Jim", Username = "jhalpert", ContactNumber = "555-0107", Manager = "Michael Scott", Email = "jhalpert@anyteller.com", JobDescription = "Sales", Level = "L2", LeaveReason = "Prank War" },
                new UserRecord { FirstName = "Pam", Username = "pbeesly", ContactNumber = "555-0108", Manager = "Michael Scott", Email = "pbeesly@anyteller.com", JobDescription = "Receptionist", Level = "L1", LeaveReason = "Art School" },
                new UserRecord { FirstName = "Ryan", Username = "rhoward", ContactNumber = "555-0109", Manager = "Michael Scott", Email = "rhoward@anyteller.com", JobDescription = "Temp", Level = "L1", LeaveReason = "Business School" },
                new UserRecord { FirstName = "Stanley", Username = "shudson", ContactNumber = "555-0110", Manager = "Michael Scott", Email = "shudson@anyteller.com", JobDescription = "Sales", Level = "L2", LeaveReason = "Retirement" }
            };
        }
    }
}
