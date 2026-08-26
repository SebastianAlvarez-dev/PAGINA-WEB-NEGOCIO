# Bitácora del proyecto Faraluna Bisutería

Este documento registra las decisiones, entregas y pasos de configuración realizados durante el proyecto.

## 26 de agosto de 2026

### Aplicación y arquitectura

- Se construyó el catálogo web con frontend React y API ASP.NET Core.
- Se organizó el backend como monolito modular con Clean Architecture, DDD y CQRS.
- Se configuraron PostgreSQL, Entity Framework Core y la integración prevista con Supabase.
- Se añadieron catálogo, inventario, categorías, fotografías, reseñas, puntuaciones, comentarios, carrito por WhatsApp y panel administrativo.
- Se prepararon imágenes Docker independientes para frontend y backend.

### Automatización

- Se creó el workflow `.github/workflows/ci-cd.yaml` para CI/CD con GitHub Actions.
- El CI compila y prueba frontend y backend de manera independiente.
- El CD desplegará dos servicios de Cloud Run: `faraluna-web` y `faraluna-api`.
- El primer CI del repositorio terminó correctamente.
- Se configuraron `.gitignore` y `.dockerignore`.

### Google Cloud

- Se decidió no utilizar el proyecto universitario `Landing Page`.
- Se creó un proyecto exclusivo llamado `Faraluna Bisutería`.
- ID confirmado del proyecto: `faraluna-bisuteria`.
- Cloud Shell quedó abierto con el proyecto correcto seleccionado.
- Se habilitaron correctamente las APIs de Cloud Run, Artifact Registry, Secret Manager, IAM Credentials y Security Token Service.
- Se creó el repositorio Docker `jewelry-store` en Artifact Registry, región `us-central1`.
- Se creó la cuenta de servicio `github-deployer@faraluna-bisuteria.iam.gserviceaccount.com` para el despliegue automatizado.
- Se creó la cuenta `faraluna-runtime@faraluna-bisuteria.iam.gserviceaccount.com` para ejecutar los servicios de Cloud Run.
- Se actualizó el workflow para asignar explícitamente `faraluna-runtime` al frontend y al backend.
- Se concedió a `github-deployer` el rol `roles/run.admin` para administrar los despliegues de Cloud Run.
- Se concedió a `github-deployer` el rol `roles/artifactregistry.writer`, limitado al repositorio `jewelry-store`.
- Se concedió a `github-deployer` el rol `roles/iam.serviceAccountUser` sobre `faraluna-runtime`.
- Se creó el pool global de Workload Identity Federation `github` para autenticar GitHub Actions sin llaves JSON.
- Se creó el proveedor OIDC `faraluna-github`, configurado para restringir la confianza al repositorio de Faraluna y a la rama `main`.
- Se verificó que el proveedor está `ACTIVE`, usa el emisor oficial de GitHub y contiene los atributos y la condición de seguridad esperados.
- Número confirmado del proyecto de Google Cloud: `487549405508`.
- Se autorizó al repositorio federado para suplantar temporalmente a `github-deployer` mediante `roles/iam.workloadIdentityUser`.
- Se añadió la variable de control `DEPLOY_ENABLED` al workflow para impedir despliegues incompletos.
- Se guardaron en GitHub `GCP_PROJECT_ID`, `GCP_WORKLOAD_IDENTITY_PROVIDER` y `GCP_SERVICE_ACCOUNT`.
- `DEPLOY_ENABLED` quedó en `false` hasta completar Supabase y Secret Manager.

### Supabase

- Se creó el proyecto `faraluna-bisuteria-db` en la región general `Americas`.
- Se confirmó la asignación exacta a `us-east-1` (East US, North Virginia), con referencia pública `ptvnlzvskbphglqamips`.
- URL pública del proyecto: `https://ptvnlzvskbphglqamips.supabase.co`.
- Se dejó sin conectar la integración GitHub de Supabase porque el esquema se desplegará mediante EF Core y nuestro workflow existente.
- Se rotó la contraseña inicial de PostgreSQL después de que apareciera en una captura; el nuevo valor no se registró en el chat, Git ni la documentación.
- Se verificó que el proyecto dispone de una clave `sb_publishable_...` y una clave `sb_secret_...` modernas, ambas con nombre `default`.
- La clave secreta permaneció enmascarada y no se registró en capturas, chat, Git ni documentación.
- La clave publicable se guardó como secreto del repositorio GitHub con el nombre `SUPABASE_PUBLISHABLE_KEY`.
- Se adaptó el workflow para leer la clave publicable desde el contexto `secrets` de GitHub Actions.
- Se creó `faraluna-supabase-secret` en Google Secret Manager con una versión habilitada.
- Se verificó sin imprimir su contenido que el valor almacenado tiene el formato `sb_secret_...` esperado.
- Próximo paso: conceder a `faraluna-runtime` acceso de lectura únicamente sobre este secreto.

## Estado actual

- Código almacenado en `SebastianAlvarez-dev/PAGINA-WEB-NEGOCIO`.
- Aplicación construida y validada localmente.
- CI configurado y aprobado.
- Configuración inicial de Google Cloud en curso.
- Despliegue de producción todavía pendiente.
