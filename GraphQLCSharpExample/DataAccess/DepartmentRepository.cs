using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.Model.Sort;

namespace GraphQLCSharpExample.DataAccess
{
    using Database;
    using Common;

    public class DepartmentRepository
    {
        private SingletonSQLiteDb db;

        public DepartmentRepository(SingletonSQLiteDb db)
        {
            this.db = db;
        }

        public int Count(string? name)
        {
            // This is db count operation, not memory count operation.
            // Linq2DB will generate "select count(*) from ..." automatically. 
            return addCriteria(from d in db.Departments select d, name).Count();
        }

        public IList<Department> Find(
            string? name,
            DepartmentSortedType sortedType,
            bool descending,
            int? limit,
            int? offset)
        {
            var query = addCriteria(from d in db.Departments select d, name);
            switch (sortedType)
            {
                case DepartmentSortedType.Id:
                    query = query.OrderBy(descending, d => d.Id);
                    break;
                case DepartmentSortedType.Name:
                    query = query.OrderBy(descending, d => d.Name);
                    break;
            }
            return query.Limit(limit, offset).ToList();
        }

        public IList<Department> FindByIds(IReadOnlyCollection<long> ids)
        {
            var query =
                from d in db.Departments
                where ids.Contains(d.Id)
                select d;
            return query.ToList();
        }

        public long Insert(string name)
        {
            return db
                .Departments
                .Value(d => d.Name, name)
                .InsertWithInt32Identity() ?? throw new InvalidOperationException("Internal bug");
        }

        public int Update(long id, string name)
        {
            return db
                .Departments
                .Where(d => d.Id == id)
                .Set(d => d.Name, name)
                .Update();
        }

        public int Delete(long id)
        {
            return db.Departments.Delete(d => d.Id == id);
        }

        private static IQueryable<Department> addCriteria(
            IQueryable<Department> query, 
            string? name) 
        {
            if (!string.IsNullOrEmpty(name))
            {
                query = from d in query where d.Name.ToLower().Contains(name.ToLower()) select d;
            }
            return query;
        }
    }
}
