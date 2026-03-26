# Claude.md - Guia de Contexto para IA

Este documento fornece contexto essencial sobre o projeto **BarbersClub** para assistentes de IA, facilitando a compreensão da estrutura, arquitetura e convenções do código.

---

## 📋 Visão Geral do Projeto

**BarbersClub** é uma plataforma web ASP.NET Core MVC que conecta clientes a barbearias. O sistema permite:
- Usuários buscarem e avaliarem barbearias
- Donos de barbearias gerenciarem seus negócios, serviços e horários
- Autenticação via Cookies (Web) e JWT (API)

---

## 🏗️ Arquitetura

### Estrutura de 3 Camadas

```
BarbersClub/
├── Web/           # Camada de Apresentação (MVC + API Controllers)
├── Business/      # Camada de Lógica de Negócio (Services + DTOs)
└── Repository/    # Camada de Dados (EF Core + Models)
```

### Fluxo de Dados
```
User Request → Controller → Service (Business) → Repository → Database
                    ↓                  ↓
                  View              DTOs
```

---

## 📂 Estrutura de Diretórios Detalhada

### Web/
- **Controllers/**
  - `WebControllers/` - Controllers que retornam Views (MVC)
  - `ServiceControllers/` - Controllers que retornam JSON (API REST)
- **Views/** - Razor Views organizadas por controller
  - `Auth/`, `BarberShop/`, `Service/`, `NavBar/`, `Home/`
- **wwwroot/** - Arquivos estáticos
  - `css/` - Estilos organizados por feature
  - `js/` - Scripts organizados por feature
  - `images/` - Imagens (users/, barbershops/, services/)
  - `lib/` - Bibliotecas externas (Bootstrap, jQuery)
- **Middleware/** - Middlewares customizados (ex: ErrorMiddleware)

### Business/
- **Services/** - Implementação da lógica de negócio
  - Interfaces: `IAuthService`, `IBarberShopService`, `IServiceService`, `IUserService`
  - Implementações: `AuthService`, `BarberShopService`, `ServiceService`, `UserService`
- **DTOs/** - Data Transfer Objects para comunicação entre camadas
  - Request DTOs: `UserRegisterRequest`, `ServiceRegisterRequest`, etc.
  - Response DTOs: `UserViewResponse`, `BarberShopViewResponse`, etc.
- **Error Handling/** - Exceções customizadas
  - `UserNotFoundException`, `BarberShopNotFoundException`, etc.

### Repository/
- **DbContext/** - `ProjectDbContext` - Configuração do EF Core
- **Models/** - Entidades do banco de dados
  - `User`, `BarberShop`, `Service`, `OfferedService`, `Rating`
  - **Enums/**: `ServiceTypes`, `WorkingDays`
- **Migrations/** - Histórico de migrações do EF Core

---

## 🗄️ Modelos de Dados Principais

### User
```csharp
- UserId (PK)
- FirstName, LastName, Email, Password (hashed)
- ProfilePicUrl
- IsBarberShopOwner
- BarberShops (navigation property)
```

### BarberShop
```csharp
- BarberShopId (PK)
- Name, Description, Address, City, State
- WhatsApp, Instagram
- OpeningHours, ClosingHours
- ProfilePicUrl
- WorkingDays (List<WorkingDays> enum)
- UserId (FK) - Dono da barbearia
- Services, OfferedServices, Ratings (navigation properties)
```

### Service
```csharp
- ServiceId (PK)
- Title, Description, Price
- ServicePicUrl
- BarberShopId (FK)
```

### OfferedService
```csharp
- OfferedServiceId (PK)
- ServiceType (enum: ServiceTypes)
- BarberShopId (FK)
```

### ServiceTypes (Enum)
```csharp
CorteSocial
CorteDiversificado
Barba
Sombrancelha
CorteSocialEBarba
CorteDiversificadoEBarba
CorteSocialESombrancelha
CorteDiversificadoESombrancelha
CorteSocialEBarbaESombrancelha
CorteDiversificadoEBarbaESombrancelha
```

### Rating
```csharp
- RatingId (PK)
- Score (1-5), Comment
- UserId, BarberShopId (FKs)
```

---

## 🔐 Autenticação e Autorização

### Dual Authentication
1. **Cookie Authentication** - Para navegação web (ASP.NET Core Identity Cookies)
   - LoginPath: `/Auth/Login`
   - ExpireTimeSpan: 7 dias

2. **JWT Bearer** - Para consumo de API
   - Configuração em `appsettings.json`: `Jwt:SecretKey`, `Jwt:Issuer`, `Jwt:Audience`
   - Token válido por tempo configurado, sem ClockSkew

### Autorização
- `[Authorize]` - Requer autenticação
- Claims: UserId armazenado em `ClaimTypes.NameIdentifier`

---

## 🔄 Convenções e Padrões

### Nomenclatura
- **Controllers**: `{Feature}Controller` (Web) ou `{Feature}ApiController` (API)
- **Services**: `{Feature}Service` implementa `I{Feature}Service`
- **DTOs**: `{Entity}{Action}Request/Response`
- **Views**: Organizadas em `Views/{Controller}/{Action}.cshtml`

### Tratamento de Erros
- Exceções customizadas em `Business/Error Handling/`
- Middleware global: `ErrorMiddleware` captura exceções e retorna JSON/View apropriada
- Uso de `try-catch` em controllers quando necessário

### Upload de Arquivos
- Imagens de perfil de usuários: `wwwroot/images/users/`
- Imagens de barbearias: `wwwroot/images/barbershops/`
- Imagens de serviços: `wwwroot/images/services/`
- Geração de nomes únicos: `Guid.NewGuid() + extensão`

### Validação
- Data Annotations nos DTOs
- Validação customizada nos Services
- AntiForgeryToken em formulários

---

## 🚀 Executando o Projeto

### Docker (Recomendado)
```bash
# Configurar .env com WEB_PORT, DB_USER, DB_PASSWORD
docker-compose up --build
# Acesse: http://localhost:8080
```

### Local
```bash
# 1. Aplicar migrations
dotnet ef database update --project Repository --startup-project Web

# 2. Executar
dotnet run --project Web
```

---

## 🧪 Testes (Planejado)

Atualmente não há testes implementados. Estrutura sugerida:
- `BarbersClub.Tests/` - Testes unitários (xUnit)
  - `Business.Tests/` - Testes de serviços
  - `Web.Tests/` - Testes de controllers
- Usar Moq para mock de dependências
- InMemoryDatabase para testes de integração

---

## 📡 Endpoints API Principais

### Auth
- `POST /api/auth/register` - Registro de usuário
- `POST /api/auth/login` - Login e geração de JWT

### BarberShops
- `GET /api/barbershops` - Listar todas
- `GET /api/barbershops/{id}` - Detalhes
- `POST /api/barbershops` - Criar (autenticado)
- `PUT /api/barbershops/{id}` - Atualizar (autenticado)
- `DELETE /api/barbershops/{id}` - Deletar (autenticado)

### Services
- `GET /api/services/barbershop/{id}` - Serviços de uma barbearia
- `POST /api/services` - Criar serviço (autenticado)
- `PUT /api/services/{id}` - Atualizar (autenticado)
- `DELETE /api/services/{id}` - Remover (autenticado)

### Users
- `GET /api/users/{id}` - Perfil do usuário
- `PUT /api/users/edit` - Editar perfil (autenticado)

---

## 🛠️ Tecnologias e Dependências

### Backend
- **.NET 9.0**
- **Entity Framework Core 9.0.8**
- **SQL Server**
- **JWT Authentication** (`System.IdentityModel.Tokens.Jwt 8.13.1`)

### Frontend
- **Bootstrap 5**
- **Bootstrap Icons**
- **jQuery 3.x** (validação)
- **SweetAlert2** (notificações)
- **Vanilla JavaScript** (consumo de APIs)

### DevOps
- **Docker & Docker Compose**
- **Scalar API Reference 2.6.9** (documentação interativa)
- **Swashbuckle (Swagger)**

---

## 🐛 Problemas Conhecidos e Soluções

### Arquivos Estáticos não Carregam no Docker
**Causa**: Case sensitivity em caminhos (ex: `~/css/navbar/` vs `~/css/NavBar/`)
**Solução**: Manter consistência de case nos caminhos das Views

### Imagem default-avatar.png não Encontrada
**Causa**: Arquivo não existe em `wwwroot/images/`
**Solução**: Garantir que imagens padrão existam antes do build

### Migrations não Aplicadas no Docker
**Causa**: Container inicia antes do banco estar pronto
**Solução**: `healthcheck` no docker-compose.yml + `depends_on` com `condition: service_healthy`

---

## 📝 Tarefas Futuras / TODOs

- [ ] Implementar sistema de agendamentos
- [ ] Dashboard de estatísticas para donos de barbearia
- [ ] Sistema de busca com filtros avançados (geolocalização)
- [ ] Notificações por email/push
- [ ] Testes automatizados (unitários e integração)
- [ ] CI/CD pipeline
- [ ] Containerização otimizada para produção
- [ ] Implementar paginação nas listagens
- [ ] Cache para melhorar performance
- [ ] Logs estruturados (Serilog)

---

## 💡 Dicas para IA

### Ao Modificar Código
1. **Sempre** verificar a camada correta (Web/Business/Repository)
2. Manter consistência com padrões existentes
3. Atualizar DTOs ao modificar Models
4. Considerar impacto em migrations ao alterar entities
5. Verificar case sensitivity em caminhos de arquivos estáticos

### Ao Adicionar Features
1. Criar Model em `Repository/Models/`
2. Criar DTOs em `Business/DTOs/`
3. Criar Service em `Business/Services/` com interface
4. Registrar Service em `Business/DependencyInjection.cs`
5. Criar Controller em `Web/Controllers/`
6. Criar Views em `Web/Views/` se necessário
7. Adicionar migration: `dotnet ef migrations add {Name} --project Repository --startup-project Web`

### Debugging
- Logs são escritos no console
- Swagger/Scalar disponível em `/scalar/v1`
- Verificar migrations: `dotnet ef migrations list --project Repository --startup-project Web`
- Inspecionar banco: Conectar SQL Server na porta 1433 (credenciais no `.env`)

---

## 📞 Contato e Contribuição

Este é um projeto educacional. Para sugestões ou melhorias, abra uma issue ou pull request no repositório.

---

**Última atualização**: 2026-03-26
**Versão**: 1.0.0
**Framework**: .NET 9.0
