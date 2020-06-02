using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLCSharpExample.DataAccess;
using GraphQLCSharpExample.DataAccess.Database;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.Model.Input;

namespace GraphQLCSharpExample.BusinessLogic
{
    using Exception;

    public class OrgMutation
    {
        private SingletonSQLiteDb db;

        private DepartmentRepository departmentRepository;

        private EmployeeRepository employeeRepository;

        public OrgMutation(
            SingletonSQLiteDb db,
            DepartmentRepository departmentRepository,
            EmployeeRepository employeeRepository)
        {
            this.db = db;
            this.departmentRepository = departmentRepository;
            this.employeeRepository = employeeRepository;
        }

        public long CreateDepartment(string name)
        {
            return doInTransaction(
                () => departmentRepository.Insert(name)
            );
        }

        public bool ModifyDepartment(long id, string name)
        {
            return doInTransaction(
                () => departmentRepository.Update(id, name) != 0
            );
        }

        public bool DeleteDepartment(long id)
        {
            return doInTransaction(
                () => {
                    var employees = employeeRepository.FindByDepartmentIds(new List<long> { id });
                    if (employees.Count != 0)
                    {
                        throw BusinessException.CannotDeleteDepartmentWithEmployees(
                            id,
                            employees
                        );
                    }
                    return departmentRepository.Delete(id) != 0;
                }
            );
        }

        public long CreateEmployee(EmployeeInput input)
        {
            return doInTransaction(
                () => {
                    var departments = departmentRepository.FindByIds(new List<long> { input.DepartmentId });
                    if (departments.Count == 0)
                    {
                        throw BusinessException.IllegalDepartmentId(input.DepartmentId);
                    }
                    if (input.SupervisorId != null)
                    {
                        var supervisors = employeeRepository.FindByIds(new List<long> { input.SupervisorId ?? 0 });
                        if (supervisors.Count == 0)
                        {
                            throw BusinessException.IllegalSupervisorId(input.SupervisorId ?? 0);
                        }
                    }
                    return employeeRepository.Insert(input);
                }
            );
        }

        public bool ModifyEmployee(long id, EmployeeInput input)
        {
            return doInTransaction(
                () => {
                    var departments = departmentRepository.FindByIds(new List<long> { input.DepartmentId });
                    if (departments.Count == 0)
                    {
                        throw BusinessException.IllegalDepartmentId(input.DepartmentId);
                    }
                    validateSupervisorReferenceCycle(id, input.SupervisorId, new List<Employee>());
                    return employeeRepository.Update(id, input) != 0;
                }
            );
        }

        public bool DeleteEmployee(long id)
        {
            return doInTransaction(
                () => {
                    var subordinates = employeeRepository.FindBySupervisorIds(new List<long> { id });
                    if (subordinates.Count != 0)
                    {
                        throw BusinessException.CannotDeleteEmployeeWithSubordinates(
                            id,
                            subordinates
                        );
                    }
                    return employeeRepository.Delete(id) != 0;
                }
            );
        }

        private T doInTransaction<T>(Func<T> handler)
        {
            T result;
            db.BeginTransaction();
            try
            {
                result = handler();
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
            db.CommitTransaction();
            return result;
        }

        private void validateSupervisorReferenceCycle(
            long id,
            long? supervisorId,
            IList<Employee> supervisors//Chain
        )
        {
            if (supervisorId == null)
            {
                return;
            }
            var supervisor = employeeRepository.FindByIds(new List<long> { supervisorId ?? 0 }).FirstOrDefault();
            if (supervisor == null)
            {
                throw BusinessException.IllegalSupervisorId(supervisorId ?? 0);
            }
            supervisors.Add(supervisor);
            if (id == supervisorId)
            {
                IList<Employee> cycle = new List<Employee>(supervisors);
                cycle.Insert(0, supervisors[supervisors.Count - 1]);
                throw BusinessException.SupervisorCycle(id, cycle);
            }
            validateSupervisorReferenceCycle(id, supervisor.SupervisorId, supervisors);
        }
    }
}
