# 💈 BarbersClub - Sistema de Gerenciamento de Barbearias

Bem-vindo ao **BarbersClub**, uma plataforma completa e robusta desenvolvida para conectar clientes às melhores barbearias da região. O projeto oferece funcionalidades tanto para o usuário final (como busca de serviços e agendamentos) quanto para donos de barbearias (cadastro de negócios, gestão de serviços e estatísticas).

---

## 🔥 Funcionalidades Principais

### 🙍‍♂️ Para Clientes
- **Busca Avançada**: Pesquise barbearias por nome, cidade ou tipo de serviço (Corte, Barba, Pintura, Sobrancelha, etc.).
- **Perfis Detalhados**: Visualize fotos das barbearias, serviços oferecidos e preços.
- **Sistema de Avaliações**: Leia feedbacks de outros usuários e deixe sua própria nota para o atendimento recebido.
- **Gestão de Perfil**: Gerencie seus dados de usuário e histórico.

### 🏢 Para Donos de Barbearia
- **Cadastro de Negócios**: Registre sua barbearia na plataforma.
- **Gestão de Serviços**: Adicione, edite ou remova os serviços oferecidos (Haircuts, Beards, etc.).
- **Dashboard de Estatísticas**: Visualize métricas importantes sobre o desempenho da sua barbearia (em desenvolvimento).
- **Galeria de Imagens**: Publique fotos dos seus melhores cortes e do ambiente.

---

## 🛠️ Tecnologias Utilizadas

Este projeto foi construído utilizando as tecnologias mais modernas do ecossistema .NET:

*   **Backend**: [.NET 8/9](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) Core (C#)
*   **Acesso a Dados**: [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) (Code First)
*   **Banco de Dados**: Microsoft SQL Server (MSSQL)
*   **Autenticação**:
    *   ASP.NET Core Cookies (para interface Web)
    *   JWT (JSON Web Token) (para consumo via API)
*   **Frontend**: 
    *   ASP.NET Core MVC com Razor Views
    *   [Bootstrap 5](https://getbootstrap.com/) & [Bootstrap Icons](https://icons.getbootstrap.com/)
    *   Custom JavaScript (consumo de APIs internas)
*   **Documentação**: [Swagger / OpenAPI](https://swagger.io/) e [Scalar API Reference](https://scalar.com/)
*   **Infraestrutura**: [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)

---

## 🏗️ Arquitetura do Projeto

O projeto segue uma estrutura de **3 Camadas (3-Tier Architecture)** para garantir separação de responsabilidades e facilidade de manutenção:

```bash
BarbersClub/
├── Business/       # Regras de Negócio, Serviços e DTOs
├── Repository/     # Contexto do Banco (EF Core), Modelos e Migrations
├── Web/            # Controllers, Views, Middlewares e Estáticos (wwwroot)
├── db/             # Customizações do Docker para o banco de dados
└── BarbersClub.sln # Arquivo da Solução .NET
```

---

## 🚀 Como Executar

### Pré-requisitos
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop/) instalados.

### Rodando com Docker (Recomendado)

O projeto já está configurado para subir tanto a aplicação quanto o banco de dados SQL Server automaticamente:

1.  Certifique-se de configurar as variáveis de ambiente necessárias no `docker-compose.yml` (ou em um arquivo `.env`).
2.  No terminal, na raiz do projeto, execute:
    ```bash
    docker-compose up --build
    ```
3.  Acesse a aplicação em: `http://localhost:5000` (ou na porta definida no seu arquivo).
4.  A documentação da API (Swagger/Scalar) estará disponível em: `http://localhost:5000/scalar/v1`.

### Rodando Localmente (.NET SDK)

1.  Configure a string de conexão no `Web/appsettings.json` apontando para um SQL Server local.
2.  Execute as migrations para criar o banco de dados:
    ```bash
    dotnet ef database update --project Repository --startup-project Web
    ```
3.  Inicie o projeto:
    ```bash
    dotnet run --project Web
    ```

---

## 📄 Licença

Este projeto é destinado a fins educacionais e de portfólio. Sinta-se à vontade para explorar e modificar.

---

**Desenvolvido com ❤️ para a comunidade Barbers Club.**
