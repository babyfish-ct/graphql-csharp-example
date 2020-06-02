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

    public class EmployeeListBySupervisorIdLoader : AbstractListLoader<int, Employee>
    {
        private EmployeeRepository repository;

        public EmployeeListBySupervisorIdLoader(EmployeeRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Employee> BatchFetch(IReadOnlyCollection<int> keys)
        {
            return repository.FindBySupervisorIds(keys);
        }

        protected override int GetKey(Employee value)
        {
            return value.SupervisorId ?? throw new InvalidProgramException("Internal bug");
        }
    }
}
