using System.ComponentModel.DataAnnotations;


public class CadastrarPropriedadeDto
{
    [Required] public int ProdutorId { get; set; }
    [Required] public int PlanoId { get; set; }
    [Required] public string Nome { get; set; } = null!;
    [Required] public decimal AreaHectares { get; set; }
    public string? Cidade { get; set; }
    [StringLength(2)] public string? Estado { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Status { get; set; } = "ATIVA";
}

public class AtualizarPropriedadeDto
{
    [Required] public int PlanoId { get; set; }
    [Required] public string Nome { get; set; } = null!;
    [Required] public decimal AreaHectares { get; set; }
    public string? Cidade { get; set; }
    [StringLength(2)] public string? Estado { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string Status { get; set; } = "ATIVA";
}
