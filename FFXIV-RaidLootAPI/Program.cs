using FFXIV_RaidLootAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using FFXIV_RaidLootAPI.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        //Scheme = "Bearer"
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthorizationBuilder();
builder.Services.AddDbContextFactory<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnection"));
});
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<DataContext>()
    .AddApiEndpoints();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
    });

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder.WithOrigins("http://localhost:4200","https://localhost:7203", "https://xivloot.com", "172.64.80.1", "10.124.0.3")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());

    //options.AddPolicy("AllowProdOrigin",
    //    builder => builder.WithOrigins("https://xivloot.com")
    //                      .AllowAnyHeader()
    //                      .AllowAnyMethod()
    //                      .AllowCredentials());
});
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.UseHttpClientMetrics();

var app = builder.Build();
app.UseDeveloperExceptionPage();
// Configure the HTTP request pipeline.
if (true)//(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseHttpMetrics();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();



using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}
app.Use(async (context, next) =>
{   
    //Console.WriteLine($"Request URL: {context.Request.Path}");
    context.Response.OnStarting(() =>
    {
        var headers = context.Response.Headers;
        //Console.WriteLine("CORS Headers:");
        foreach (var header in headers)
        {
            //Console.WriteLine($"{header.Key}: {header.Value}");
        }
        return Task.CompletedTask;
    });
    await next();
});


app.MapIdentityApi<ApplicationUser>();
app.MapMetrics();
app.MapControllers();

app.Run();