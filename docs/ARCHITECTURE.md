# Arquitectura

El backend de Faraluna Bisutería es un **monolito modular**: un proceso ASP.NET Core con módulos que conservan sus propios modelos, casos de uso y esquemas PostgreSQL. El frontend se compila y despliega de manera independiente para que ambos componentes puedan versionarse y escalar por separado.

```text
React SPA · Cloud Run `faraluna-web`
   │ HTTP/JSON
   ▼
ASP.NET Core REST API · Cloud Run `faraluna-api`
   ├── Catalog
   │   ├── Domain: Product, Category, Money
   │   ├── Application: Commands, Queries, ports
   │   └── Infrastructure: EF Core, PostgreSQL
   └── Reviews
       ├── Domain: Review, moderation
       ├── Application: Commands, Queries, ports
       └── Infrastructure: EF Core, PostgreSQL
            │
            ▼
       Supabase PostgreSQL

Supabase Auth ── valida administradores
Supabase Storage ── fotografías de productos
```

## Decisiones principales

- **DDD:** `Product`, `Category` y `Review` protegen sus invariantes dentro de los agregados.
- **CQRS:** cada escritura es un comando y cada lectura una consulta. `MessageDispatcher` no añade dependencias externas.
- **Clean Architecture:** Domain no conoce EF Core, HTTP ni Supabase; Application define puertos; Infrastructure los implementa; API compone el sistema.
- **Límites de módulos:** Catalog usa el esquema `catalog`; Reviews usa `reviews` y referencia productos únicamente por identificador.
- **REST:** los endpoints públicos están bajo `/api/catalog` y `/api/products`; las mutaciones están bajo `/api/admin`.
- **ORM:** EF Core + Npgsql administra mapeos y migraciones independientes por módulo.
- **Autorización:** el backend valida el access token contra Supabase Auth y solo acepta `app_metadata.role = admin`.
- **Fotos:** el navegador envía el archivo al backend; la clave secreta de Storage nunca llega al frontend.

## Extensión futura

Los pagos deben entrar como un módulo nuevo (`Orders`/`Payments`) que posea órdenes, ítems, estados, idempotencia y webhooks. No debe añadirse lógica de pago dentro de Catalog.
