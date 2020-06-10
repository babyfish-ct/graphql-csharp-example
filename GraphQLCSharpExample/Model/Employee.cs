using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Resolvers;
using LinqToDB.Mapping;
using GraphQLCSharpExample.Loader;

namespace GraphQLCSharpExample.Model
{
    using Common;

    /**
     * The JVM server example(https://github.com/babyfish-ct/graphql-kotlin-example)
     * and the React client example(https://github.com/babyfish-ct/graphql-react-example)
     * are created eariler than this C# server example.
     * 
     * In those examples, the type of all the primary keys and foreign keys of database(H2) is long.
     * 
     * This C# example uses SQLite, SQLite's auto increment field can only be int, long is not supported!
     * so, there is no idea except declaring the primarys key and foreign keys as properties 
     * whose type is 'int' in this LINQ2DB entityClass
     * 
     * In order to make this C# example can work with the client example,
     * the other modules(such as Repository, Query and Mutation) still consider them as long property.
     * fortunately, 'int' and 'long' can be automatically converted to each oher in SQLite.
     */
    [GraphQLDescription(
        @"Employee object. 
        It always belongs to a parent department object, 
        may belong to a parent supervisor object

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
    [Table("employee")]
    public class Employee
    {
        [GraphQLDescription(
            @"Please see the GraphQLDescription of the class to know 
            why the type of this property is int(not long)"
        )]
        [PrimaryKey, Identity]
        public int Id { get; set; } = 0;

        [Column, NotNull]
        public string Name { get; set; } = string.Empty;

        [Column, NotNull]
        public Gender Gender { get; set; } = Gender.Male;

        [Column, NotNull]
        public decimal Salary { get; set; } = 0;

        /*
         * 1.
         * In Linq2DB, there is an attribute [Assoication], 
         * which is used to map the foreign key to associated parent object
         * 
         * but, GraphQL is better than ORM, 
         * it's unnecessary to use [Association],
         * [Column] is enough! 
         * 
         * 2.
         * Please see the GraphQLDescription of the class to know why the type of this property is int(not long)
         */
        [Column, NotNull]
        [GraphQLIgnore]
        public int DepartmentId { get; set; } = 0;

        /*
         * 1. 
         * In Linq2DB, there is an attribute [Assoication], 
         * which is used to map the foreign key to associated parent object
         * 
         * but, GraphQL is better than ORM, 
         * it's unnecessary to use [Association],
         * [Column] is enough! 
         * 
         * 2. 
         * Please see the GraphQLDescription of the class to know why the type of this property is int(not long)
         */
        [Column, Nullable]
        [GraphQLIgnore]
        public int? SupervisorId { get; set; }

        public Task<Department> GetDepartment(
            IResolverContext ctx,
            [DataLoader] DepartmentLoader loader)
        {
            if (ctx.IsSingleField("id"))
            {
                return Task.FromResult(new Department { Id = DepartmentId });
            }
            return loader.LoadRequiredAsync(DepartmentId);
        }

        public Task<Employee?> GetSupervisor(
            IResolverContext ctx,
            [DataLoader] EmployeeLoader loader)
        {
            if (SupervisorId == null)
            {
                return Task<Employee?>.FromResult<Employee?>(null);
            }
            if (ctx.IsSingleField("id"))
            {
                return Task.FromResult<Employee?>(new Employee { Id = SupervisorId ?? 0 });
            }
            return loader.LoadOptionalAsync(SupervisorId ?? throw new InvalidOperationException("Internal bug"));
        }

        public Task<IReadOnlyList<Employee>> GetSubordinates([DataLoader] EmployeeListBySupervisorIdLoader loader)
        {
            return loader.LoadAsync(Id);
        }
    }
}
