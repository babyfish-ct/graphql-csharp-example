using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using LinqToDB.Mapping;
using GraphQLCSharpExample.Loader;

namespace GraphQLCSharpExample.Model
{
    [GraphQLDescription("Employee object. " +
        "It always belongs to a parent department object, " +
        "may belong to a parent supervisor object")]
    [Table("employee")]
    public class Employee
    {
        [PrimaryKey, Identity]
        public int Id { get; set; } = 0;

        [Column, NotNull]
        public string Name { get; set; } = string.Empty;

        [Column, NotNull]
        public Gender Gender { get; set; } = Gender.Male;

        [Column, NotNull]
        public decimal Salary { get; set; } = 0;

        /*
         * In Linq2DB, there is an attribute [Assoication], 
         * which is used to map the foreign key to associated parent object
         * 
         * but, GraphQL is better than ORM, 
         * it's unnecessary to use [Association],
         * [Column] is enough! 
         */
        [Column, NotNull]
        [GraphQLIgnore]
        public int DepartmentId { get; set; } = 0;

        /*
         * In Linq2DB, there is an attribute [Assoication], 
         * which is used to map the foreign key to associated parent object
         * 
         * but, GraphQL is better than ORM, 
         * it's unnecessary to use [Association],
         * [Column] is enough! 
         */
        [Column, Nullable]
        [GraphQLIgnore]
        public int? SupervisorId { get; set; }

        public Task<Department> GetDepartment([DataLoader] DepartmentLoader loader)
        {
            return loader.LoadRequiredAsync(DepartmentId);
        }

        public Task<Employee?> GetSupervisor([DataLoader] EmployeeLoader loader)
        {
            if (SupervisorId == null)
            {
                return Task<Employee?>.FromResult<Employee?>(null);
            }
            return loader.LoadOptionalAsync(SupervisorId ?? throw new InvalidOperationException("Internal bug"));
        }

        public Task<IReadOnlyList<Employee>> GetSubordinates([DataLoader] EmployeeListBySupervisorIdLoader loader)
        {
            return loader.LoadAsync(Id);
        }
    }
}
