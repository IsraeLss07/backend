# Despliegue de la API

## Resumen

La aplicación está lista para producción. Este documento explica como subir todo a la nube:
- Base de datos en Azure SQL
- API en Railway

## Base de Datos en Azure

### Configuración inicial

Necesitas crear una base de datos SQL en Azure. El proceso es bastante directo:

1. Entra al portal de Azure
2. Crea una nueva SQL Database 
3. Elige el tier Serverless para mantener costos bajos
4. Configura las reglas de firewall para permitir conexiones

Para el servidor recomiendo usar autenticación SQL con un usuario admin. El pricing tier Serverless es perfecto para desarrollo porque solo pagas cuando se usa.

La configuración típica que funciona bien:
- 0.5-1 vCores
- 3GB memoria máxima  
- Auto-pause después de 1 hora sin uso

### Configuración de acceso

Una vez creada la DB, hay que configurar el firewall. Lo más sencillo es permitir servicios de Azure y agregar tu IP actual. Para Railway temporalmente puedes abrir el rango completo de IPs, aunque no es lo más seguro.

El connection string lo obtienes desde la configuración de la base de datos. Para este proyecto se ve así:

```
Server=tcp:testing-sv.database.windows.net,1433;Initial Catalog=testing-db;Persist Security Info=False;User ID=israel123;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### Configurar las tablas

Usa el Query Editor de Azure para ejecutar los scripts SQL que están en la carpeta `sql/`. Primero el schema para crear las tablas, después el seed para cargar algunos datos de prueba.

Los scripts ya están listos, solo córrelos en orden. El Entity Framework ya tiene configurado el modelo Product que coincide con la estructura de la tabla.

## Backend en Railway

### Preparación

El código ya está listo para Railway. La configuración de Kestrel en Program.cs maneja el puerto dinámico que asigna Railway.

Lo único que necesitas es subir el código a un repo de GitHub y conectarlo con Railway. Ahí configuras las variables de entorno, principalmente el connection string de la base de datos.

### Proceso de despliegue

1. Crea una cuenta en Railway con tu GitHub
2. Conecta tu repositorio 
3. Railway detecta automáticamente que es un proyecto .NET
4. Configura las variables de entorno:
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `ConnectionStrings__DefaultConnection`: Server=tcp:testing-sv.database.windows.net,1433;Initial Catalog=testing-db;Persist Security Info=False;User ID=israel123;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

Railway maneja el build automáticamente. Detecta el .csproj y hace el publish. Solo asegúrate de que el connection string esté bien configurado.

**Importante**: La variable de entorno debe llamarse exactamente `ConnectionStrings__DefaultConnection` (con doble guión bajo) para que .NET la reconozca correctamente.

### Verificación

Una vez desplegado, puedes probar los endpoints:
- `/` devuelve un mensaje de que está funcionando
- `/api/products` devuelve la lista de productos
- `/swagger` para ver la documentación

## Configuración final

### Frontend
Cambia la URL en `ap.js` para que apunte a tu dominio de Railway:
```javascript
const API_BASE_URL = 'https://tu-app.railway.app/api';
```

### CORS
El CORS ya está configurado para permitir cualquier origen en desarrollo. Para producción deberías especificar el dominio exacto de tu frontend.

### Seguridad
Railway maneja HTTPS automáticamente. Para la base de datos, considera usar reglas de firewall más específicas una vez que tengas todo funcionando.

## Costos aproximados

Azure SQL en modo Serverless sale alrededor de $5-15 por mes dependiendo del uso. Railway tiene plan gratuito con 500 horas mensuales, que debería ser suficiente para desarrollo.

## Troubleshooting

Los errores más comunes suelen ser:

**Conexión a base de datos**: Revisar el firewall de Azure y el connection string
**Build en Railway**: Verificar que el .csproj esté bien configurado  
**CORS**: Asegurarse de que los origins estén permitidos en Program.cs

## Notas finales

El proceso completo toma un par de horas la primera vez. Una vez que tienes todo configurado, los despliegues posteriores son automáticos cuando pusheas código.

Railway detecta los cambios en el repo y rebuilds automáticamente. Azure SQL se mantiene siempre disponible y solo pagas por el tiempo que realmente se usa con el tier Serverless.