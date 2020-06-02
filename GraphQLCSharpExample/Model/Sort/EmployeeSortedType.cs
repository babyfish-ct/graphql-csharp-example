using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLCSharpExample.Model.Sort
{
    public enum EmployeeSortedType
    {
        Id,
        Name,
        Salary,

        /*
         * HotChocolate(https://hotchocolate.io/) is very well,
         * its output result is the standard JSON, not C# style JSON.
         * 
         * 1. All the property name follows camel rule, not pascal rule
         * 2. Enum constant are rendered as the string with uppercase characters, not C# enum numeric.
         * 
         * But, it is still not smart enough, for example
         * C# Enum constant name 'DepartmentId' will be rendered as 'DEPARTMENTID',
         * not the expected text 'DEPARTMENT_NAME'.
         * 
         * OMG, no idead except let this constant lieteral to be 'Department_Id', 
         * which seems very strange to C# language 
         */
        Department_Id,

        /**
         * The perfect is 'DepartmentName', not 'Department_Name',
         * please see the comment of 'Department_Id' to know more.
         */
        Department_Name
    }
}
