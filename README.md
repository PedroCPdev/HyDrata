# HyDrata – API de Gestão

API REST administrativa do sistema **HyDrata**, responsável pelo cadastro de produtores rurais, cooperativas, planos de assinatura e propriedades.

Desenvolvida em **.NET 9** com **ASP.NET Core Web API**, **Entity Framework Core** e banco de dados **Oracle**.

---

## Tecnologias

| Tecnologia | Versão |
|---|---|
| .NET / ASP.NET Core | 9.0 |
| Entity Framework Core | 9.0.5 |
| Oracle.EntityFrameworkCore | 9.23.60 |
| Swashbuckle (Swagger) | 7.3.1 |

---

## Arquitetura

O projeto adota arquitetura em **3 camadas**, com separação clara de responsabilidades:

```
┌──────────────────────────────────────────────────────┐
│                   Controllers                        │  ← Recebe HTTP, valida, retorna respostas
├──────────────────────────────────────────────────────┤
│                  Repositories                        │  ← Abstração do acesso a dados (interfaces + impl.)
├──────────────────────────────────────────────────────┤
│              Dados (AppDbContext / EF Core)          │  ← Mapeamento ORM → Oracle
└──────────────────────────────────────────────────────┘
```

### Estrutura de Pastas

```
HyDrata_Vitalis/
├── Controllers/
│   ├── ProdutoresApiController.cs
│   ├── CooperativasApiController.cs
│   ├── PlanosApiController.cs
│   ├── PropriedadesApiController.cs
│   └── ProdutorCooperativaApiController.cs
├── Repositories/
│   ├── IRepositories.cs              ← Interfaces de todos os repositórios
│   ├── ProdutorRepository.cs
│   ├── CooperativaRepository.cs
│   ├── PlanoRepository.cs
│   ├── PropriedadeRepository.cs
│   └── ProdutorCooperativaRepository.cs
├── Dto/
│   ├── ProdutorDtos.cs
│   ├── CooperativaDtos.cs
│   ├── PlanoDtos.cs
│   ├── PropriedadeDtos.cs
│   └── ProdutorCooperativaDtos.cs
├── Models/
│   ├── Produtor.cs
│   ├── Cooperativa.cs
│   ├── Plano.cs
│   ├── Propriedade.cs
│   └── ProdutorCooperativa.cs
├── Dados/
│   └── AppDbContext.cs
├── Properties/
│   └── launchSettings.json
├── Migrations/                       (gerado pelo EF Core)
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

---

## Modelagem do Banco

### Diagrama de Entidades

```
┌──────────────┐       1:N      ┌──────────────────┐
│   TB_PLANOS  │──────────────►│  TB_PROPRIEDADES  │
└──────────────┘                └──────────────────┘
                                        ▲  N:1
                                        │
┌──────────────┐       1:N      ┌───────┴──────────┐
│ TB_PRODUTORES│──────────────►│  TB_PROPRIEDADES  │
│              │                └──────────────────┘
│              │   N:N via      ┌──────────────────────────┐
│              │──────────────►│  TB_PRODUTOR_COOPERATIVA  │◄──┐
└──────────────┘                │  ProdutorId (PK, FK)     │   │
                                │  CooperativaId (PK, FK)  │   │
                                └──────────────────────────┘   │
                                                  ┌───────────────┐
                                                  │TB_COOPERATIVAS│
                                                  └───────────────┘
```

### Tabelas

| Tabela | Descrição |
|---|---|
| `TB_PRODUTORES` | Produtores rurais cadastrados no sistema |
| `TB_COOPERATIVAS` | Cooperativas agrícolas |
| `TB_PLANOS` | Planos de assinatura disponíveis |
| `TB_PROPRIEDADES` | Propriedades rurais vinculadas a produtor e plano |
| `TB_PRODUTOR_COOPERATIVA` | Tabela de junção para o N:N Produtor ↔ Cooperativa |

---

## Relacionamentos

### 1:N — Produtor → Propriedades

Um produtor pode ter várias propriedades. Cada propriedade pertence a exatamente um produtor. Configurado com `OnDelete(Restrict)` — não é possível deletar um produtor que possua propriedades vinculadas.

### 1:N — Plano → Propriedades

Um plano pode ser usado por várias propriedades. Configurado com `OnDelete(Restrict)` — não é possível deletar um plano em uso.

### N:N — Produtor ↔ Cooperativa

Um produtor pode pertencer a várias cooperativas, e uma cooperativa pode ter vários produtores. Implementado via tabela de junção `TB_PRODUTOR_COOPERATIVA` com **chave primária composta** `(ProdutorId, CooperativaId)`. Configurado com `OnDelete(Cascade)` nos dois lados — ao deletar um produtor ou cooperativa, os vínculos são removidos automaticamente.

---

## Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Oracle DB (local via Docker ou instância disponível)
- [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### 1. Subir o Oracle com Docker

```bash
docker run -d \
  --name oracle-hydrata \
  -p 1521:1521 \
  -e ORACLE_PASSWORD=oracle \
  gvenzl/oracle-free:latest
```

Aguarde ~60s até o container estar pronto:

```bash
docker logs -f oracle-hydrata | grep "DATABASE IS READY"
```

### 2. Configurar a Connection String

Edite `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "Data Source=localhost:1521/FREE;User Id=SYSTEM;Password=SUA_SENHA_AQUI;"
  }
}
```

### 3. Aplicar as Migrations

```bash
cd HyDrata_Vitalis

dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Executar a API

```bash
dotnet run
```

Acesse o Swagger em: **http://localhost:5000/swagger**

---

## Migrations

O EF Core rastreia todas as mudanças no schema via migrations. Cada `dotnet ef migrations add <Nome>` gera um arquivo C# com os comandos SQL correspondentes.

```bash
# Criar nova migration após alterar um Model
dotnet ef migrations add NomeDaMudanca

# Aplicar no banco
dotnet ef database update

# Reverter para uma migration anterior (rollback)
dotnet ef database update NomeDaMigrationAnterior

# Remover a última migration (se ainda não foi aplicada)
dotnet ef migrations remove

# Listar todas as migrations e seu status
dotnet ef migrations list
```

As migrations ficam na pasta `Migrations/` e devem ser commitadas no repositório.

---

## Endpoints

### Produtores — `/api/produtores`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/produtores` | Lista todos os produtores |
| `GET` | `/api/produtores/{id}` | Busca produtor por ID |
| `GET` | `/api/produtores/{id}/propriedades` | Produtor com suas propriedades vinculadas |
| `POST` | `/api/produtores` | Cadastra novo produtor |
| `PUT` | `/api/produtores/{id}` | Atualiza produtor |
| `DELETE` | `/api/produtores/{id}` | Remove produtor |

### Cooperativas — `/api/cooperativas`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/cooperativas` | Lista todas |
| `GET` | `/api/cooperativas/{id}` | Busca por ID |
| `POST` | `/api/cooperativas` | Cadastra nova |
| `PUT` | `/api/cooperativas/{id}` | Atualiza |
| `DELETE` | `/api/cooperativas/{id}` | Remove |

### Planos — `/api/planos`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/planos` | Lista todos |
| `GET` | `/api/planos/{id}` | Busca por ID |
| `POST` | `/api/planos` | Cadastra novo |
| `PUT` | `/api/planos/{id}` | Atualiza |
| `DELETE` | `/api/planos/{id}` | Remove |

### Propriedades — `/api/propriedades`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/propriedades` | Lista todas (aceita `?produtorId=1`) |
| `GET` | `/api/propriedades/{id}` | Busca por ID |
| `POST` | `/api/propriedades` | Cadastra nova |
| `PUT` | `/api/propriedades/{id}` | Atualiza |
| `DELETE` | `/api/propriedades/{id}` | Remove |

### Associações Produtor-Cooperativa — `/api/produtorcooperativa`

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/produtorcooperativa` | Lista todas as associações |
| `GET` | `/api/produtorcooperativa/produtor/{id}` | Cooperativas de um produtor |
| `GET` | `/api/produtorcooperativa/cooperativa/{id}` | Produtores de uma cooperativa |
| `POST` | `/api/produtorcooperativa` | Cria associação |
| `DELETE` | `/api/produtorcooperativa/{prodId}/{coopId}` | Remove associação |

---

## Exemplos de Uso

### Criar um Plano

```bash
curl -X POST http://localhost:5000/api/planos \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Plano Básico",
    "valorMensalidade": 99.90,
    "descricao": "Monitoramento de até 3 sensores",
    "status": "ATIVO"
  }'
```

### Criar um Produtor

```bash
curl -X POST http://localhost:5000/api/produtores \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João da Silva",
    "cpf": "123.456.789-00",
    "email": "joao@fazenda.com",
    "telefone": "(11) 98765-4321",
    "senha": "senha123",
    "status": "ATIVO"
  }'
```

### Criar uma Cooperativa

```bash
curl -X POST http://localhost:5000/api/cooperativas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "CoopAgro SP",
    "email": "contato@coopagrosp.com.br",
    "telefone": "(11) 3000-0000",
    "status": "ATIVA"
  }'
```

### Criar uma Propriedade

```bash
curl -X POST http://localhost:5000/api/propriedades \
  -H "Content-Type: application/json" \
  -d '{
    "produtorId": 1,
    "planoId": 1,
    "nome": "Fazenda Boa Vista",
    "areaHectares": 250.5,
    "cidade": "Ribeirão Preto",
    "estado": "SP",
    "latitude": -21.1775,
    "longitude": -47.8106,
    "status": "ATIVA"
  }'
```

### Associar Produtor a Cooperativa

```bash
curl -X POST http://localhost:5000/api/produtorcooperativa \
  -H "Content-Type: application/json" \
  -d '{
    "produtorId": 1,
    "cooperativaId": 1
  }'
```

### Buscar Produtor com suas Propriedades

```bash
curl http://localhost:5000/api/produtores/1/propriedades
```

### Remover Associação N:N

```bash
curl -X DELETE http://localhost:5000/api/produtorcooperativa/1/1
# → 204 No Content
```

---

## Comportamento ao Deletar

| Entidade deletada | O que acontece |
|---|---|
| **Produtor** | ❌ Bloqueado se tiver propriedades (`Restrict`). Associações com cooperativas são removidas automaticamente (`Cascade`). |
| **Cooperativa** | As associações com produtores são removidas automaticamente (`Cascade`). |
| **Plano** | ❌ Bloqueado se houver propriedades usando o plano (`Restrict`). |
| **Propriedade** | ✅ Removida diretamente, sem impacto em outras tabelas. |
| **Associação Produtor-Cooperativa** | ✅ Removida diretamente — produtor e cooperativa permanecem. |

O código Oracle para violação de FK é **ORA-02292**, tratado nos controllers com `409 Conflict`.

---

## Validações de Negócio

- **CPF único:** não é possível cadastrar dois produtores com o mesmo CPF (`409 Conflict`)
- **E-mail único:** não é possível cadastrar dois produtores com o mesmo e-mail (`409 Conflict`)
- **FK de Produtor/Plano:** ao criar uma propriedade, o sistema verifica se o `ProdutorId` e o `PlanoId` existem antes de inserir (`404 Not Found`)
- **Associação duplicada:** não é possível associar o mesmo produtor à mesma cooperativa duas vezes (`409 Conflict`)
