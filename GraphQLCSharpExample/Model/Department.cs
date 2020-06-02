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
    [GraphQLDescription(
        @"Department object, it can contains some child Employee objects.
        
        Notes:
        The JVM server example(https://github.com/babyfish-ct/graphql-kotlin-example)
        and the React client example(https://github.com/babyfish-ct/graphql-react-example)
        are created eariler than this C# server example.
        
        In those examples, the type of all the primary keys and foreign keys of database(H2) is long.
        
        This C# example uses SQLite, SQLite's auto increment field can only be int, long is not supported!
        so, there is no idea except declaring the primarys key and foreign keys as properties 
        whose type is 'int' in this LINQ2DB entityClass
        
        In order to make this C# example can work with the client example,
        the other modules(such as Repository, Query and Mutation) still consider them as long property.
        fortunately, 'int' and 'long' can be automatically converted to each oher in SQLite."
    )]
    [Table("department")]
    public class Department
    {
        [GraphQLDescription(
            @"Please see the GraphQLDescription of the class to know 
            why the type of this property is int(not long)"
        )]
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column, NotNull]
        public string Name { get; set; } = string.Empty;

        public async Task<decimal?> GetAvgSalary([DataLoader] DepartmentAvgSalaryLoader loader)
        {
            return (await loader.LoadAsync(Id))?.Item2;
        }

        public Task<IReadOnlyList<Employee>> GetEmployees([DataLoader] EmployeeListByDepartmentIdLoader loader)
        {
            return loader.LoadAsync(Id);
        }
    }
}
