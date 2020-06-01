using System.Collections.Generic;
using GraphQLCSharpExample.Model;
using System.Linq;

namespace GraphQLCSharpExample.DataAccess
{
    using Database;

    public class DepartmentRepository
    {
        private SingletonSQLiteDb db;
        
        public DepartmentRepository(SingletonSQLiteDb db)
        {
            this.db = db;
        }

        public IList<Department> Find(string? name) 
        {
            IQueryable<Department> query;
            query = from d in db.Departments select d;
            if (!string.IsNullOrEmpty(name))
            {
                query = from d in query where d.Name.ToLower().Contains(name.ToLower()) select d;
            }
            return query.ToList();
        }

        public IList<Department> FindByIds(IReadOnlyCollection<int> ids)
        {
            var query =
                from d in db.Departments
                where ids.Contains(d.Id)
                select d;
            return query.ToList();
        }
    }
}
