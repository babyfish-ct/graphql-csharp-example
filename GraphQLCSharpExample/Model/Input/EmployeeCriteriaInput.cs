using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLCSharpExample.Model.Input
{
    public class EmployeeCriteriaInput
    {
        public string? Name { get; set; }

        public Gender? Gender { get; set; }

        public decimal? MinSalary { get; set; }

        public decimal? MaxSalary { get; set; }

        public IReadOnlyCollection<long>? DeparmtentIds { get; set; }
    }
}
