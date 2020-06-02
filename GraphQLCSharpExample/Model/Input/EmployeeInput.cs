using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLCSharpExample.Model.Input
{
    public class EmployeeInput
    {
        public string Name { get; set; } = string.Empty;

        public Gender Gender { get; set; } = Gender.Male;

        public decimal Salary { get; set; } = 0;

        public long DepartmentId { get; set; } = 0;

        public long? SupervisorId { get; set; }
    }
}
