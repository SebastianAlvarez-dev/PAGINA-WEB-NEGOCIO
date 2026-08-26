# Despliegue independiente con GitHub Actions

El pipeline crea dos imágenes y dos servicios de Cloud Run:

| Componente | Imagen | Servicio |
| --- | --- | --- |
| Frontend React + Nginx | `jewelry-store/faraluna-web` | `faraluna-web` |
| API ASP.NET Core | `jewelry-store/faraluna-api` | `faraluna-api` |

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

Crea los secretos. Nunca guardes sus valores en Git:

```bash
printf '%s' 'CADENA_POSTGRES_DE_SUPABASE' | gcloud secrets create faraluna-db-connection --data-file=-
printf '%s' 'sb_secret_REEMPLAZAR' | gcloud secrets create faraluna-supabase-secret --data-file=-
```

La cuenta de servicio que ejecuta Cloud Run necesita `roles/secretmanager.secretAccessor` para leer ambos secretos.

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
| `SUPABASE_PUBLISHABLE_KEY` | `sb_publishable_...` |
| `WHATSAPP_NUMBER` | `593996359219` |

La clave publicable de Supabase se integra en el frontend y no concede privilegios administrativos. La clave `sb_secret_...` permanece exclusivamente en Secret Manager y solo llega al backend.

El proyecto de Supabase se llama `faraluna-bisuteria-db`, su referencia pública es `ptvnlzvskbphglqamips` y está alojado en `us-east-1`.

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
