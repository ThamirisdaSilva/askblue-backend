namespace ApiCampanhas.Dominio;

public class Campanha
{
    public Guid Id { get; }
    public string Nome { get; }
    public string Mensagem { get; }

    public Campanha(string? nome, string? mensagem)
    {
        // Regras ilustrativas da demo: confirmar as regras reais com o grupo.
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da campanha é obrigatório.");
        if (string.IsNullOrWhiteSpace(mensagem))
            throw new ArgumentException("A mensagem da campanha é obrigatória.");

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Mensagem = mensagem.Trim();
    }
}
