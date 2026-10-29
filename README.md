# FSINCA EVENTS - Sistema de Gestión de Eventos e Inscripciones
### Desarrollo de Aplicaciones Empresariales Avanzadas (DAEA) - Laboratorio 11
**Institución:** TECSUP | Ciclo 6 - C24 (Sección C - D)  
**Estudiante:** Frank Sinca  
**Docente:** Arévalo Sermeño, Edwin William  
**Repositorio GitHub:** [https://github.com/Dorian-devw/DAEA-lab11.git](https://github.com/Dorian-devw/DAEA-lab11.git)

---

## 📌 Descripción del Proyecto

Aplicación web empresarial desarrollada con **ASP.NET Core MVC en .NET 10** y **Entity Framework Core (Code First)** para la administración y venta/inscripción de eventos especiales, conferencias y talleres. 

La base de datos se genera y actualiza exclusivamente a través de **migraciones de EF Core**, sin recurrir a scripts SQL manuales ni procedimientos almacenados, utilizando LINQ para todas las consultas y el **Patrón Repositorio** para garantizar una arquitectura limpia y desacoplada.

---

## 🚀 Tecnologías y Herramientas

* **Plataforma:** .NET 10 (C# 14)
* **Framework Web:** ASP.NET Core MVC (.NET 10.0)
* **ORM:** Entity Framework Core 10.0.14
  * `Microsoft.EntityFrameworkCore.SqlServer`
  * `Microsoft.EntityFrameworkCore.Design`
  * `Microsoft.EntityFrameworkCore.Tools`
* **Base de Datos:** Microsoft SQL Server (SQLEXPRESS)
* **Frontend:** HTML5, CSS3, Bootstrap 5.3, Bootstrap Icons (embebido de forma local)
* **Control de Versiones:** Git & GitHub

---

## 🏗️ Modelo de Datos y Entidades

Todas las entidades están modeladas en la carpeta `Models` y configuradas **únicamente mediante Data Annotations** (sin sobrescribir `OnModelCreating` ni Fluent API):

| Entidad | Propiedades Principales | Restricciones y Data Annotations |
| :--- | :--- | :--- |
| **Categoria** | `Id`, `Nombre`, `Activo` | `[Index(nameof(Nombre), IsUnique = true)]`, `[Required]`, `[StringLength(50)]`, borrado lógico (`Activo = false`). |
| **Evento** | `Id`, `Titulo`, `Fecha`, `Lugar`, `Aforo`, `Precio`, `Modalidad`, `CategoriaId`, `Activo` | `[Required]`, `[StringLength(100)]`, `[Range(1, 5000)]` para Aforo, `[Column(TypeName = "decimal(10,2)")]`, `[Range(0, ...)]` para Precio, `Modalidad` obligatoria (Presencial/Virtual), borrado lógico. |
| **Asistente** | `Id`, `Nombres`, `Dni`, `Correo`, `Telefono`, `Activo` | `[Index]` único para `Dni` (8 dígitos numéricos) y `Correo` (`[EmailAddress]`), `Telefono` opcional (máx. 9 car.), borrado lógico. |
| **Inscripcion** | `Id`, `EventoId`, `AsistenteId`, `FechaInscripcion`, `Estado` | Claves foráneas a Evento y Asistente. `Estado` ("Confirmada" o "Cancelada"). Cancelación lógica sin borrado físico. |

---

## 📦 Secuencia de Migraciones (Exactamente 3)

El proyecto contiene el historial exacto de 3 migraciones aplicadas de manera secuencial en SQL Server:

```
Eventos.Web/Migrations/
├── 20261028214512_InitialCreate.cs
├── 20261028214512_InitialCreate.Designer.cs
├── 20261028214746_AgregarModalidadEvento.cs
├── 20261028214746_AgregarModalidadEvento.Designer.cs
├── 20261028215215_AgregarTelefonoAsistente.cs
├── 20261028215215_AgregarTelefonoAsistente.Designer.cs
└── EventosContextModelSnapshot.cs
```

### 1. Migración 1: `InitialCreate`
* **Objetivo:** Creación inicial de la base de datos `EventosDB` y las 4 tablas base (`Categorias`, `Eventos`, `Asistentes`, `Inscripciones`) con sus índices únicos y claves foráneas.
* **Comandos:**
  ```powershell
  dotnet ef migrations add InitialCreate --project Eventos.Web
  dotnet ef database update --project Eventos.Web
  ```

### 2. Migración 2: `AgregarModalidadEvento`
* **Objetivo:** Adición de la columna obligatoria `Modalidad` (máx. 20 caracteres) a `Eventos`.
* **Modificación obligatoria:** Antes de aplicar la migración a la base de datos, se editó el archivo generado para establecer `defaultValue: "Presencial"`, asegurando la integridad de los registros existentes.
* **Comandos:**
  ```powershell
  dotnet ef migrations add AgregarModalidadEvento --project Eventos.Web
  dotnet ef database update --project Eventos.Web
  ```

### 3. Migración 3: `AgregarTelefonoAsistente`
* **Objetivo:** Incorporación de la columna opcional `Telefono` (máx. 9 caracteres, nullable) a la entidad `Asistentes`.
* **Comandos:**
  ```powershell
  dotnet ef migrations add AgregarTelefonoAsistente --project Eventos.Web
  dotnet ef database update --project Eventos.Web
  ```

### 🔍 Evidencia en SQL Server
Al consultar la tabla de control de migraciones en la base de datos:
```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```
**Resultado:**
```text
MigrationId                              ProductVersion
---------------------------------------- ----------------
20261028214512_InitialCreate             10.0.14
20261028214746_AgregarModalidadEvento    10.0.14
20261028215215_AgregarTelefonoAsistente  10.0.14
```

---

## 🏛️ Arquitectura del Sistema

El sistema implementa una arquitectura modular desacoplada:

```
DAEA-sem11/
├── Eventos.Web/
│   ├── Controllers/
│   │   ├── EventosController.cs       # CRUD de eventos, filtros por categoría y métricas
│   │   ├── AsistentesController.cs    # Listado y registro con validación DNI/Correo
│   │   ├── InscripcionesController.cs # Control de aforo, estados y cancelación
│   │   └── HomeController.cs          # Redirección y alternador de rol (Asistente/Admin)
│   ├── Data/
│   │   └── EventosContext.cs          # DbContext (DbSets sin Fluent API)
│   ├── Models/
│   │   ├── Categoria.cs
│   │   ├── Evento.cs
│   │   ├── Asistente.cs
│   │   └── Inscripcion.cs
│   ├── Repositorios/
│   │   ├── EventoRepositorio.cs       # LINQ con AsNoTracking(), filtros y SaveChangesAsync
│   │   ├── AsistenteRepositorio.cs    # Verificaciones de unicidad y operaciones async
│   │   └── InscripcionRepositorio.cs  # Validación de aforo y estados
│   ├── Views/
│   │   ├── Eventos/                   # Catálogo de tarjetas, Detalle, Crear, Editar, Eliminar
│   │   ├── Asistentes/                # Directorio y formulario de registro
│   │   ├── Inscripciones/             # Control de inscripciones y cancelaciones
│   │   └── Shared/
│   │       └── _Layout.cshtml         # Barra de navegación adaptativa y sticky footer
│   ├── wwwroot/                       # Recursos estáticos locales (imágenes e íconos)
│   ├── appsettings.json               # Cadena de conexión a SQL Server
│   └── Program.cs                     # Inyección de dependencias y semillero de datos
├── .gitignore
|
└── README.md
```

### Principios y Patrones Aplicados:
1. **Patrón Repositorio:** Los controladores nunca inyectan directamente el `EventosContext`. Todos los métodos son asíncronos (`ToListAsync`, `AnyAsync`, `CountAsync`, `SaveChangesAsync`) y las lecturas usan `.AsNoTracking()`.
2. **Post / Redirect / Get (PRG) & TempData:** Tras procesar un formulario exitosamente, se redirige a la vista correspondiente mostrando notificaciones contextuales (`TempData["Exito"]`).
3. **Vistas Fuertemente Tipadas:** Formularios y tablas enlazadas directamente a los modelos con validaciones cliente/servidor (`ModelState.IsValid`).
4. **Semillero de Datos (Seed Data):** Al arrancar la aplicación en `Program.cs`, se verifica con `AnyAsync()` si existen datos. Solo si la tabla está vacía, se registran **3 categorías, 4 eventos y 5 asistentes**.

---

## 🎨 Características de la Interfaz y Roles

* **Diseño Inspirado en la Maqueta:**
  * Logotipo `fsincaevents`.
  * Título central **FSINCA EVENTS** y subtítulo **REUNIONES ESPECIALES**.
  * Sección en fondo amarillo con **CATEGORIAS** filtrables (*TODOS*, *Marketing & Publicidad*, *Emprendimiento*, *Fiestas & Conciertos*).
  * Tarjetas en cuadrícula de 3 columnas con fotografías reales de alta resolución, badge de modalidad, precio, cupos libres e inscritos confirmados.
* **Modo Asistente (Público):**
  * Vista limpia y comercial orientada al cliente.
  * Solo muestra el catálogo de eventos y el botón **Inscribirme**.
  * Oculta los accesos administrativos de creación, edición y borrado.
* **Modo Administrador (Evaluación):**
  * Activado con el botón `⚙️ Modo Admin`.
  * Muestra la barra superior de estado y habilita las opciones CRUD completas (**Nuevo Evento**, **Asistentes**, **Inscripciones**, **Editar** y **Eliminar** lógico).

---

## ⚙️ Guía de Instalación y Ejecución Paso a Paso

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Dorian-devw/DAEA-lab11.git
cd DAEA-lab11
```

### 2. Configurar la Cadena de Conexión
Verificar en `Eventos.Web/appsettings.json` el nombre de la instancia local de SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EventosDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Pooling=true;Connect Timeout=5;"
  }
}
```

### 3. Aplicar las Migraciones a la Base de Datos
Desde la terminal:
```powershell
dotnet ef database update --project Eventos.Web
```
*(Esto creará la base de datos `EventosDB` y sus tablas en SQL Server de manera automática).*

### 4. Ejecutar la Aplicación Web
Entrar a la carpeta del proyecto y ejecutar:
```powershell
cd Eventos.Web
dotnet run
```
*(O ejecutar con recarga en caliente usando `dotnet watch`).*

### 5. Abrir en el Navegador
La consola indicará el puerto local activo:
👉 **[http://localhost:5292](http://localhost:5292)**

---

