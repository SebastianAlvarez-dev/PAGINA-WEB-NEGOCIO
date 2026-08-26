# Despliegue independiente con GitHub Actions

El pipeline crea dos imágenes y dos servicios de Cloud Run:

| Componente | Imagen | Servicio |
| --- | --- | --- |
| Frontend React + Nginx | `jewelry-store/faraluna-web` | `faraluna-web` |
| API ASP.NET Core | `jewelry-store/faraluna-api` | `faraluna-api` |

## Producción

| Componente | URL pública |
| --- | --- |
| Frontend | `https://faraluna-web-psg7hp34iq-uc.a.run.app` |
| Backend | `https://faraluna-api-psg7hp34iq-uc.a.run.app` |
| Salud del frontend | `https://faraluna-web-psg7hp34iq-uc.a.run.app/health` |
| Salud del backend | `https://faraluna-api-psg7hp34iq-uc.a.run.app/api/health` |

El primer despliegue completo verificado corresponde a la ejecución de GitHub Actions `32946084706` y al commit `8334ea8`.

## 1. Preparar Google Cloud

El proyecto exclusivo de producción para este sistema es:

| Dato | Valor |
| --- | --- |
| Nombre | `Faraluna Bisutería` |
| ID del proyecto | `faraluna-bisuteria` |
| Número del proyecto | `487549405508` |
| Región inicial | `us-central1` |

No se debe desplegar en el proyecto universitario `Landing Page`.

Selecciona el proyecto de Faraluna y habilita los servicios necesarios:

```bash
gcloud config set project faraluna-bisuteria
gcloud services enable run.googleapis.com artifactregistry.googleapis.com secretmanager.googleapis.com iamcredentials.googleapis.com sts.googleapis.com
gcloud artifacts repositories create jewelry-store --repository-format=docker --location=us-central1
```

Crea los secretos. Nunca guardes sus valores en Git ni los escribas directamente en el historial de la terminal. En este proyecto ya se crearon `faraluna-db-connection` y `faraluna-supabase-secret`, ambos con una primera versión habilitada.

La cuenta de servicio que ejecuta Cloud Run necesita `roles/secretmanager.secretAccessor` para leer ambos secretos. La cuenta de despliegue necesita ese mismo rol únicamente sobre `faraluna-db-connection`, porque el workflow lee la cadena para ejecutar las migraciones de Entity Framework Core.

Las identidades utilizadas son:

| Cuenta | Responsabilidad |
| --- | --- |
| `github-deployer@faraluna-bisuteria.iam.gserviceaccount.com` | Autenticar GitHub Actions y realizar despliegues |
| `faraluna-runtime@faraluna-bisuteria.iam.gserviceaccount.com` | Ejecutar los servicios de Cloud Run y leer secretos en producción |

## 2. Conectar GitHub mediante Workload Identity Federation

Crea una cuenta de servicio para despliegues y un proveedor OIDC restringido al repositorio `SebastianAlvarez-dev/PAGINA-WEB-NEGOCIO`. La identidad debe poder impersonar esa cuenta mediante `roles/iam.workloadIdentityUser`.

La cuenta de despliegue necesita, como mínimo:

- `roles/run.admin`
- `roles/artifactregistry.writer`
- `roles/secretmanager.secretAccessor`
- `roles/iam.serviceAccountUser` sobre `faraluna-runtime`

No uses una llave JSON de cuenta de servicio. El workflow solicita credenciales temporales mediante OIDC.

## 3. Variables del repositorio GitHub

En **Settings > Secrets and variables > Actions > Variables**, agrega:

| Variable | Ejemplo |
| --- | --- |
| `DEPLOY_ENABLED` | `false` durante la configuración; cambiar a `true` cuando todo esté listo |
| `GCP_PROJECT_ID` | `faraluna-bisuteria` |
| `GCP_WORKLOAD_IDENTITY_PROVIDER` | `projects/487549405508/locations/global/workloadIdentityPools/github/providers/faraluna-github` |
| `GCP_SERVICE_ACCOUNT` | `github-deployer@faraluna-bisuteria.iam.gserviceaccount.com` |
| `SUPABASE_URL` | `https://ptvnlzvskbphglqamips.supabase.co` |
| `WHATSAPP_NUMBER` | `593996359219` |

En **Settings > Secrets and variables > Actions > Secrets**, agrega:

| Secreto de GitHub | Uso |
| --- | --- |
| `SUPABASE_PUBLISHABLE_KEY` | Clave `sb_publishable_...` utilizada durante la compilación y el despliegue |

La clave publicable de Supabase no concede privilegios administrativos, aunque en este repositorio se guarda como secreto de GitHub para mantener una única configuración. La clave `sb_secret_...` permanece exclusivamente en Google Secret Manager y solo llega al backend.

El proyecto de Supabase se llama `faraluna-bisuteria-db`, su referencia pública es `ptvnlzvskbphglqamips` y está alojado en `us-east-1`.

La conexión PostgreSQL utiliza el Shared pooler en modo sesión:

| Parámetro | Valor |
| --- | --- |
| Host | `aws-0-us-east-1.pooler.supabase.com` |
| Puerto | `5432` |
| Base de datos | `postgres` |
| Usuario | `postgres.ptvnlzvskbphglqamips` |

La contraseña no se documenta. La cadena Npgsql completa se guarda en Google Secret Manager como `faraluna-db-connection`.

## 4. Ejecutar el pipeline

Todo pull request y push hacia `main` ejecuta CI. El CD solo se ejecuta cuando `DEPLOY_ENABLED` tiene exactamente el valor `true`. Debe permanecer en `false` hasta completar Google Cloud, Supabase y Secret Manager. También puedes iniciar el workflow manualmente desde **Actions > CI/CD > Run workflow**.

El orden del despliegue es:

1. Compilar y probar backend y frontend.
2. Aplicar migraciones EF Core contra PostgreSQL de Supabase.
3. Publicar y desplegar `faraluna-api`.
4. Compilar el frontend con el URL real del backend.
5. Publicar y desplegar `faraluna-web`.
6. Actualizar CORS del backend con el URL real del frontend.
7. Verificar `/api/health` y `/health`.

Los dominios personalizados pueden añadirse después en Cloud Run. Al hacerlo, agrega el dominio del frontend a `Cors:AllowedOrigins` del backend.
