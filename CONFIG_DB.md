# Configuración Conexión a la Base de Datos

Este proyecto utiliza archivos de configuración separados para mantener segura la información sensible de conexión a la base de datos.

## Configuración para Desarrollo

1. **Crea tu archivo de configuración local:**
   - Copia `appsettings.example.json` a `appsettings.Development.json`
   - O crea `appsettings.Development.json` con tu cadena de conexión real

2. **Configura tu cadena de conexión:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=TU_BASE_DATOS;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

## Tipos de Cadenas de Conexión

### Windows Authentication (Para desarrollo local)
```
Server=NOMBRE_SERVIDOR;Database=NOMBRE_BD;Integrated Security=True;TrustServerCertificate=True;
```

### SQL Server Authentication (Usuario y Contraseña)
```
Server=NOMBRE_SERVIDOR;Database=NOMBRE_BD;User Id=USUARIO;Password=CONTRASEÑA;TrustServerCertificate=True;
```

### LocalDB (Base de datos local de desarrollo)
```
Server=(localdb)\\mssqllocaldb;Database=NOMBRE_BD;Integrated Security=True;
```
```
