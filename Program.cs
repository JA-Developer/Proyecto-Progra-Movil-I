using PrediccionAsistenciaAPI.Contexts;
using PrediccionAsistenciaAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar política CORS que permita cualquier origen, cabecera y método
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Inyectar ConexionSQLite (igual que en el proyecto Prueba)
var connectionString = builder.Configuration.GetConnectionString("CadenaConexionSQLite")
    ?? "Data Source=clinica_psicologia.db";

var conexionSqlite = new ConexionSQLite(connectionString);
conexionSqlite.InicializarBaseDeDatos();

builder.Services.AddScoped(_ => new ConexionSQLite(connectionString));

// 4. Inyectar los Repositorios para cada entidad
builder.Services.AddScoped<PacienteRepository>();
builder.Services.AddScoped<PsicologoRepository>();
builder.Services.AddScoped<CitaRepository>();
builder.Services.AddScoped<ModeloMlRepository>();
builder.Services.AddScoped<PrediccionCitaRepository>();
builder.Services.AddScoped<IntervencionCitaRepository>();
builder.Services.AddScoped<DatasetMlRepository>();

var app = builder.Build();

// 5. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();
