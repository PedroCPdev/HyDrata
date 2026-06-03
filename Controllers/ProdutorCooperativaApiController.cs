using HyDrata.GestaoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/produtorcooperativa")]
public class ProdutorCooperativaApiController : ControllerBase
{
    private readonly IProdutorCooperativaRepository _repo;
    private readonly IProdutorRepository            _produtorRepo;
    private readonly ICooperativaRepository         _cooperativaRepo;

    public ProdutorCooperativaApiController(
        IProdutorCooperativaRepository repo,
        IProdutorRepository produtorRepo,
        ICooperativaRepository cooperativaRepo)
    {
        _repo            = repo;
        _produtorRepo    = produtorRepo;
        _cooperativaRepo = cooperativaRepo;
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_repo.GetAll().Select(MapToResponse));

    [HttpGet("produtor/{produtorId:int}")]
    public IActionResult GetByProdutor(int produtorId)
        => Ok(_repo.GetByProdutorId(produtorId).Select(MapToResponse));

    [HttpGet("cooperativa/{cooperativaId:int}")]
    public IActionResult GetByCooperativa(int cooperativaId)
        => Ok(_repo.GetByCooperativaId(cooperativaId).Select(MapToResponse));

    [HttpPost]
    public IActionResult Associar([FromBody] AssociarProdutorCooperativaDto dto)
    {
        if (_produtorRepo.GetById(dto.ProdutorId) == null)
            return NotFound(new { erro = $"Produtor com ID {dto.ProdutorId} não encontrado." });

        if (_cooperativaRepo.GetById(dto.CooperativaId) == null)
            return NotFound(new { erro = $"Cooperativa com ID {dto.CooperativaId} não encontrada." });

        if (_repo.ExisteAssociacao(dto.ProdutorId, dto.CooperativaId))
            return Conflict(new { erro = "Esse produtor já está associado a essa cooperativa." });

        var associacao = new ProdutorCooperativa
        {
            ProdutorId    = dto.ProdutorId,
            CooperativaId = dto.CooperativaId
        };

        _repo.Add(associacao);

        var criada = _repo.Get(dto.ProdutorId, dto.CooperativaId)!;
        return StatusCode(StatusCodes.Status201Created, MapToResponse(criada));
    }

    [HttpDelete("{produtorId:int}/{cooperativaId:int}")]
    public IActionResult Desassociar(int produtorId, int cooperativaId)
    {
        var associacao = _repo.Get(produtorId, cooperativaId);
        if (associacao == null) return NotFound("Associação não encontrada.");

        _repo.Delete(associacao);
        return NoContent();
    }

    private static object MapToResponse(ProdutorCooperativa pc) => new
    {
        pc.ProdutorId,
        ProdutorNome    = pc.Produtor?.Nome ?? "",
        pc.CooperativaId,
        CooperativaNome = pc.Cooperativa?.Nome ?? "",
        pc.DataAssociacao
    };
}
