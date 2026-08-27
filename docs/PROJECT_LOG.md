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
- Se concedió a `faraluna-runtime` el rol `roles/secretmanager.secretAccessor`, limitado a `faraluna-supabase-secret`.
- Se confirmó la conexión Session pooler: `aws-0-us-east-1.pooler.supabase.com:5432`, base `postgres`, usuario `postgres.ptvnlzvskbphglqamips`.
- Se guardó la cadena Npgsql completa en Google Secret Manager como `faraluna-db-connection`, versión `1`, sin imprimir ni registrar la contraseña.
- Se concedió a `faraluna-runtime` el rol `roles/secretmanager.secretAccessor`, limitado a `faraluna-db-connection`.
- Se concedió a `github-deployer` el rol `roles/secretmanager.secretAccessor`, limitado a `faraluna-db-connection`, para ejecutar las migraciones del pipeline.
- Se verificó que `github-deployer` no necesita acceso a `faraluna-supabase-secret`: el workflow solo referencia ese secreto y Cloud Run lo entrega mediante `faraluna-runtime`.
- Se configuró en GitHub la variable `WHATSAPP_NUMBER=593996359219`; quedaron completas las seis variables requeridas por el workflow.
- Se revisó el workflow contra la documentación vigente de las acciones oficiales de Google: autenticación OIDC y despliegue a Cloud Run usan la sintaxis actual.
- La validación local final terminó correctamente: backend sin advertencias ni errores y 5/5 pruebas aprobadas; frontend sin vulnerabilidades reportadas, TypeScript válido y compilación de producción correcta.
- Se confirmó que los últimos cinco CI ejecutados en GitHub terminaron correctamente.
- La construcción local de imágenes no se ejecutó porque Docker Desktop no estaba iniciado; GitHub Actions realizará esta prueba en su runner durante el primer CD.
- Se activó `DEPLOY_ENABLED=true` y se inició manualmente el primer CD (`CI/CD #26`).
- El frontend CI y el backend CI aprobaron, al igual que la autenticación OIDC y el acceso a Artifact Registry.
- El primer CD se detuvo antes de publicar recursos porque el runner independiente del job de despliegue no había restaurado los paquetes NuGet requeridos por `dotnet ef`; no fue un error de credenciales ni de Supabase.
- Se añadió al job de despliegue una restauración explícita de `JewelryStore.slnx` antes de ejecutar las migraciones.
- El segundo CD (`32946084706`) terminó correctamente en 3 minutos y 10 segundos: aplicó ambas migraciones, publicó las dos imágenes, desplegó los dos servicios, actualizó CORS y aprobó los chequeos de salud.
- Frontend de producción: `https://faraluna-web-psg7hp34iq-uc.a.run.app`.
- Backend de producción: `https://faraluna-api-psg7hp34iq-uc.a.run.app`.
- Se realizó una verificación independiente posterior: API y frontend saludables, conexión a PostgreSQL operativa, catálogo consultable y CORS limitado al origen del frontend.
- El catálogo de producción devuelve una lista vacía hasta que el administrador cargue los primeros productos reales.
- Se creó y confirmó el primer usuario administrativo en Supabase Auth mediante el proveedor Email; su contraseña no se registró en el chat, Git ni la documentación.
- Se asignó `app_metadata.role = admin` mediante la API administrativa desde Cloud Shell, leyendo la clave secreta directamente desde Google Secret Manager.
- Se preservaron los metadatos del proveedor y se verificó la respuesta administrativa sin revelar secretos.
- Se verificó en producción el inicio de sesión completo: Supabase autenticó al usuario, el token incluyó el rol y la API autorizó el panel administrativo.
- Se probó desde el panel administrativo la gestión de categorías, productos, stock y fotografías.
- Se confirmó que los archivos de producto se almacenan en Supabase Storage y que PostgreSQL conserva la URL asociada al producto.

### Rediseño de la tienda

- Se renovó la portada con una identidad más juvenil basada en morado, rosa, crema y dorado, manteniendo la estética de Faraluna.
- Se incorporaron tres piezas gráficas oficiales: conjuntos de corazones rojos y verdes en el hero, y una campaña de cadenas doradas en una sección editorial.
- Se actualizó el mensaje principal, los llamados a la acción y la presentación de categorías sin cambiar la lógica existente del catálogo ni del carrito por WhatsApp.
- Se añadieron estados de foco visibles y nombres accesibles para los controles móviles del menú y del carrito.
- Se validaron TypeScript, la compilación de producción, la carga de todas las imágenes y el comportamiento responsive del menú, carrito, portada y catálogo.
- El commit `1dd5c3d` activó el CI/CD `33014487708`: frontend, backend, migraciones, contenedores, despliegues y chequeos de salud terminaron correctamente.
- Se verificó la nueva portada directamente en Cloud Run, incluyendo el texto principal y la carga completa de las cuatro imágenes visibles.
- Se conectó Figma mediante MCP y se analizaron las cuatro interfaces de Quiana Joyería (`Inicio`, `Productos`, `Sobre Quiana` y `Contáctanos`) junto con la interfaz móvil de Lunara Joyería.
- Se definió una adaptación propia para Faraluna: Quiana como referencia editorial principal, un concepto lunar inspirado en Lunara y contenido, fotografías y funcionalidades originales de Faraluna.
- Se creó una marca lunar propia mediante CSS, compuesta por una media luna y destellos; no se copiaron logotipos ni recursos gráficos de las plantillas de terceros.
- Se rediseñaron la navegación, portada, manifiesto editorial, categorías, tarjetas de producto, campaña, historia, contacto, pie de página, catálogo, detalle, reseñas, carrito y panel administrativo.
- Se incorporaron las tipografías `Italiana`, `Parisienne` y `DM Sans`, con una paleta unificada en morado profundo, crema, rosa y dorado.
- Se conservaron sin cambios la integración con Supabase, el stock, las reseñas, la carga de imágenes, la autenticación administrativa y el pedido por WhatsApp.
- Se corrigió la degradación del catálogo cuando la API local no está disponible, evitando un rechazo no controlado durante la carga de categorías.
- La validación posterior aprobó la compilación de producción del frontend y las 5/5 pruebas del backend; la revisión visual confirmó escritorio y móvil sin imágenes rotas ni desbordamiento horizontal, además del funcionamiento del menú y el carrito.
- El commit `3991120` activó el CI/CD `33036165838`; los CI de frontend y backend, las migraciones, la publicación de contenedores, los dos despliegues y los chequeos de salud terminaron correctamente.
- Se verificó la URL pública después del despliegue: Cloud Run muestra el título `Brilla tu esencia cada día`, la nueva marca lunar y todas las imágenes cargan correctamente.
- Tras la primera revisión del diseño se redujo y reubicó la luna decorativa del hero para evitar que compita con el logotipo incluido en la fotografía principal.
- Se sustituyeron los cuatro símbolos tipográficos de categorías por iconos vectoriales propios de cadenas, pulseras, aretes y anillos, con trazos morados y detalles dorados.
- Se añadieron fondos diferenciados a las categorías y más color a catálogo, detalle de producto, reseñas y cabecera administrativa, conservando la portada aprobada.
- La nueva iteración volvió a aprobar la compilación del frontend y las 5/5 pruebas del backend; escritorio y móvil se validaron sin imágenes rotas ni desbordamiento horizontal.
- El commit `aaef9e3` activó el CI/CD `33037287979`; frontend, backend, migraciones de Supabase, publicación de contenedores y despliegues en Cloud Run terminaron correctamente.
- Se comprobó la versión pública final: se muestran los cuatro iconos vectoriales nuevos y la luna corregida, sin imágenes rotas ni desbordamiento horizontal.
- Se simplificó la experiencia pública para evitar acciones redundantes: la navegación principal quedó limitada a `Inicio` y `Catálogo`, manteniendo el carrito como acceso al pedido.
- Se retiraron de la portada la campaña secundaria, la historia, el bloque de promesas y la llamada adicional a WhatsApp; también se dejó un único botón principal `Ver catálogo` y se humanizó el estado de colección vacía.
- La versión simplificada aprobó la compilación del frontend, las 5/5 pruebas del backend y la revisión responsive sin imágenes rotas ni desbordamiento horizontal.

## Estado actual

- Código almacenado en `SebastianAlvarez-dev/PAGINA-WEB-NEGOCIO`.
- Aplicación construida y validada localmente.
- CI configurado y aprobado.
- Google Cloud, Supabase, Secret Manager y GitHub Actions configurados.
- Frontend y backend desplegados independientemente en Cloud Run y verificados en producción.
- Catálogo, autenticación administrativa, carga de fotografías y almacenamiento en Supabase probados.
