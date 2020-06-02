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

    public class DepartmentAvgSalaryLoader : AbstractValueLoader<long, Tuple<long, decimal>>
    {
        private EmployeeRepository repository;

        public DepartmentAvgSalaryLoader(EmployeeRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Tuple<long, decimal>> BatchFetch(IReadOnlyCollection<long> keys)
        {
            return repository.FindAvgSalaryByDepartmentIds(keys);
        }

        protected override long GetKey(Tuple<long, decimal> value)
        {
            return value.Item1;
        }
    }
}
