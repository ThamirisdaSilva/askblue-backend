namespace ApiCampanhas.Contratos;

// Formato do JSON recebido. Não é uma tabela nem a entidade de domínio.
public record CriarCampanhaRequest(string? Nome, string? Mensagem);
