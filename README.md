# Ventify2 MVP

Sistema de gestión de inventario y ventas para comerciantes. MVP con registro, login, productos, ventas e IA.

## Stack

- **Backend**: .NET 8 (ASP.NET Core)
- **Frontend**: Angular 20.3
- **BD**: MySQL (Railway)
- **Deploy**: Railway (Backend) + Netlify (Frontend)

## Features MVP

✅ Registro de comerciantes (RF1)
✅ Login con JWT + Cookies (RF2)
✅ Agregar productos con precio/stock (RF3)
✅ Ver listado de productos (RF4)
✅ Registrar ventas con cálculo automático (RF6, RF7, RF9)
✅ Asistente IA para preguntas sobre ventas

## Quick Start Local

### Backend
```bash
cd back
dotnet restore
dotnet build
dotnet run
```
Visitá: http://localhost:5000/swagger

### Frontend
```bash
cd front
npm install
npm start
```
Visitá: http://localhost:4200

## Variables de Environment (Railway)

```
ConnectionStrings__DefaultConnection=Server=mysql.railway.internal;Port=3306;Database=railway;User Id=root;Password=xxx
JWT_SECRET=tu-clave-secreta-minimo-32-caracteres
JWT_EXPIRES_MINUTES=60
REFRESH_EXPIRES_DAYS=30
OPENROUTER_API_KEY=sk-or-v1-xxx (opcional)
```

## Deployment

Ver [DEPLOYMENT.md](DEPLOYMENT.md) para pasos completos Railway + Netlify.

## Licencia

Privado
