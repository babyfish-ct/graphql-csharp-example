using System;
using System.Linq;
using System.Collections.Generic;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.Model.Input;
using GraphQLCSharpExample.Model.Sort;
using GraphQLCSharpExample.DataAccess;

namespace GraphQLCSharpExample.BusinessLogic
{
    public class OrgQuery
    {
        private DepartmentRepository departmentRepository;

        private EmployeeRepository employeeRepository;

        public OrgQuery(
            DepartmentRepository departmentRepository,
            EmployeeRepository employeeRepository)
        {
            this.departmentRepository = departmentRepository;
            this.employeeRepository = employeeRepository;
        }

        public Department? GetDepartment(long id)
        {
            return departmentRepository.FindByIds(new List<long> { id }).FirstOrDefault();
        }

        public int GetDepartmentCount(string? name)
        {
            return departmentRepository.Count(name);
        }

        public IList<Department> GetDepartments(
            string? name,
            DepartmentSortedType? sortedType,
            bool? descending,
            int? limit,
            int? offset)
        {
            return departmentRepository.Find(
                name, 
                sortedType ?? DepartmentSortedType.Id,
                descending ?? false,
                limit, 
                offset
            );
        }

        public Employee? GetEmployee(long id)
        {
            return employeeRepository.FindByIds(new List<long> { id }).FirstOrDefault();
        }

        public int GetEmployeeCount(EmployeeCriteriaInput? criteria)
        {
            return employeeRepository.Count(criteria);
        }

        public IList<Employee> GetEmployees(
            EmployeeCriteriaInput? criteria,
            EmployeeSortedType? sortedType,
            bool? descending,
            int? limit,
            int? offset)
        {
            return employeeRepository.Find(
                criteria,
                sortedType ?? EmployeeSortedType.Id,
                descending ?? false,
                limit,
                offset
            );
        }
    }
}
