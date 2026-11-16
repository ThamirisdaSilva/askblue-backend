using System.Collections.Concurrent;
using ApiCampanhas.Dominio;

namespace ApiCampanhas.Aplicacao;

public class CampanhaService
{
    // Armazenamento temporário: não é banco de dados.
    // ConcurrentDictionary evita problemas de alteração simultânea da coleção.
    private readonly ConcurrentDictionary<Guid, Campanha> campanhas = new();

    public Campanha Criar(string? nome, string? mensagem)
    {
        // O domínio verifica as regras antes de permitir a criação.
        var campanha = new Campanha(nome, mensagem);
        campanhas[campanha.Id] = campanha;
        return campanha;
    }

    public Campanha[] Listar() => campanhas.Values.ToArray();

    public Campanha? Buscar(Guid id) =>
        campanhas.TryGetValue(id, out var campanha) ? campanha : null;
}
