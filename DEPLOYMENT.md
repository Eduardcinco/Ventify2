# Ventify2 MVP - Deployment Ready

## 🚀 Antes de subir a GitHub

### 1️⃣ **BD - Ejecuta el script SQL**
Copia el contenido de `SCHEMA.sql` y ejecútalo en tu MySQL de Railway en la BD `railway`:
```bash
# En Railway MySQL:
mysql -h mysql.railway.internal -u root -p -e "source SCHEMA.sql"
```
O ejecutalo manualmente por PHPMyAdmin/MySQL Workbench.

### 2️⃣ **Backend - Variables de Environment**
En Railway (tu backend), añade estas variables:

```
ConnectionStrings__DefaultConnection=Server=mysql.railway.internal;Port=3306;Database=railway;User Id=root;Password=kxDTqjEKMUiUQoCOTMcepoQjCnjp7JUdP;
JWT_SECRET=tu-clave-super-secreta-de-minimo-32-caracteres-aleatorios-aqui
JWT_EXPIRES_MINUTES=60
REFRESH_EXPIRES_DAYS=30
OPENROUTER_API_KEY=sk-or-v1-xxxxxxxxxxxxx (opcional, solo si quieres chat IA)
ASPNETCORE_ENVIRONMENT=Production
```

### 3️⃣ **Frontend - Actualiza URL del Backend**
En `front/src/environments/environment.prod.ts`, cambia:
```typescript
apiUrl: 'https://tu-backend-railway.railway.app/api'
```

### 4️⃣ **Dockerfile Backend** (para Railway)
Crea en `back/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "VentifyAPI.dll"]
```

### 5️⃣ **Netlify - netlify.toml para el Front**
Crea en `front/netlify.toml`:
```toml
[build]
  command = "npm run build"
  publish = "dist/browser"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
```

---

## 📦 Estructura para GitHub

```
Ventify2/
├── back/                 # Backend .NET
│   ├── VentifyAPI.csproj
│   ├── Program.cs
│   ├── Startup.cs
│   ├── appsettings.json  ⚠️ (actualizado con tu BD)
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── DTOs/
│   ├── Data/
│   └── Dockerfile        (crear)
├── front/                # Frontend Angular
│   ├── src/
│   ├── angular.json
│   ├── package.json
│   ├── tsconfig.json
│   ├── src/environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts ⚠️ (actualizar URL backend)
│   └── netlify.toml      (crear)
├── SCHEMA.sql            ✅ (ya creado)
├── README.md             (crear)
└── .gitignore            (crear)
```

---

## ✅ Checklist Final Antes de Push a GitHub

- [ ] Ejecuté SCHEMA.sql en Railway
- [ ] Variables de Environment en Backend Railway actualizadas
- [ ] environment.prod.ts actualizado con URL del Backend
- [ ] Dockerfile creado en back/
- [ ] netlify.toml creado en front/
- [ ] .gitignore configurado (excluir node_modules, bin, obj)
- [ ] Git initialized en Ventify2

---

## 🔐 Variables sensibles (NO incluir en GitHub)

En tu `.gitignore` agrega:
```
node_modules/
bin/
obj/
dist/
.env
appsettings.Production.json
```

---

## 🚀 Deploy Final

**Backend (Railway):**
1. Crea nuevo servicio desde GitHub en Railway
2. Selecciona repo Ventify2, rama main
3. Configura variables de Environment
4. Deploy automático

**Frontend (Netlify):**
1. Crea sitio desde GitHub en Netlify
2. Selecciona repo Ventify2
3. Build command: `npm run build`
4. Publish directory: `dist/browser`
5. Deploy automático

---

## 🎯 URLs después del deploy

- Backend: `https://tu-backend-railway.railway.app`
- Frontend: `https://tu-frontend-netlify.netlify.app`

---

**¿Listo? Cuando actualices esos 3 puntos (SCHEMA, Backend Vars, environment.prod.ts) puedes pushear a GitHub.**
