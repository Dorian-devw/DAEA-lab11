using Microsoft.EntityFrameworkCore;
using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de repositorios con AddScoped
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Carga de datos iniciales al iniciar la aplicación si las tablas están vacías
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();

    // 1. Categorías en Español (3 categorías)
    if (!await context.Categorias.AnyAsync())
    {
        var catMarketing = new Categoria { Nombre = "Marketing & Publicidad", Activo = true };
        var catEmprendimiento = new Categoria { Nombre = "Emprendimiento", Activo = true };
        var catFiestas = new Categoria { Nombre = "Fiestas & Conciertos", Activo = true };

        context.Categorias.AddRange(catMarketing, catEmprendimiento, catFiestas);
        await context.SaveChangesAsync();
    }

    // 2. Asistentes (5 asistentes)
    if (!await context.Asistentes.AnyAsync())
    {
        var asistentes = new List<Asistente>
        {
            new Asistente { Nombres = "Frank Sinca", Dni = "71234561", Correo = "frank.sinca@tecsup.edu.pe", Telefono = "987654321", Activo = true },
            new Asistente { Nombres = "Valeria Ramos", Dni = "71234562", Correo = "valeria.ramos@gmail.com", Telefono = "987654322", Activo = true },
            new Asistente { Nombres = "Diego Mendoza", Dni = "71234563", Correo = "diego.mendoza@outlook.com", Telefono = "987654323", Activo = true },
            new Asistente { Nombres = "Camila Torres", Dni = "71234564", Correo = "camila.torres@empresa.com", Telefono = "987654324", Activo = true },
            new Asistente { Nombres = "Mateo Vargas", Dni = "71234565", Correo = "mateo.vargas@yahoo.com", Telefono = "987654325", Activo = true }
        };

        context.Asistentes.AddRange(asistentes);
        await context.SaveChangesAsync();
    }

    // 3. Eventos en Español (4 eventos)
    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.ToListAsync();
        var catMarketingId = categorias.FirstOrDefault(c => c.Nombre.Contains("Marketing"))?.Id ?? categorias[0].Id;
        var catEmprendimientoId = categorias.FirstOrDefault(c => c.Nombre.Contains("Emprendimiento"))?.Id ?? categorias[0].Id;
        var catFiestasId = categorias.FirstOrDefault(c => c.Nombre.Contains("Fiestas"))?.Id ?? categorias[0].Id;

        var eventos = new List<Evento>
        {
            new Evento
            {
                Titulo = "Bootcamp de Growth Marketing y Estrategias Digitales",
                Fecha = DateTime.Now.AddDays(7).Date.AddHours(18),
                Lugar = "Innoland Co-working Center",
                Aforo = 50,
                Precio = 65.00m,
                Modalidad = "Presencial",
                CategoriaId = catMarketingId,
                Activo = true
            },
            new Evento
            {
                Titulo = "Cumbre Nacional de Emprendimiento e Innovación",
                Fecha = DateTime.Now.AddDays(14).Date.AddHours(19),
                Lugar = "Remoto",
                Aforo = 100,
                Precio = 85.00m,
                Modalidad = "Virtual",
                CategoriaId = catEmprendimientoId,
                Activo = true
            },
            new Evento
            {
                Titulo = "Sunset Festival: Gran Fiesta & Música en Vivo",
                Fecha = DateTime.Now.AddDays(21).Date.AddHours(20),
                Lugar = "Salón Mirador Hotel Westin",
                Aforo = 120,
                Precio = 120.00m,
                Modalidad = "Presencial",
                CategoriaId = catFiestasId,
                Activo = true
            },
            new Evento
            {
                Titulo = "Taller Intensivo de Marketing y Ventas B2B",
                Fecha = DateTime.Now.AddDays(28).Date.AddHours(18).AddMinutes(30),
                Lugar = "Centro Empresarial Real San Isidro",
                Aforo = 80,
                Precio = 70.00m,
                Modalidad = "Presencial",
                CategoriaId = catMarketingId,
                Activo = true
            }
        };

        context.Eventos.AddRange(eventos);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
