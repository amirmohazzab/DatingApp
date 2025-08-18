using DatingApp.Api.Errors;
using DatingApp.Api.Extensions;
using DatingApp.Api.Helper;
using DatingApp.Api.Middlewares;
using DatingApp.Application.Helper;
using DatingApp.Data.Context;
using DatingApp.Data.SeedData;
using DatingApp.Domain.Entities.User;
using DatingApp.Ioc.Dependencies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

#region Add Services

builder.Services.RegisterServices();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

builder.Services.AddApplicationService(builder.Configuration);

builder.Services.AddScoped<LogUserActivity>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

#endregion

#region Cors

builder.Services.AddCors();

#endregion

#region Error Handling

builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();

            var errorResponse = new ApiValidationErrorResponse
            {
                Errors = errors
            };

            return new BadRequestObjectResult(errorResponse);
        }
);

#endregion

#region Identity

builder.Services.AddIdentityService(builder.Configuration);

#endregion

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//    try
//    {
//        var context = services.GetRequiredService<DatingAppDbContext>();
//        //var userManager = services.GetRequiredService<UserManager<User>>();
//        await context.Database.MigrateAsync();
//        await SeedUserData.SeedUsers(context, loggerFactory);
//    }
//    catch (Exception ex)
//    {
//        var logger = loggerFactory.CreateLogger("Program");
//        logger.LogError(ex, "An error occurred during migration or seeding.");
//    }
//}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseMiddleware<ExceptionHandlerMilldeware>();

app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
