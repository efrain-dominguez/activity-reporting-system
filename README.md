# Activity Reporting System



Sistema de seguimiento y reporte de actividades con arquitectura limpia para gestión de proyectos universitarios.



## 🚀 Tecnologías



### Backend

\- \*\*.NET 9\*\* - Framework principal

\- \*\*MongoDB\*\* - Base de datos NoSQL

\- \*\*Clean Architecture\*\* - Patrón de arquitectura

\- \*\*FluentValidation\*\* - Validación de datos

\- \*\*Docker\*\* - Containerización



### Frontend (En desarrollo)

\- \*\*Angular 19\*\* - Framework frontend

\- \*\*Tailwind CSS\*\* - Estilos

\- \*\*Microsoft Entra ID\*\* - Autenticación



\## 📋 Características



\- ✅ Gestión de usuarios con roles (Admin, PMO, Entity, Directorate, Team)

\- ✅ Creación y seguimiento de solicitudes de reporte

\- ✅ Sistema de notificaciones en tiempo real

\- ✅ Estructura jerárquica de entidades

\- ✅ Workflow de aprobación de actividades

\- ✅ RESTful API con Swagger

\- ✅ Validación robusta de datos



\## 🏗️ Arquitectura

```

backend/

├── ARS.Domain/          # Entidades y lógica de negocio

├── ARS.Application/     # DTOs, Validators, Interfaces

├── ARS.Infrastructure/  # Repositorios, MongoDB, Datos

└── ARS.API/            # Controllers, Middleware

```



\## 🛠️ Configuración Local



\### Prerequisitos

\- .NET 9 SDK

\- Docker Desktop

\- Visual Studio 2022 o VS Code



\### Pasos



1\. \*\*Clonar el repositorio\*\*

```bash

git clone https://github.com/efrain-dominguez/activity-reporting-system.git

cd activity-reporting-system/backend

```



2\. \*\*Levantar MongoDB con Docker\*\*

```bash

docker-compose up -d

```



3\. \*\*Restaurar paquetes NuGet\*\*

```bash

dotnet restore

```



4\. \*\*Ejecutar la aplicación\*\*

```bash

dotnet run --project ARS.API

```



5\. \*\*Acceder a Swagger\*\*

```

https://localhost:7xxx/swagger

```



6\. \*\*Mongo Express (UI para MongoDB)\*\*

```

http://localhost:8081

```



\## 📊 Modelos de Datos



\- \*\*User\*\* - Usuarios del sistema con roles

\- \*\*Entity\*\* - Entidades organizacionales (jerárquicas)

\- \*\*TrackingRequest\*\* - Solicitudes de seguimiento

\- \*\*Notification\*\* - Sistema de notificaciones

\- \*\*RequestAssignment\*\* - Asignaciones de solicitudes (En desarrollo)

\- \*\*Activity\*\* - Actividades reportadas (En desarrollo)

\- \*\*Review\*\* - Revisiones y aprobaciones (En desarrollo)



\## 🔐 Autenticación



\- Microsoft Entra ID (En desarrollo)

\- JWT Tokens

\- Role-based authorization



\## 👤 Autor



\*\*Efraín Domínguez Goycochea\*\*

\- GitHub: \[@efrain-dominguez](https://github.com/efrain-dominguez)

\- LinkedIn: \[Tu LinkedIn]

\- Email: efrain.dominguez.goycochea@gmail.com



\## 📝 Estado del Proyecto



🚧 \*\*En desarrollo activo\*\* - Backend funcional, Frontend en progreso



\---



⭐ Si este proyecto te resulta útil, considera darle una estrella en GitHub

