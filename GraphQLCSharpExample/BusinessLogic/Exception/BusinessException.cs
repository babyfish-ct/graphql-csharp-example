using System;
using System.Linq;
using System.Collections.Generic;
using GraphQLCSharpExample.Model;

namespace GraphQLCSharpExample.BusinessLogic.Exception
{
    public class BusinessException : ApplicationException
    {
        public string Code { get; private set; }

        public IReadOnlyDictionary<string, object> Fields { get; private set; }

        public BusinessException(
            string code,
            string message,
            IReadOnlyDictionary<string, object>? fields = null) :
            base(message)
        {
            Code = code;
            Fields = fields ?? new Dictionary<string, object>();
        }

        public static BusinessException IllegalLoginName(string loginName)
        {
            return new BusinessException(
                "ILLEGAL_LOGIN_NAME",
                $"The user whose login name is '{loginName}' does not exists"
            );
        }

        public static BusinessException IllegalPassowrd()
        {
            return new BusinessException(
                "ILLEGAL_PASSWORD",
                "The password is illegal"
            );
        }

        public static BusinessException Unauthorized()
        {
            return new BusinessException(
                "UNAUTHORIZED",
                "Unauthorized, please login"
            );
        }

        public static BusinessException IllegalDepartmentId(long departmentId)
        {
            return new BusinessException(
                "ILLEGAL_DEPARTMENT_ID",
                $"The department whose id is {departmentId} does not exists",
                new Dictionary<string, object>
                {
                    { nameof(departmentId), departmentId }
                }
            );
        }

        public static BusinessException IllegalSupervisorId(long supervisorId)
        {
            return new BusinessException(
                "ILLEGAL_DEPARTMENT_ID",
                $"The supervisor whose id is {supervisorId} does not exists",
                new Dictionary<string, object>
                {
                    { nameof(supervisorId), supervisorId }
                }
            );
        }

        public static BusinessException CannotDeleteDepartmentWithEmployees(
            long departmentId,
            IEnumerable<Employee> employees)
        {
            return new BusinessException(
                "CANNOT_DELETE_DEPARTMENT_WITH_EMPLOYEES",
                $"Cannot delete the department whose id is {departmentId} because it has employees",
                new Dictionary<string, object>
                {
                    { nameof(departmentId), departmentId },
                    {
                        nameof(employees),
                        (
                            from e in employees
                            select new Dictionary<string, object>
                            {
                                { "id", e.Id },
                                { "name", e.Name }
                            }
                        ).ToList()
                    }
                }
            );
        }

        public static BusinessException CannotDeleteEmployeeWithSubordinates(
            long employeeId,
            IEnumerable<Employee> subordinates)
        {
            return new BusinessException(
                "CANNOT_DELETE_EMPLOYEE_WITH_SUBORDINATES",
                $"Cannot delete the employee whose id is {employeeId} because it has subordinates",
                new Dictionary<string, object>
                {
                    { nameof(employeeId), employeeId },
                    {
                        nameof(subordinates),
                        (
                            from e in subordinates
                            select new Dictionary<string, object>
                            {
                                { "id", e.Id },
                                { "name", e.Name }
                            }
                        ).ToList()
                    }
                }
            );
        }

        public static BusinessException SupervisorCycle(
            long employeeId,
            IEnumerable<Employee> supervisors)
        {
            return new BusinessException(
                "SUPERVISOR_CYCLE",
                "New employee has supervisor cycle",
                new Dictionary<string, object>
                {
                    { nameof(employeeId), employeeId },
                    {
                        nameof(supervisors),
                        (
                            from e in supervisors
                            select new Dictionary<string, object>
                            {
                                { "id", e.Id },
                                { "name", e.Name }
                            }
                        ).ToList()
                    }
                }
            );
        }
    }
}
