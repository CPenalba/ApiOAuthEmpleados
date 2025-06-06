using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
HelperCryptography.Initialize(builder.Configuration);
//INYECTAMOS HTTOCONTEXTACCCESSOR
builder.Services.AddHttpContextAccessor();

//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionServicesOAuth helper = new HelperActionServicesOAuth(builder.Configuration);
//ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ PARA QUE NUESTRA APLICACION PUEDA VALIDAR CON TODO LO QUE HA CREADO
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
//HABILITAMOS LA SEGURIDAD UTILIZANDO LA CLASE HELPER
builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

//YA NO UTILIZAREMOS CONFIGURATION NUNCA MAS
//NECESITAMOS RECUPERAR EL OBJETO INYECTADO
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret secret = await secretClient.GetSecretAsync("SqlAzure");

string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
}
app.MapOpenApi();
app.UseHttpsRedirection();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(url: "/openapi/v1.json", name: "Api OAuth Empleados");
    options.RoutePrefix = "";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
