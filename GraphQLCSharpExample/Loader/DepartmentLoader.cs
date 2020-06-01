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

    public class DepartmentLoader : AbstractValueLoader<int, Department>
    {
        private DepartmentRepository repository;

        public DepartmentLoader(DepartmentRepository repository)
        {
            this.repository = repository;
        }

        protected override IList<Department> BatchLoad(IReadOnlyCollection<int> keys)
        {
            return repository.FindByIds(keys);
        }

        protected override int GetKey(Department value)
        {
            return value.Id;
        }
    }
}
