using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.DataAccess;

namespace GraphQLCSharpExample.Loader
{
    using Common;

    public class EmployeeLoader : AbstractValueLoader<long, Employee>
    {
        private EmployeeRepository repository;

        public EmployeeLoader(EmployeeRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Employee> BatchFetch(IReadOnlyCollection<long> keys)
        {
            return repository.FindByIds(keys);
        }

        protected override long GetKey(Employee value)
        {
            return value.Id;
        }
    }
}
