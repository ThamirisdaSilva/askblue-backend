# Aula prática: uma campanha atravessando o backend

**Objetivo:** cadastrar e consultar uma campanha no Swagger, entendendo onde cada parte do código fica. O projeto é uma demo independente, não uma alteração no projeto dos alunos.

**Verificação desta entrega:** arquivos e coerência revisados estaticamente; não foi possível compilar ou executar porque o ambiente de preparação não tem o SDK .NET. Execute o checklist abaixo antes da aula. Não considere esta demo homologada sem esse teste.

## 1. Rodar o projeto pronto

É necessário **SDK .NET 10** (não apenas Runtime), terminal, editor e acesso à internet para restaurar o pacote NuGet. No Mac Apple Silicon, instale a versão Arm64.

Descompacte o ZIP e abra a pasta `demo-segunda`. No terminal:

```bash
dotnet --list-sdks
cd ApiCampanhas
dotnet restore
dotnet build
dotnet run --no-launch-profile -- --urls http://localhost:5080 --environment Development
```

A primeira linha deve mostrar um SDK `10.x`. O comando de execução deve permanecer aberto; não feche o terminal. Abra:

```text
http://localhost:5080/swagger
```

O comando fixa a porta e habilita o ambiente Development, no qual o Swagger está disponível. Para parar: `Ctrl+C`.

## 2. Demonstrar antes de explicar todas as pastas

1. Expanda **GET /campanhas** → **Try it out** → **Execute**. Inicialmente a resposta é `200` com `[]`.
2. Expanda **POST /campanhas** → **Try it out**. Substitua o JSON por:

```json
{
  "nome": "Aniversário da empresa",
  "mensagem": "Venha comemorar com a nossa equipe!"
}
```

3. Clique **Execute**. Espere `201`, um `id`, `nome` e `mensagem`. O cabeçalho `Location` indica onde consultar essa campanha.
4. Execute **GET /campanhas** novamente. A campanha deve aparecer.
5. Teste o POST com:

```json
{
  "nome": "   ",
  "mensagem": "Texto de teste"
}
```

6. Espere `400` e a explicação de que o nome é obrigatório. Consulte a lista: esse pedido inválido não deve ter adicionado uma campanha.
7. Repita com `nome` preenchido e `mensagem` vazia. Também deve retornar `400`.
8. No **GET /campanhas/{id}**, use o ID criado: `200`. Use `00000000-0000-0000-0000-000000000000`: `404`.

**Essas duas regras são didáticas, não regras confirmadas do projeto do grupo.** Criar campanha aqui não publica nada nem faz aprovação ou agendamento.

## 3. O que acabou de acontecer?

```text
Swagger envia POST /campanhas com JSON
   ↓
Program.cs recebe o pedido no endpoint
   ↓
CampanhaService.Criar coordena a criação
   ↓
new Campanha(...) valida nome e mensagem
   ↓
Serviço guarda em memória e devolve a campanha
   ↓
Endpoint responde HTTP 201 com JSON
```

Se uma regra falhar, o construtor interrompe a criação com `ArgumentException`. O endpoint captura esse erro esperado e responde `400`. Ele não captura qualquer erro técnico como se fosse erro do aluno.

**Endpoint** é uma operação acessível por um método HTTP e um caminho. `GET /campanhas` consulta; `POST /campanhas` cria. Mesmo caminho, operações diferentes. Swagger é uma tela para experimentar essas operações, não o backend e não o banco.

## 4. Cada arquivo tem um trabalho

| Arquivo | Trabalho | Explicação para falar em voz alta |
|---|---|---|
| `Program.cs` | Configura a API, registra o serviço e define os endpoints | “Aqui entra o pedido e sai a resposta HTTP.” |
| `Contratos/CriarCampanhaRequest.cs` | Define nome e mensagem recebidos no JSON | “É o formato dos dados que o cliente pode enviar.” |
| `Aplicacao/CampanhaService.cs` | Coordena criar, listar e buscar; nesta demo também guarda os dados | “Pede ao domínio para criar e guarda o resultado.” |
| `Dominio/Campanha.cs` | Representa uma campanha válida e protege as regras | “Não deixa existir uma campanha sem nome ou mensagem.” |
| `ApiCampanhas.csproj` | Define .NET 10 e a dependência do Swagger | “São as configurações e bibliotecas do projeto.” |

### Por que não deixar tudo no endpoint?

Poderia, no primeiro exercício. Agora separamos responsabilidades para enxergar melhor o código. O endpoint chama `service.Criar(...)`: essa chamada executa o método C# do serviço. Não é outra chamada de rede. O serviço usa `new Campanha(...)`, que executa o construtor e suas validações.

O ASP.NET Core fornece `CampanhaService service` automaticamente porque registramos a classe com `AddSingleton`. A mesma instância atende os pedidos, preservando os dados em memória. Não é necessário instanciar um novo serviço em cada endpoint.

`Campanha` possui propriedades somente de leitura: depois de criada, não pode receber um nome vazio por um `set` público. A entrada do usuário é um DTO (`CriarCampanhaRequest`); ele não escolhe o ID. O domínio cria o ID com `Guid.NewGuid()`.

### Isso já é Clean Architecture completa?

Não. É uma separação didática de responsabilidades em um projeto só. O domínio não conhece HTTP ou Swagger. Para reduzir complexidade, o armazenamento em memória ainda está dentro do serviço. Uma evolução posterior seria extrair um repositório e implementar persistência na infraestrutura. Não precisa criar quatro projetos antes de entender esse fluxo.

## 5. Se quiser construir do zero durante a aula

Faça isso em OUTRA pasta, sem sobrescrever o projeto dos alunos:

```bash
dotnet new web -n ApiCampanhas -f net10.0
cd ApiCampanhas
dotnet add package Swashbuckle.AspNetCore --version 10.0.1
```

1. Crie as pastas `Dominio`, `Aplicacao` e `Contratos` no editor.
2. Copie os três arquivos `.cs` dessas pastas da demo para as respectivas pastas do novo projeto.
3. Substitua o conteúdo de `Program.cs` pelo da demo, inteiro.
4. Execute `dotnet build`.
5. Execute o comando de `dotnet run` da seção 1 e faça os testes da seção 2.

Não é necessário `dotnet new webapi`, controllers ou banco para esta demo. O template `web` vazio evita código extra. Não misture este exemplo com trechos de `AddOpenApi/MapOpenApi`: escolhemos aqui a geração de documentação pelo Swashbuckle.

## 6. Transferir o raciocínio para os projetos

Não é pedir que todos copiem campanha como seu projeto. É repetir a estrutura usando o que já modelaram:

- Campanhas: criar campanha e consultar o resultado.
- Orçamentação: criar orçamento ligado a serviço e consultar o resultado; usar as validações que o grupo já implementou.
- Incidentes: registrar incidente e consultar o resultado; confirmar os campos obrigatórios com o grupo.

Cada grupo demonstra uma requisição válida, uma inválida e a linha da regra que rejeitou a inválida. Quem já tem classe válida reaproveita essa classe; não refaz tudo por causa da demo. Banco, aprovação, autenticação e publicação real são próximos incrementos, não requisitos desta primeira entrega.

## 7. Problemas comuns

- **SDK não suporta net10.0:** confirme `dotnet --list-sdks`; instale o SDK .NET 10 correto para o computador. `global.json` em pasta acima pode forçar outro SDK.
- **Restore falha:** veja acesso ao NuGet/proxy e a mensagem exata; é preciso baixar Swashbuckle e dependências.
- **Porta ocupada:** pare a outra API com Ctrl+C ou troque `5080` por `5081` no comando e no navegador.
- **Swagger 404:** use `/swagger` e execute exatamente com `--environment Development`.
- **Navegador não abre:** confira se o terminal continua executando e se a URL é HTTP, não HTTPS.
- **POST 400 com JSON malformado:** isso pode ser rejeitado pelo ASP.NET Core antes do serviço; não é necessariamente nossa regra de domínio.
- **A lista sumiu:** é esperado após reiniciar. Os dados ficam só em memória e não são compartilhados entre processos.

## Limites e segurança

Projeto apenas local/didático: sem autenticação, sem autorização, sem banco, sem integração social, sem edição ou aprovação. Não disponibilize publicamente nem use dados pessoais reais. A coleção suporta acesso concorrente simples, mas isso não torna a aplicação pronta para produção. A lista não tem ordenação garantida.

## Referências oficiais e dependência fixada

- SDK .NET: https://dotnet.microsoft.com/download/dotnet/10.0
- Swashbuckle.AspNetCore 10.0.1: https://www.nuget.org/packages/Swashbuckle.AspNetCore/10.0.1
- Swagger/ASP.NET Core: https://learn.microsoft.com/aspnet/core/tutorials/web-api-help-pages-using-swagger

A versão do pacote foi fixada para evitar alterações silenciosas entre máquinas. A página do pacote foi consultada; execução e compilação desta entrega ainda precisam do checklist local.
