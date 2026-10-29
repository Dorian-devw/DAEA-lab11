
> **Desarrollo de Aplicaciones Empresariales Avanzadas**
>
> Docente Arévalo Sermeño, Edwin William
>
> Desarrollo de Aplicaciones Empresariales Avanzadas
>
> 6 - C24 - Sección C - D | TECSUP
> Estudiante Frank Sinca

**[EF Core Code First: Eventos e inscripciones SEMANA11]{.underline}**

En este desafío, se te pide que implementes una aplicación web de
eventos e inscripciones con Entity Framework Core (Code First): la base
de datos se crea a partir de tus clases C# mediante **migraciones**, sin
scripts SQL ni procedimientos almacenados. Todo el acceso a datos se
resuelve con LINQ.

**Base de Datos (se crea con migraciones):**

1.  No se ejecuta ningún script SQL ni se crea la base de datos a mano:
    la base **EventosDB** y sus tablas se generan con migraciones. No
    usar EnsureCreated ni modificar la base directamente en SQL Server.

2.  Definir las siguientes entidades en la carpeta Models, con sus
    relaciones (propiedad de clave foránea y propiedad de navegación) y
    configuradas **solo con Data Annotations**:

  ---------------------------------------------------------------------
  **Entidad**   **Propiedades y restricciones**
  ------------- -------------------------------------------------------
  Categoria     Id (clave); Nombre (obligatorio, máx. 50, único); Activo

  Evento        Id (clave); Titulo (obligatorio, máx. 100); Fecha;
                Lugar (obligatorio, máx. 100); Aforo (entero entre 1 y
                5000); Precio (decimal(10,2), no negativo); CategoriaId
                (FK a Categoria); Activo

  Asistente     Id (clave); Nombres (obligatorio, máx. 100); Dni
                (obligatorio, 8 dígitos, único); Correo (obligatorio,
                formato de correo, único); Activo

  Inscripcion   Id (clave); EventoId (FK a Evento); AsistenteId (FK a
                Asistente); FechaInscripcion; Estado (Confirmada o
                Cancelada)
  ---------------------------------------------------------------------

3.  Los índices únicos se declaran con el atributo
    \[Index(nameof(Propiedad), IsUnique = true)\] sobre la clase, y el
    decimal con \[Column(TypeName = \"decimal(10,2)\")\].

4.  Categoria, Evento y Asistente tienen la columna Activo: la
    eliminación es lógica (Activo = false), nunca un DELETE físico. En
    Inscripcion, cancelar cambia el Estado a Cancelada.

5.  Crear en la carpeta Data la clase **EventosContext** (hereda de
    DbContext) con un DbSet por entidad. **No usar Fluent API**: no se
    sobrescribe OnModelCreating.

**ASP.NET Core MVC:**

6.  Crear un proyecto **ASP.NET Core Web App (Model-View-Controller)**
    de nombre Eventos.Web con los paquetes
    Microsoft.EntityFrameworkCore.SqlServer y
    Microsoft.EntityFrameworkCore.Design. Guardar la cadena de conexión
    (Database=EventosDB) en appsettings.json y registrar el contexto con
    AddDbContext.

7.  Datos iniciales: cargarlos desde Program.cs al iniciar la
    aplicación, solo si la tabla está vacía (AnyAsync, AddRange y
    SaveChangesAsync): 3 categorías, 4 eventos y 5 asistentes. Se
    ejecuta después de aplicar la migración, no antes.

8.  **Acceso a datos:** crear en una carpeta Repositorios las clases
    EventoRepositorio, AsistenteRepositorio e InscripcionRepositorio,
    que reciben EventosContext por constructor y contienen todas las
    consultas LINQ y los SaveChangesAsync (también los datos de las
    listas desplegables). Registrarlas en Program.cs con AddScoped e
    inyectarlas en los controladores. Los controladores no usan
    EventosContext directamente.

9.  EventosController: Index (eventos activos con su categoría, filtro
    por categoría y orden por fecha, mostrando los inscritos confirmados
    y los cupos disponibles), Details, Create, Edit y Delete (lógico,
    con SaveChanges). Create y Edit tienen lista desplegable de
    categorías activas.

10. **Create, Edit y Delete:** la acción GET muestra la vista y la POST
    la procesa con ModelState.IsValid. Después de guardar se redirige
    (patrón Post/Redirect/Get) y el resultado se muestra con TempData.
    Las vistas son fuertemente tipadas.

11. AsistentesController: Index (asistentes activos ordenados por
    nombres) y Create. Si el DNI o el correo ya existe, mostrar el error
    en el formulario (ModelState.AddModelError).

12. InscripcionesController, acción Create: listas desplegables de
    eventos activos y asistentes activos. Validar con
    ModelState.AddModelError que el evento no haya pasado, que el
    asistente no esté ya inscrito (Confirmada) en ese evento y que el
    aforo no esté completo (inscripciones Confirmada menores que el
    Aforo); se guarda con Estado = \"Confirmada\". La acción Index lista
    las inscripciones con evento, asistente, fecha y estado, y permite
    cancelar (Estado = \"Cancelada\") las que están Confirmada.

13. Los métodos de los repositorios y las acciones de los controladores
    deben ser async (ToListAsync, AnyAsync, CountAsync,
    SaveChangesAsync). Las consultas de lectura usan AsNoTracking() y no
    hay SQL escrito a mano (no usar FromSqlRaw).

14. Modificar \_Layout.cshtml para incluir un menú con Eventos,
    Asistentes e Inscripciones.

**Migraciones (3 en total):**

15. **Migración 1, InitialCreate:** con las cuatro entidades y el
    contexto, crear la migración y aplicarla a la base de datos (ver
    \"Según tu editor\"). Comprobar en SQL Server que existen la base
    EventosDB, las cuatro tablas y la tabla \_\_EFMigrationsHistory.

16. **Migración 2, AgregarModalidadEvento:** agregar a Evento la
    propiedad Modalidad (obligatoria, máx. 20; valores Presencial o
    Virtual). Antes de aplicarla, en el archivo de la migración generada
    cambiar el valor por defecto (defaultValue) por \"Presencial\", para
    que los eventos ya registrados lo conserven. Mostrar la propiedad en
    Create y Edit (lista desplegable) y en Index del evento.

17. **Migración 3, AgregarTelefonoAsistente:** agregar a Asistente la
    propiedad Telefono (opcional, máx. 9 caracteres). Mostrarla en
    Create e Index de asistentes.

18. La carpeta Migrations debe quedar con **exactamente tres
    migraciones**, con los nombres indicados, tal como se aplicaron: no
    borrar ni renombrar archivos, no editarlos (salvo el defaultValue
    indicado) y no cambiar la base con SQL.

19. **Evidencia:** en SQL Server ejecutar la siguiente consulta sobre la
    base EventosDB y capturar el resultado (deben verse tres filas);
    capturar también la carpeta Migrations expandida. Los tres
    MigrationId de la tabla deben **coincidir** con los nombres de los
    archivos de la carpeta (aaaaMMddHHmmss_NombreDeLaMigración) y debe
    verse el archivo EventosContextModelSnapshot.cs.

> SELECT MigrationId, ProductVersion FROM \_\_EFMigrationsHistory ORDER
> BY MigrationId;

**Según tu editor:**

20. **Visual Studio:** agregar también el paquete
    Microsoft.EntityFrameworkCore.Tools y ejecutar en la Consola del
    Administrador de paquetes (reemplazar \<Nombre\> por el nombre de
    cada migración):

> **Add-Migration** \<Nombre\>
>
> **Update-Database**

21. **VS Code:** instalar una sola vez la herramienta dotnet-ef:

> **dotnet** tool install \--global dotnet-ef
>
> Y luego, dentro de la carpeta del proyecto, por cada migración:
>
> **dotnet** ef migrations add \<Nombre\>
>
> **dotnet** ef database update

22. Si SQL Server rechaza el certificado, agregar
    TrustServerCertificate=True a la cadena de conexión.


