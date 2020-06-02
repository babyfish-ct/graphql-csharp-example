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

    public class EmployeeListByDepartmentIdLoader : AbstractListLoader<long, Employee>
    {
        private EmployeeRepository repository;

        public EmployeeListByDepartmentIdLoader(EmployeeRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Employee> BatchFetch(IReadOnlyCollection<long> keys)
        {
            return repository.FindByDepartmentIds(keys);
        }

        protected override long GetKey(Employee value)
        {
            return value.DepartmentId;
        }
    }
}
