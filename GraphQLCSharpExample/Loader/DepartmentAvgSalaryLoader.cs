using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.DataAccess;

namespace GraphQLCSharpExample.Loader
{
    using Common;

    public class DepartmentAvgSalaryLoader : AbstractValueLoader<int, Tuple<int, decimal>>
    {
        private EmployeeRepository repository;

        public DepartmentAvgSalaryLoader(EmployeeRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Tuple<int, decimal>> BatchLoad(IReadOnlyCollection<int> keys)
        {
            return repository.FindAvgSalaryByDepartmentIds(keys);
        }

        protected override int GetKey(Tuple<int, decimal> value)
        {
            return value.Item1;
        }
    }
}
