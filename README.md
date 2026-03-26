# 💈 BarbersClub - Sistema de Gerenciamento de Barbearias

Bem-vindo ao **BarbersClub**, uma plataforma completa e robusta desenvolvida para conectar clientes às melhores barbearias da região. O projeto oferece funcionalidades tanto para o usuário final (como busca de serviços e agendamentos) quanto para donos de barbearias (cadastro de negócios, gestão de serviços e estatísticas).

---

## 🔥 Funcionalidades Principais

### 🙍‍♂️ Para Clientes
- **Busca Avançada**: Pesquise barbearias por nome, cidade ou tipo de serviço (Corte Social, Corte Diversificado, Barba, Sobrancelha, etc.).
- **Perfis Detalhados**: Visualize fotos das barbearias, serviços oferecidos e preços.
- **Sistema de Avaliações**: Leia feedbacks de outros usuários e deixe sua própria nota para o atendimento recebido.
- **Gestão de Perfil**: Edite seus dados pessoais, foto de perfil e senha.

### 🏢 Para Donos de Barbearia
- **Cadastro de Negócios**: Registre sua barbearia na plataforma com informações detalhadas (endereço, horários, redes sociais).
- **Gestão de Serviços**: Adicione, edite ou remova os serviços oferecidos com preços e descrições.
- **Galeria de Imagens**: Publique fotos dos seus melhores cortes e do ambiente.
- **Gerenciamento de Horários**: Configure dias e horários de funcionamento.

---

## 🛠️ Tecnologias Utilizadas

Este projeto foi construído utilizando as tecnologias mais modernas do ecossistema .NET:

*   **Backend**: [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) Core (C#)
*   **Acesso a Dados**: [Entity Framework Core 9.0](https://learn.microsoft.com/en-us/ef/core/) (Code First)
*   **Banco de Dados**: Microsoft SQL Server (MSSQL)
*   **Autenticação**:
    *   ASP.NET Core Cookies (para interface Web)
    *   JWT (JSON Web Token) (para consumo via API)
*   **Frontend**: 
    *   ASP.NET Core MVC com Razor Views
    *   [Bootstrap 5](https://getbootstrap.com/) & [Bootstrap Icons](https://icons.getbootstrap.com/)
    *   Vanilla JavaScript (consumo de APIs internas)
    *   [SweetAlert2](https://sweetalert2.github.io/) para notificações
*   **Documentação**: [Swagger / OpenAPI](https://swagger.io/) e [Scalar API Reference](https://scalar.com/)
*   **Infraestrutura**: [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)

---

## 🏗️ Arquitetura do Projeto

O projeto segue uma estrutura de **3 Camadas (3-Tier Architecture)** para garantir separação de responsabilidades e facilidade de manutenção:

```
BarbersClub/
├── Business/          # Regras de Negócio, Serviços e DTOs
│   ├── Services/      # Implementação da lógica de negócio
│   ├── DTOs/          # Data Transfer Objects
│   └── Error Handling/ # Tratamento de exceções customizadas
├── Repository/        # Contexto do Banco (EF Core), Modelos e Migrations
│   ├── DbContext/     # Configuração do Entity Framework
│   ├── Models/        # Entidades do banco de dados
│   │   └── Enums/     # Enumeradores (ServiceTypes, WorkingDays, etc.)
│   └── Migrations/    # Histórico de migrações do banco
├── Web/               # Controllers, Views, Middlewares e Estáticos
│   ├── Controllers/   # Controllers MVC e API
│   │   ├── WebControllers/    # Controllers para Views
│   │   └── ServiceControllers/ # Controllers para API REST
│   ├── Views/         # Razor Views
│   ├── Middleware/    # Middlewares customizados (error handling)
│   └── wwwroot/       # Arquivos estáticos (CSS, JS, imagens)
├── db/                # Configuração do Docker para SQL Server
└── docker-compose.yml # Orquestração de containers
```

### Principais Modelos de Dados
- **User**: Usuários do sistema (clientes e donos de barbearia)
- **BarberShop**: Dados das barbearias
- **Service**: Serviços oferecidos pelas barbearias
- **OfferedService**: Relação entre barbearia e tipos de serviço
- **Rating**: Avaliações dos clientes
- **ServiceTypes** (Enum): CorteSocial, CorteDiversificado, Barba, Sobrancelha e combinações

---

## 🚀 Como Executar

### Pré-requisitos
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop/) instalados.

### Rodando com Docker (Recomendado)

O projeto já está configurado para subir tanto a aplicação quanto o banco de dados SQL Server automaticamente:

1.  Configure as variáveis de ambiente no arquivo `.env` na raiz do projeto:
    ```env
    WEB_PORT=8080
    DB_USER=sa
    DB_PASSWORD=YourStrong!Passw0rd
    ```

2.  No terminal, na raiz do projeto, execute:
    ```bash
    docker-compose up --build
    ```

3.  Acesse a aplicação em: `http://localhost:8080` (ou na porta definida no `.env`).

4.  A documentação da API (Scalar) estará disponível em: `http://localhost:8080/scalar/v1`.

### Rodando Localmente (.NET SDK)

1.  Certifique-se de ter o [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) instalado.

2.  Configure a string de conexão no `Web/appsettings.json` apontando para um SQL Server local:
    ```json
    "ConnectionStrings": {
      "BarbersClubDB": "Server=localhost;Database=BarbersClub;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
    }
    ```

3.  Execute as migrations para criar o banco de dados:
    ```bash
    dotnet ef database update --project Repository --startup-project Web
    ```

4.  Inicie o projeto:
    ```bash
    dotnet run --project Web
    ```

---

## 🔑 Configuração JWT

Para autenticação via API, configure as seguintes chaves no `appsettings.json`:

```json
"Jwt": {
  "SecretKey": "your-secret-key-here-min-32-chars",
  "Issuer": "BarbersClubAPI",
  "Audience": "BarbersClubUsers"
}
```

---

## 📡 Endpoints Principais da API

### Autenticação
- `POST /api/auth/register` - Registro de novos usuários
- `POST /api/auth/login` - Login e geração de token JWT

### Barbearias
- `GET /api/barbershops` - Listar todas as barbearias
- `GET /api/barbershops/{id}` - Detalhes de uma barbearia
- `POST /api/barbershops` - Criar nova barbearia (autenticado)
- `PUT /api/barbershops/{id}` - Atualizar barbearia (autenticado)
- `DELETE /api/barbershops/{id}` - Deletar barbearia (autenticado)

### Serviços
- `GET /api/services/barbershop/{id}` - Serviços de uma barbearia
- `POST /api/services` - Adicionar serviço (autenticado)
- `PUT /api/services/{id}` - Atualizar serviço (autenticado)
- `DELETE /api/services/{id}` - Remover serviço (autenticado)

---

## 🧪 Estrutura de Testes (Planejado)
- Testes unitários para camada Business
- Testes de integração para APIs
- Testes E2E para fluxos principais

---

## 📄 Licença

Este projeto é destinado a fins educacionais e de portfólio. Sinta-se à vontade para explorar e modificar.

---

**Desenvolvido com ❤️ para a comunidade Barbers Club.**
