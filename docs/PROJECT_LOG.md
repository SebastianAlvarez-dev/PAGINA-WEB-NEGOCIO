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
- Próximo paso: crear el pool de Workload Identity Federation para autenticar GitHub Actions sin llaves JSON.

## Estado actual

- Código almacenado en `SebastianAlvarez-dev/PAGINA-WEB-NEGOCIO`.
- Aplicación construida y validada localmente.
- CI configurado y aprobado.
- Configuración inicial de Google Cloud en curso.
- Despliegue de producción todavía pendiente.
