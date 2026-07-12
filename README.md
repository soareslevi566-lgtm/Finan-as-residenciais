# Casa Clara — Controle de gastos residenciais

Aplicação full stack para registrar pessoas e transações de uma residência e acompanhar receitas, despesas e saldo. Os dados são persistidos em SQLite e permanecem disponíveis após reiniciar a aplicação.

## Tecnologias

- API: .NET 8, ASP.NET Core, C#, Entity Framework Core, SQLite e Swagger/OpenAPI.
- Interface: React 18, TypeScript, Vite, Axios, React Router e CSS responsivo.
- Testes: xUnit e SQLite em memória (somente nos testes).

## Arquitetura

O back-end separa controllers, DTOs, entidades, repositórios, serviços e middleware. Controllers tratam HTTP; serviços concentram regras de negócio; repositórios isolam o acesso ao Entity Framework. Entidades nunca são expostas diretamente. O front-end separa tipos e cliente HTTP, com páginas responsivas para pessoas, transações e totais.

```text
controle-gastos-residenciais/
├── backend/
│   ├── ControleGastos.Api/
│   │   ├── Controllers/ Data/ DTOs/ Enums/ Exceptions/
│   │   ├── Middleware/ Migrations/ Models/ Repositories/ Services/
│   │   ├── Program.cs e appsettings.json
│   ├── ControleGastos.Tests/ServiceTests.cs
│   └── ControleGastos.sln
├── frontend/src/{services,types}
├── frontend/src/App.tsx
├── frontend/src/styles.css
└── .gitignore
```

## Regras principais

- Nome e descrição não aceitam conteúdo vazio ou somente espaços.
- Idade deve ser zero ou maior; valores financeiros devem ser positivos.
- A pessoa informada na transação precisa existir.
- Menores de 18 anos só podem receber despesas. A interface antecipa a regra e a API é a autoridade final.
- Cada transação possui data e categoria; datas futuras e períodos fora do limite são rejeitados pela API.
- A listagem aceita filtros combinados por pessoa, tipo, categoria, período e descrição.
- Excluir uma pessoa remove suas transações por `ON DELETE CASCADE` no SQLite.
- Pessoas sem transações aparecem nos totais com valores zerados.
- Valores monetários usam `decimal` na API e são exibidos em BRL na interface.

## Como executar

Pré-requisitos: SDK .NET 8 e Node.js 20 ou superior.

### API

```powershell
cd backend/ControleGastos.Api
dotnet restore
dotnet run --urls http://localhost:5000
```

A migração é aplicada automaticamente ao iniciar e cria `controle-gastos.db`. Swagger: `http://localhost:5000/swagger`.

Para criar uma nova migração:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add NomeDaMigracao --project backend/ControleGastos.Api
dotnet ef database update --project backend/ControleGastos.Api
```

### Interface

```powershell
cd frontend
copy .env.example .env
npm install
npm run dev
```

Interface: `http://localhost:5173`. Para outra URL de API, altere `VITE_API_URL` no `.env`.

## Endpoints

| Método | Rota | Resultado |
|---|---|---|
| POST | `/api/pessoas` | Cria pessoa (`201`) |
| GET | `/api/pessoas` | Lista pessoas (`200`) |
| DELETE | `/api/pessoas/{id}` | Exclui em cascata (`204`) |
| POST | `/api/transacoes` | Cria transação (`201`) |
| GET | `/api/transacoes?pessoaId=1&tipo=Despesa&categoria=Moradia&inicio=2026-01-01&fim=2026-12-31&busca=energia` | Lista/filtra transações (`200`) |
| DELETE | `/api/transacoes/{id}` | Exclui um lançamento (`204`) |
| GET | `/api/totais` | Totais individuais e gerais (`200`) |

Exemplos:

```http
POST /api/pessoas
Content-Type: application/json

{"nome":"Maria","idade":25}
```

```http
POST /api/transacoes
Content-Type: application/json

{"descricao":"Salário","valor":3000.00,"tipo":"Receita","pessoaId":1,"categoria":"Salario","data":"2026-07-10T12:00:00Z"}
```

Enums são enviados e recebidos como texto: `Despesa` ou `Receita`. Erros seguem `{"status":400,"mensagem":"..."}`.

## Testes e builds

```powershell
dotnet build backend/ControleGastos.sln
dotnet test backend/ControleGastos.sln
cd frontend
npm install
npm run build
```

Os 11 testes cobrem criação e validação de pessoa, regras de menores, pessoa inexistente, valores inválidos, cascata, totais individuais/gerais e pessoa sem transações.

## Decisões e melhorias futuras

A migração automática simplifica a avaliação local; repositórios são objetivos, sem abstrações genéricas. O agregado de totais preserva pessoas sem transações. A API restringe origens CORS, aplica limite global por IP, padroniza validações e envia cabeçalhos contra MIME sniffing, framing e uso indevido de recursos do navegador. Melhorias possíveis: autenticação, paginação, edição, orçamento mensal, auditoria e pipeline de CI.
