[![Build Status](https://img.shields.io/appveyor/ci/thiagoloureiro/netcore-jwt-integrator-extension/master.svg)](https://ci.appveyor.com/project/thiagoloureiro/netcore-jwt-integrator-extension)

# rbk-api-modules
Set of libraries for quickly scaffolding production ready APIs in ASP.NET Core

1 Falar das libs que vao em cada projeto
2 Explicar as classes de cada lib
3 Criar o user herdando de BaseUser
4 Chamar o ApplyConfigurantions no DatabaseContext

services.AddTransient<DbContext, DatabaseContext>();
services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
services.AddRbkApiModulesInfrastructure();


"Microsoft.EntityFrameworkCore.Database.Command": "Information"

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database"))
                .EnableSensitiveDataLogging());


                    public class UserConfig: BaseUserConfig, IEntityTypeConfiguration<User>
    { 
        public void Configure(EntityTypeBuilder<User> entity)
        {
            base.Configure<User>(entity, 512, 1024);

            entity.HasOne(x => x.Client)
                .WithOne(x => x.User)
                .HasForeignKey<User>(x => x.Id);
        }
    }