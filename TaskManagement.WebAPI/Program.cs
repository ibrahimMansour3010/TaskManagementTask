using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using TaskManagement.Infrastructure;
using TaskManagement.Application;
using TaskManagement.Infrastructure.Context;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ApplicationService();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Swagger Doc",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert Token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
               {
                 new OpenApiSecurityScheme
                 {
                   Reference = new OpenApiReference
                   {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                   }
                  },
                  new string[] { }
                }
              });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<AppInitializer>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
    {
        ["activated"] = false
    };
    c.RoutePrefix = "api";
    c.DocumentTitle = "Swagger UI - TaskManagment";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", $" V1");
    c.OAuthClientSecret("no_password"); //Leaving it blank doesn't work
});

app.UseCors("CorsPolicy");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();
