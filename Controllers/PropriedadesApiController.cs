using HyDrata.GestaoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/propriedades")]
public class PropriedadesApiController : ControllerBase
{
    private readonly IPropriedadeRepository _repo;
    private readonly IProdutorRepository    _produtorRepo;
    private readonly IPlanoRepository       _planoRepo;

    public PropriedadesApiController(
        IPropriedadeRepository repo,
        IProdutorRepository produtorRepo,
        IPlanoRepository planoRepo)
    {
        _repo         = repo;
        _produtorRepo = produtorRepo;
        _planoRepo    = planoRepo;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? produtorId)
    {
        var propriedades = produtorId.HasValue
            ? _repo.GetByProdutorId(produtorId.Value)
            : _repo.GetAll();

        return Ok(propriedades.Select(MapToResponse));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var propriedade = _repo.GetById(id);
        if (propriedade == null) return NotFound($"Propriedade {id} não encontrada.");

        return Ok(MapToResponse(propriedade));
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastrarPropriedadeDto dto)
    {
        if (_produtorRepo.GetById(dto.ProdutorId) == null)
            return NotFound(new { erro = $"Produtor com ID {dto.ProdutorId} não encontrado." });

        if (_planoRepo.GetById(dto.PlanoId) == null)
            return NotFound(new { erro = $"Plano com ID {dto.PlanoId} não encontrado." });

        var propriedade = new Propriedade
        {
            ProdutorId   = dto.ProdutorId,
            PlanoId      = dto.PlanoId,
            Nome         = dto.Nome,
            AreaHectares = dto.AreaHectares,
            Cidade       = dto.Cidade,
            Estado       = dto.Estado,
            Latitude     = dto.Latitude,
            Longitude    = dto.Longitude,
            Status       = dto.Status
        };

        _repo.Add(propriedade);

        var criada = _repo.GetById(propriedade.Id)!;
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, MapToResponse(criada));
    }

    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] AtualizarPropriedadeDto dto)
    {
        var propriedade = _repo.GetById(id);
        if (propriedade == null) return NotFound($"Propriedade {id} não encontrada.");

        if (_planoRepo.GetById(dto.PlanoId) == null)
            return NotFound(new { erro = $"Plano com ID {dto.PlanoId} não encontrado." });

        propriedade.PlanoId      = dto.PlanoId;
        propriedade.Nome         = dto.Nome;
        propriedade.AreaHectares = dto.AreaHectares;
        propriedade.Cidade       = dto.Cidade;
        propriedade.Estado       = dto.Estado;
        propriedade.Latitude     = dto.Latitude;
        propriedade.Longitude    = dto.Longitude;
        propriedade.Status       = dto.Status;

        _repo.Update(propriedade);

        var atualizada = _repo.GetById(id)!;
        return Ok(MapToResponse(atualizada));
    }

    [HttpDelete("{id:int}")]
    public IActionResult Deletar(int id)
    {
        var propriedade = _repo.GetById(id);
        if (propriedade == null) return NotFound($"Propriedade {id} não encontrada.");

        _repo.Delete(id);
        return NoContent();
    }

    private static object MapToResponse(Propriedade p) => new
    {
        p.Id,
        p.ProdutorId,
        ProdutorNome = p.Produtor?.Nome ?? "",
        p.PlanoId,
        PlanoNome    = p.Plano?.Nome ?? "",
        p.Nome,
        p.AreaHectares,
        p.Cidade,
        p.Estado,
        p.Latitude,
        p.Longitude,
        p.Status,
        p.DataCadastro
    };
}
