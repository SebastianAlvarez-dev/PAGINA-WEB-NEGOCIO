# Faraluna Bisutería

Catálogo de bisutería con inventario, carrito por WhatsApp, reseñas moderadas y panel administrativo. Está construido como monolito modular con ASP.NET Core 10, React 19, DDD, CQRS, Clean Architecture, REST, EF Core y PostgreSQL de Supabase.

## Ejecutar localmente

La forma más rápida requiere Docker:

```powershell
docker compose up --build
```

Abre el frontend en `http://localhost:5173` y la API en `http://localhost:5080`. En `/admin`, usa **Entrar en modo local**. El modo local de administrador solo existe cuando `ASPNETCORE_ENVIRONMENT=Development`.

Para desarrollo con recarga rápida:

```powershell
docker compose up postgres -d
dotnet run --project backend/src/JewelryStore.Api --urls http://localhost:5080
```

En otra terminal:

```powershell
cd frontend
npm install
npm run dev
```

## Configurar Supabase

1. Crea un proyecto en Supabase.
2. En **Connect**, copia la cadena de conexión del Session Pooler (puerto 5432). Para Cloud Run limita el pool, por ejemplo:

```text
Host=aws-0-REGION.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.PROJECT_REF;Password=PASSWORD;SSL Mode=Require;Trust Server Certificate=true;Maximum Pool Size=10
```

3. Configura estas variables del backend:

```text
ConnectionStrings__Supabase=...
Supabase__Url=https://PROJECT_REF.supabase.co
Supabase__PublishableKey=sb_publishable_...
Supabase__SecretKey=sb_secret_...
Database__ApplyMigrations=true
```

4. Configura `frontend/.env` usando `frontend/.env.example`. Solo la clave publicable puede estar en el frontend.
5. Registra el usuario administrador en Supabase Auth. En el SQL Editor, asigna el rol usando su correo:

```sql
update auth.users
set raw_app_meta_data = coalesce(raw_app_meta_data, '{}'::jsonb)
  || '{"role":"admin"}'::jsonb
where email = 'TU_CORREO@DOMINIO.COM';
```

Cierra y vuelve a iniciar sesión para recibir un JWT actualizado.

## Migraciones

Las migraciones iniciales ya están versionadas. Para aplicarlas manualmente:

```powershell
$env:ConnectionStrings__Supabase='TU_CADENA'
dotnet tool restore
dotnet ef database update --project backend/src/Modules/Catalog/Catalog.Infrastructure --startup-project backend/src/JewelryStore.Api --context CatalogDbContext
dotnet ef database update --project backend/src/Modules/Reviews/Reviews.Infrastructure --startup-project backend/src/JewelryStore.Api --context ReviewsDbContext
```

En producción se recomienda aplicar migraciones como paso controlado y mantener `Database__ApplyMigrations=false` en Cloud Run.

## CI/CD y despliegue en Google Cloud

El frontend y el backend se publican como servicios independientes:

- `faraluna-web`: React servido por Nginx en Cloud Run.
- `faraluna-api`: API REST de ASP.NET Core en Cloud Run.

El workflow `.github/workflows/ci-cd.yaml` compila y prueba cada proyecto en pull requests y pushes. En `main`, cuando la variable `DEPLOY_ENABLED` es `true`, aplica las migraciones, publica dos imágenes en Artifact Registry y despliega ambos servicios. Mientras la configuración de producción esté incompleta, el valor `false` mantiene activo el CI y omite el despliegue de forma segura.

Consulta [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) para realizar la configuración inicial de Google Cloud, GitHub y Supabase. La arquitectura interna está documentada en [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

El historial de decisiones y avances se mantiene en [docs/PROJECT_LOG.md](docs/PROJECT_LOG.md).

## Verificación

```powershell
dotnet test JewelryStore.slnx
cd frontend
npm run build
```
