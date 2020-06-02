using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HotChocolate;
using HotChocolate.AspNetCore;
using GraphQLCSharpExample.BusinessLogic;
using GraphQLCSharpExample.DataAccess;
using GraphQLCSharpExample.DataAccess.Database;
using GraphQLCSharpExample.Loader;

namespace GraphQLCSharpExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            setupdDatabase(services);

            services.AddCors(
                options => {
                    options.AddPolicy(
                        "CorsPolicy",
                        builder =>
                        {
                            builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin();
                        }
                    );
                }
            );

            services.AddGraphQL(
                sp => SchemaBuilder
                .New()
                .AddQueryType<OrgQuery>()
                .AddServices(sp)
                .Create()
            );

            services.AddDataLoaderRegistry();
            services.AddDataLoader<DepartmentLoader>();
            services.AddDataLoader<EmployeeLoader>();
            services.AddDataLoader<DepartmentAvgSalaryLoader>();
            services.AddDataLoader<EmployeeListByDepartmentIdLoader>();
            services.AddDataLoader<EmployeeListBySupervisorIdLoader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseRouting()
                .UseCors("CorsPolicy");

            const string path = "/graphql";
            app
                .UseGraphQL(path)
                .UsePlayground(path);
        }

        private void setupdDatabase(IServiceCollection services)
        {
            /**
             * Only sqlite needs the singleton database object.
             * Don't do the same thing for other database.
             */
            services.AddSingleton<SingletonSQLiteDb>();
            services.AddHostedService<DatabaseInitializer>();

            services.AddSingleton<DepartmentRepository>();
            services.AddSingleton<EmployeeRepository>();
        }
    }
}
