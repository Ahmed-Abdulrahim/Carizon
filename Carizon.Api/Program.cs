using Carizon.Api.Middleware;
using Carizon.Application.Validation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carizon.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;

                options.JsonSerializerOptions.PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerService(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureService(builder.Configuration);
            builder.Services.AddCors(op => op.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyOrigin()
               .AllowAnyHeader();
            }));
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CarizonDbContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                    await app.Services.SeedAsync();
                }
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseGlobalExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
