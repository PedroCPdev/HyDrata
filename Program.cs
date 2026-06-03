using HyDrata.GestaoApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddScoped<IProdutorRepository,           ProdutorRepository>();
builder.Services.AddScoped<ICooperativaRepository,        CooperativaRepository>();
builder.Services.AddScoped<IPlanoRepository,              PlanoRepository>();
builder.Services.AddScoped<IPropriedadeRepository,        PropriedadeRepository>();
builder.Services.AddScoped<IProdutorCooperativaRepository, ProdutorCooperativaRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "HyDrata – API de Gestão",
        Version     = "v1",
        Description = "API administrativa do HyDrata: gestão de produtores, cooperativas, planos e propriedades rurais."
    });
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HyDrata Gestão v1"));

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
