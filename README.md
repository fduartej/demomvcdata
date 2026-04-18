# Crear la migración

dotnet ef migrations add AgregarZonasInseguras

# Actualizar la base de datos

dotnet ef database update

# Ejecutar la aplicación

dotnet run

dotnet watch run

dotnet aspnet-codegenerator identity -dc demomvcdata.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.ForgotPassword;Account.ResetPassword"
