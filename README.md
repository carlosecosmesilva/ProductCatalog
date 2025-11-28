# API - Catálogo de Produtos

**API - Catálogo de Produtos** é uma API RESTing construída em .NET 8 para gerenciar o CRUD completo de produtos. O projeto segue princípios de arquitetura empresarial (DDD + Clean/Onion), aplica caching com Redis para otimização de leitura e conta com cobertura de testes e CI/CD para garantir qualidade e confiabilidade.

**Estado:** Projeto com arquitetura orientada a domínio, testes automatizados e preparação para containerização/CI.

**Visão Geral e Requisitos Funcionais**

-   **Descrição:** API REST para CRUD de Produtos (criação, leitura, atualização e deleção), com listagem, busca por nome e ordenação.
-   **Requisitos Funcionais atendidos:**
    -   **Criação:** `POST /products` com validação (preço não negativo).
    -   **Atualização:** `PUT /products/{id}` com validações de domínio.
    -   **Deleção:** `DELETE /products/{id}`.
    -   **Listagem Completa:** `GET /products` com filtros opcionais.
    -   **Visualização por ID:** `GET /products/{id}`.
    -   **Ordenação e Busca:** parâmetros de query para ordenar e filtrar por nome.

**Stack Tecnológica e Padrões**

-   **Linguagem / Framework:** .NET 8 (ou superior).
-   **Arquitetura:** Domain-Driven Design (DDD) combinado com Clean / Onion Architecture.
-   **Persistência:** Entity Framework Core (EF Core).
-   **Cache:** Redis (padrão cache-aside com versãoamento para invalidação eficiente).
-   **Qualidade:** Testes Unitários + Testes de Integração (xUnit + Moq + fixtures de integração).
-   **CI/CD:** GitHub Actions (workflow `ci.yml` presente em `.github/workflows/`).
-   **Conteinerização:** Docker multi-stage (SDK/runtime separados) e `docker-compose.yml` para orquestrar API + SQL Server + Redis.

**Estrutura da Solution e Projetos**

A Solution está organizada para refletir separação de responsabilidades e padrões DDD/Clean:

-   `ProductCatalog.API` — Camada de apresentação / API (controllers, configuração de DI, Swagger).
-   `ProductCatalog.Application` — Casos de uso, DTOs, serviços de aplicação (`ProductService`), interfaces de domínio para orquestração.
-   `ProductCatalog.Domain` — Entidades, regras de negócio e validações (ex.: validação de preço não negativo).
-   `ProductCatalog.Infrastructure` — Implementações de infra (EF Core DbContext, Repositories, Redis cache adapter, Migrations).
-   `ProductCatalog.Tests` — Testes unitários e de integração cobrindo services, mappers e integrações.

**Persistência de Dados (Entity Framework)**

-   **Abordagem EF:** Code-First. O projeto utiliza migrações do Entity Framework Core para evoluir o schema (`Migrations/` presente no projeto `ProductCatalog.Infrastructure`).
-   **Data Seeding:** O projeto configura _data seeding_ para popular **5 produtos iniciais** durante a criação/atualização do banco via migrações, garantindo um conjunto mínimo de dados para desenvolvimento e testes.

Observação: para aplicar migrações localmente pode-se usar:

```bash
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
```

**Execução Local e Conteinerização**

Pré-requisitos: `Docker` e `docker-compose` (ou `docker compose`) instalados localmente.

1. Crie o secret de DB (dev) localmente — **NÃO** comitar esse arquivo:

```bash
mkdir -p secrets
printf 'MyStrongSaPassword!' > secrets/db_password.txt
chmod 600 secrets/db_password.txt
```

2. Subir todo o ambiente (API + SQL Server + Redis):

```bash
docker compose up --build -d
# ou, se você usa a versão clássica do compose:
docker-compose up -d
```

-   A API expõe o Swagger UI (após inicialização) em: `http://localhost:8080/swagger` (ou `http://<HOST>:8080/swagger`).
-   Se precisar aplicar migrações manualmente dentro do container da API:

```bash
docker compose exec productcatalog-api dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.API
```

**Segurança:** Use Docker Secrets / Vault em ambientes de produção. Nunca comite arquivos de senha.

**Nota sobre Connection String via `docker-compose`**

O `docker-compose.yml` presente no repositório também pode fornecer a connection string diretamente para a API via variável de ambiente `ConnectionStrings__DefaultConnection`. Exemplo de trecho do `docker-compose.yml`:

```yaml
services:
    productcatalog-api:
        environment:
            - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=ProductCatalogDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;
```

Essa abordagem é conveniente para desenvolvimento local. Em produção, prefira montar o segredo via Docker Secrets ou utilizar um Vault para evitar senhas em texto plano.

**Testes e CI/CD**

-   O projeto inclui **Testes Unitários** e **Testes de Integração** localizados em `ProductCatalog.Tests`.
-   Executar todos os testes localmente:

```bash
dotnet test
```

-   O repositório inclui um workflow do GitHub Actions (`.github/workflows/ci.yml`) que restaura dependências, executa `dotnet test` e realiza `dotnet build` em cada `push` ou `pull_request` para as branches `main`/`master`.

**Padrões de Caching e Consistência**

-   O padrão adotado é _cache-aside_ com versioning. A chave de versão (`products:version`) é atualizada em operações de escrita (Add/Update/Delete). As chaves de listagem incluem essa versão para permitir invalidação eficiente sem enumerar chaves.
-   TTL aplicado para listagens: 7 minutos (configurável no serviço de aplicação).
-   Observações operacionais: falhas na atualização da versão podem causar leitura de dados desatualizados até expiração do TTL — recomenda-se monitoramento e retry na invalidação em produção.

**Boas Práticas e Recomendações Operacionais**

-   Use **um gerenciador de secrets** (Docker Swarm Secrets, Kubernetes Secrets, HashiCorp Vault, Azure Key Vault) em produção.
-   Ative logs estruturados para operações de cache (hit/miss/invalidate) para melhor observabilidade.
-   Considere adicionar `actions/cache` ao workflow de CI para acelerar builds com nuget cache.
-   Execute os testes de integração contra instâncias isoladas de SQL Server/Redis em ambientes CI (containers ephemeral), garantindo pipelines determinísticos.

**Contato e Contribuição**

Contribuições são bem-vindas via Pull Requests. Para dúvidas ou alinhamentos arquiteturais, abra uma issue descrevendo o objetivo.

---

**Arquitetura e Qualidade:** Este repositório demonstra preocupações de engenharia: separação de responsabilidades, patterns DDD/Clean, testes automatizados, CI e preparo para produção com conteinerização e gestão de secrets.
