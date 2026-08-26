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

## Diferencia entre imágenes Docker y fotografías

| Elemento | Qué contiene | Dónde se almacena |
| --- | --- | --- |
| Imagen Docker `faraluna-web` | Frontend compilado, Nginx y dependencias necesarias para ejecutarlo | Google Artifact Registry `jewelry-store` |
| Imagen Docker `faraluna-api` | API compilada, runtime de .NET y dependencias necesarias para ejecutarla | Google Artifact Registry `jewelry-store` |
| Fotografía de producto | Archivo JPG, PNG o WebP seleccionado por la administradora | Supabase Storage, bucket `product-images` |
| Datos del producto | Nombre, descripción, categoría, precio, stock y URL de la fotografía | PostgreSQL de Supabase mediante EF Core |
| Reseña | Autor, puntuación, comentario y estado de moderación | PostgreSQL de Supabase mediante EF Core |

Los archivos de fotografías no se guardan como datos binarios dentro de las tablas del catálogo. Supabase Storage conserva el archivo y PostgreSQL guarda la URL que relaciona el producto con su fotografía.

## Extensión futura

Los pagos deben entrar como un módulo nuevo (`Orders`/`Payments`) que posea órdenes, ítems, estados, idempotencia y webhooks. No debe añadirse lógica de pago dentro de Catalog.
