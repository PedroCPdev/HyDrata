using System.ComponentModel.DataAnnotations;


public class CadastrarPlanoDto
{
    [Required] public string Nome { get; set; } = null!;
    [Required] public decimal ValorMensalidade { get; set; }
    public string? Descricao { get; set; }
    public string Status { get; set; } = "ATIVO";
}

public class AtualizarPlanoDto
{
    [Required] public string Nome { get; set; } = null!;
    [Required] public decimal ValorMensalidade { get; set; }
    public string? Descricao { get; set; }
    public string Status { get; set; } = "ATIVO";
}
