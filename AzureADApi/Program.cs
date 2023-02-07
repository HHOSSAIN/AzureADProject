using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//adding authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = "https://login.microsoftonline.com/21af2c97-fc02-46fd-84fb-323961b73470/v2.0"; //copied from web client program.cs
            options.Audience = "api://f6b5f57a-2924-435e-93d3-9ef705e7d308"; //it's the api id we generated in "expose an api" in azure.
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//add authentication middleware b4 authorisation
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
