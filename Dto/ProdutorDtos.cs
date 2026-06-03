using System.ComponentModel.DataAnnotations;


public class CadastrarProdutorDto
{
    [Required] public string Nome { get; set; } = null!;
    [Required] public string Cpf { get; set; } = null!;
    [EmailAddress] public string? Email { get; set; }
    public string? Telefone { get; set; }
    [Required] public string Senha { get; set; } = null!;
    public string Status { get; set; } = "ATIVO";
}

public class AtualizarProdutorDto
{
    [Required] public string Nome { get; set; } = null!;
    [EmailAddress] public string? Email { get; set; }
    public string? Telefone { get; set; }
    [Required] public string Senha { get; set; } = null!;
    public string Status { get; set; } = "ATIVO";
}
