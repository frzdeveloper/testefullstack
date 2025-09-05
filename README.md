# User Management System

Uma aplicação fullstack para gerenciamento de usuários desenvolvida com .NET Core 8 (backend) e Angular 19 (frontend).

> **📋 Projeto para Vaga de Emprego**  
> Este é um projeto desenvolvido como parte de um processo seletivo, demonstrando conhecimentos em desenvolvimento fullstack com .NET e Angular.

## Funcionalidades

- ✅ Cadastro de usuários
- ✅ Listagem dos usuários cadastrados
- ✅ Autenticação de usuários com JWT
- ✅ Arquitetura limpa no backend
- ✅ CQRS com MediatR
- ✅ Validação com FluentValidation
- ✅ Interceptor para JWT no frontend
- ✅ Guard para proteção de rotas

## Tecnologias Utilizadas

### Backend (.NET Core 8)

- ASP.NET Core Web API
- Entity Framework Core (SQL Server/In-Memory Database)
- MediatR (CQRS)
- FluentValidation
- JWT Authentication
- BCrypt para hash de senhas
- Swagger/OpenAPI
- Testes unitários com xUnit, Moq e FluentAssertions

### Frontend (Angular 19)

- Angular 19 com Standalone Components
- Reactive Forms
- HttpClient
- JWT Interceptor
- Route Guards
- Server-Side Rendering (SSR)

## Arquitetura

- **Backend**: Clean Architecture com Domain, Application, Infrastructure e API
- **Frontend**: Modular com Guards, Interceptors e Services

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- Node.js 18.19+
- Angular CLI 19
- SQL Server (opcional - projeto usa banco em memória por padrão)

### Execução Manual

#### Backend

```bash
cd src/Backend/UserManagement.API
dotnet restore
dotnet run
```

API disponível em: `https://localhost:55930`
Swagger: `https://localhost:55930/swagger`

#### Frontend

```bash
cd src/Frontend/user-management-app
npm install
ng serve
```

Frontend disponível em: `http://localhost:4200`

## Banco de Dados

### Opção 1: Banco em Memória (Padrão)

- Configurado em `appsettings.Development.json`: `"UseInMemoryDatabase": true`
- Não requer SQL Server
- Dados são perdidos ao reiniciar a aplicação

### Opção 2: SQL Server

- Configure a connection string em `appsettings.json`
- Altere para `"UseInMemoryDatabase": false`
- Execute as migrações:

```bash
# Instalar EF Tools
dotnet tool install --global dotnet-ef

# Aplicar migrações
cd src/Backend/UserManagement.API
dotnet ef database update
```

## Endpoints da API

### Autenticação

- `POST /api/auth/login` - Login do usuário
- `POST /api/auth/logout` - Logout do usuário

### Usuários

- `POST /api/users` - Criar novo usuário
- `GET /api/users` - Listar usuários (requer autenticação)

## Fluxo da Aplicação

1. **Cadastro**: Acesse `/users/create` para criar um novo usuário
2. **Login**: Acesse `/auth/login` para fazer login
3. **Lista de Usuários**: Após o login, você será redirecionado para `/users`

## Testes

```bash
cd src/Backend/UserManagement.Tests
dotnet test
```

**Tecnologias**: xUnit, Moq, FluentAssertions
**Cobertura**: Handlers CQRS, validações, hash de senhas, repositório

## Docker

O projeto inclui Dockerfiles para containerização:

- **Backend**: `src/Backend/Dockerfile`
- **Frontend**: `src/Frontend/user-management-app/Dockerfile`

## Sobre o Projeto

Projeto desenvolvido para processo seletivo, demonstrando conhecimentos em desenvolvimento fullstack com .NET 8 e Angular 19, seguindo Clean Architecture e boas práticas de desenvolvimento.

**Desenvolvido por**: [Felipe Rabelo Zavitoski]
