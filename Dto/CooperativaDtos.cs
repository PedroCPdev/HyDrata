using System.ComponentModel.DataAnnotations;


public class CadastrarCooperativaDto
{
    [Required] public string Nome { get; set; } = null!;
    [EmailAddress] public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string Status { get; set; } = "ATIVA";
}

public class AtualizarCooperativaDto
{
    [Required] public string Nome { get; set; } = null!;
    [EmailAddress] public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string Status { get; set; } = "ATIVA";
}
