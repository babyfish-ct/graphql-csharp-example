using System;
using System.Collections.Generic;
using GraphQLCSharpExample.Model;
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

        public IList<Department> GetDepartments()
        {
            return departmentRepository.Find(null);
        }

        public IList<Employee> GetEmployees()
        {
            return employeeRepository.Find(null);
        }
    }
}
