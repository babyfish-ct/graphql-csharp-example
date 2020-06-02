using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.Model.Input;
using GraphQLCSharpExample.Model.Sort;
using LinqToDB;

namespace GraphQLCSharpExample.DataAccess
{
    using Database;
    using Common;

    public class EmployeeRepository
    {
        private SingletonSQLiteDb db;

        public EmployeeRepository(SingletonSQLiteDb db)
        {
            this.db = db;
        }

        public int Count(EmployeeCriteriaInput? criteria)
        {
            // This is db count operation, not memory count operation.
            // Linq2DB will generate "select count(*) from ..." automatically. 
            return addCriteria(from e in db.Employees select e, criteria).Count();
        }

        public IList<Employee> Find(
            EmployeeCriteriaInput? criteria,
            EmployeeSortedType sortedType,
            bool descending,
            int? limit,
            int? offset) 
        {
            var query = from e in db.Employees select e;
            query = addCriteria(query, criteria);
            switch (sortedType)
            {
                case EmployeeSortedType.Id:
                    query = query.OrderBy(descending, e => e.Id);
                    break;
                case EmployeeSortedType.Name:
                    query = query.OrderBy(descending, e => e.Name, e => e.Id);
                    break;
                case EmployeeSortedType.Salary:
                    query = query.OrderBy(descending, e => e.Salary, e => e.Id);
                    break;
                case EmployeeSortedType.Department_Id:
                    query = query.OrderBy(descending, e => e.DepartmentId, e => e.Id);
                    break;
                case EmployeeSortedType.Department_Name:
                    var joinQuery = 
                        (from e in query
                            join d in db.Departments on e.DepartmentId equals d.Id
                         select new { Department = d, Employee = e });
                    query = joinQuery
                        .OrderBy(descending, tuple => tuple.Department.Name, tuple => tuple.Employee.Id)
                        .Select(tuple => tuple.Employee);
                    break;
            }
            return query.Limit(limit, offset).ToList();
        }

        public IList<Employee> FindByIds(IReadOnlyCollection<int> ids)
        {
            var query =
                from e in db.Employees
                where ids.Contains(e.Id)
                select e;
            return query.ToList();
        }

        public IList<Employee> FindByDepartmentIds(IReadOnlyCollection<int> departmentIds)
        {
            var query =
                from e in db.Employees
                where departmentIds.Contains(e.DepartmentId)
                select e;
            return query.ToList();
        }

        public IList<Employee> FindBySupervisorIds(IReadOnlyCollection<int> supervisorIds)
        {
            var query =
                from e in db.Employees
                where supervisorIds.Contains(Sql.ToNotNullable(e.SupervisorId))
                select e;
            return query.ToList();
        }

        public IList<Tuple<int, decimal>> FindAvgSalaryByDepartmentIds(IReadOnlyCollection<int> departmentIds)
        {
            var query =
                from e in db.Employees
                where departmentIds.Contains(e.DepartmentId)
                group e.Salary by e.DepartmentId into g
                select new Tuple<int, decimal>(g.Key, g.Average());
            return query.ToList();
        }

        public int Insert(EmployeeInput input)
        {
            return db
                .Employees
                .Value(e => e.Name, input.Name)
                .Value(e => e.Gender, input.Gender)
                .Value(e => e.Salary, input.Salary)
                .Value(e => e.DepartmentId, input.DepartmentId)
                .Value(e => e.SupervisorId, input.SupervisorId)
                .InsertWithInt32Identity() ?? throw new InvalidProgramException("Internal bug");
        }

        public int Update(int id, EmployeeInput input)
        {
            return db
                .Employees
                .Where(e => e.Id == id)
                .Set(e => e.Name, input.Name)
                .Set(e => e.Gender, input.Gender)
                .Set(e => e.Salary, input.Salary)
                .Set(e => e.DepartmentId, input.DepartmentId)
                .Set(e => e.SupervisorId, input.SupervisorId)
                .Update();
        }

        public int Delete(int id)
        {
            return db.Employees.Delete(e => e.Id == id);
        }

        private static IQueryable<Employee> addCriteria(
            IQueryable<Employee> query, 
            EmployeeCriteriaInput? criteria)
        {
            if (criteria != null)
            {
                if (!string.IsNullOrEmpty(criteria.Name))
                {
                    query =
                        from e in query
                        where e.Name.ToLower().Contains(criteria.Name.ToLower())
                        select e;
                }
                if (criteria.Gender != null)
                {
                    query =
                        from e in query
                        where e.Gender == criteria.Gender
                        select e;
                }
                if (criteria.MinSalary != null)
                {
                    query =
                        from e in query
                        where e.Salary >= criteria.MinSalary
                        select e;
                }
                if (criteria.MaxSalary != null)
                {
                    query =
                        from e in query
                        where e.Salary <= criteria.MaxSalary
                        select e;
                }
                if (criteria.DeparmtentIds != null)
                {
                    query =
                        from e in query
                        where criteria.DeparmtentIds.Contains(e.DepartmentId)
                        select e;
                }
            }
            return query;
        }
    }
}
