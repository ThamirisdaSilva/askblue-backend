using ApiCampanhas.Aplicacao;
using ApiCampanhas.Contratos;
using ApiCampanhas.Dominio;

var builder = WebApplication.CreateBuilder(args);

// Permitem gerar a descrição dos endpoints e mostrar o Swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Uma instância compartilhada mantém os dados enquanto esta aplicação roda.
builder.Services.AddSingleton<CampanhaService>();

var app = builder.Build();

// Documentação interativa apenas no ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/campanhas", (CampanhaService service) =>
    Results.Ok(service.Listar()))
    .Produces<Campanha[]>(StatusCodes.Status200OK);

app.MapGet("/campanhas/{id:guid}", IResult (Guid id, CampanhaService service) =>
{
    var campanha = service.Buscar(id);
    return campanha is null ? Results.NotFound() : Results.Ok(campanha);
})
.Produces<Campanha>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/campanhas", IResult (CriarCampanhaRequest pedido, CampanhaService service) =>
{
    try
    {
        var campanha = service.Criar(pedido.Nome, pedido.Mensagem);
        return Results.Created($"/campanhas/{campanha.Id}", campanha);
    }
    catch (ArgumentException erro)
    {
        return Results.BadRequest(new { erro = erro.Message });
    }
})
.Accepts<CriarCampanhaRequest>("application/json")
.Produces<Campanha>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.Run();
