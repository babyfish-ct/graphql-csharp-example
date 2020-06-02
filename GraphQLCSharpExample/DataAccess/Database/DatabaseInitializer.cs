using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using GraphQLCSharpExample.Model;
using GraphQLCSharpExample.Model.Input;

namespace GraphQLCSharpExample.DataAccess.Database
{
    public class DatabaseInitializer : IHostedService
    {
        private SingletonSQLiteDb db;

        private DepartmentRepository departmentRepository;

        private EmployeeRepository employeeRepository;

        public DatabaseInitializer(
            SingletonSQLiteDb db,
            DepartmentRepository departmentRepository,
            EmployeeRepository employeeRepository)
        {
            this.db = db;
            this.departmentRepository = departmentRepository;
            this.employeeRepository = employeeRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            db.CreateTable<Department>();
            db.CreateTable<Employee>();

            db.BeginTransaction();
            try
            {
                initData();
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
            db.CommitTransaction();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void initData()
        {
            int developId = departmentRepository.Insert("Develop");
            int testId = departmentRepository.Insert("Test");

            int jimId = employeeRepository.Insert(
                new EmployeeInput 
                { 
                    Name = "Jim",
                    Gender = Gender.Male,
                    Salary = 10000,
                    DepartmentId = developId
                }
            );
            employeeRepository.Insert(
                new EmployeeInput
                {
                    Name = "Kate",
                    Gender = Gender.Female,
                    Salary = 8000,
                    DepartmentId = developId,
                    SupervisorId = jimId
                }
            );
            employeeRepository.Insert(
                new EmployeeInput
                {
                    Name = "Bob",
                    Gender = Gender.Male,
                    Salary = 7000,
                    DepartmentId = developId,
                    SupervisorId = jimId
                }
            );

            int lindaId = employeeRepository.Insert(
                new EmployeeInput
                {
                    Name = "Linda",
                    Gender = Gender.Female,
                    Salary = 11000,
                    DepartmentId = testId
                }
            );
            employeeRepository.Insert(
                new EmployeeInput
                {
                    Name = "Smith",
                    Gender = Gender.Female,
                    Salary = 6000,
                    DepartmentId = testId,
                    SupervisorId = lindaId
                }
            );
            employeeRepository.Insert(
                new EmployeeInput
                {
                    Name = "Daria",
                    Gender = Gender.Female,
                    Salary = 5000,
                    DepartmentId = testId,
                    SupervisorId = lindaId
                }
            );
        }
    }
}
