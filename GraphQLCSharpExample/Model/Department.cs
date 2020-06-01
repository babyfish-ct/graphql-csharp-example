using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.Mapping;
using HotChocolate;
using GreenDonut;
using GraphQLCSharpExample.Loader;

namespace GraphQLCSharpExample.Model
{
    [Table("department")]
    public class Department
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public string Name { get; set; } = string.Empty;

        //public decimal? AvgSalary { get; set; }

        public Task<IReadOnlyList<Employee>> GetEmployees([DataLoader] EmployeeListByDepartmentIdLoader loader)
        {
            return loader.LoadAsync(Id, new CancellationToken());
        }
    }
}
