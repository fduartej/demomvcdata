# Crear la migración

dotnet ef migrations add AgregarZonasInseguras

# Actualizar la base de datos

dotnet ef database update

# Ejecutar la aplicación

dotnet run

dotnet watch run

# Scaffold de Identity Pages

dotnet aspnet-codegenerator identity -dc demomvcdata.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.ForgotPassword;Account.ResetPassword"

dotnet aspnet-codegenerator identity -dc demomvcdata.Data.ApplicationDbContext --files "Account.Manage.Index;Account.Manage.Email;Account.Manage.ChangePassword"

# Sistema de Roles

## Roles Disponibles

- **Admin**: Acceso completo al panel de administración

## Asignar Rol Admin a un Usuario Existente

### Opción 1: Usando Scripts SQL (Recomendado)

# O ejecutar comandos individuales

sqlite3 app.db "INSERT OR IGNORE INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d', 'Admin', 'ADMIN', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d');"

sqlite3 app.db "INSERT OR IGNORE INTO AspNetUserRoles (UserId, RoleId) VALUES ('TU-USER-ID-AQUI', '1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d');"

```

### Panel de Admin

Los usuarios con rol "Admin" verán un menú desplegable "🔧 Admin Panel" con opciones adicionales:

- 👤 Gestionar Perfil
- 📧 Gestionar Email
- 🔒 Cambiar Contraseña


```
