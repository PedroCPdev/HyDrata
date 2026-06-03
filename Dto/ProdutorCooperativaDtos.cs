using System.ComponentModel.DataAnnotations;


public class AssociarProdutorCooperativaDto
{
    [Required] public int ProdutorId { get; set; }
    [Required] public int CooperativaId { get; set; }
}
