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

    public class DepartmentLoader : AbstractValueLoader<long, Department>
    {
        private DepartmentRepository repository;

        public DepartmentLoader(DepartmentRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Department> BatchFetch(IReadOnlyCollection<long> keys)
        {
            return repository.FindByIds(keys);
        }

        protected override long GetKey(Department value)
        {
            return value.Id;
        }
    }
}
