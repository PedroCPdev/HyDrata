using HyDrata.GestaoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/cooperativas")]
public class CooperativasApiController : ControllerBase
{
    private readonly ICooperativaRepository _repo;

    public CooperativasApiController(ICooperativaRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var cooperativas = _repo.GetAll().Select(c => new
        {
            c.Id, c.Nome, c.Email, c.Telefone, c.Status, c.DataCadastro
        });
        return Ok(cooperativas);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var coop = _repo.GetById(id);
        if (coop == null) return NotFound($"Cooperativa {id} não encontrada.");

        return Ok(new { coop.Id, coop.Nome, coop.Email, coop.Telefone, coop.Status, coop.DataCadastro });
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastrarCooperativaDto dto)
    {
        var cooperativa = new Cooperativa
        {
            Nome     = dto.Nome,
            Email    = dto.Email,
            Telefone = dto.Telefone,
            Status   = dto.Status
        };

        _repo.Add(cooperativa);

        return CreatedAtAction(nameof(GetById), new { id = cooperativa.Id }, new
        {
            cooperativa.Id, cooperativa.Nome, cooperativa.Email,
            cooperativa.Telefone, cooperativa.Status, cooperativa.DataCadastro
        });
    }

    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] AtualizarCooperativaDto dto)
    {
        var coop = _repo.GetById(id);
        if (coop == null) return NotFound($"Cooperativa {id} não encontrada.");

        coop.Nome     = dto.Nome;
        coop.Email    = dto.Email;
        coop.Telefone = dto.Telefone;
        coop.Status   = dto.Status;

        _repo.Update(coop);

        return Ok(new { coop.Id, coop.Nome, coop.Email, coop.Telefone, coop.Status, coop.DataCadastro });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Deletar(int id)
    {
        var coop = _repo.GetById(id);
        if (coop == null) return NotFound($"Cooperativa {id} não encontrada.");

        _repo.Delete(id);
        return NoContent();
    }
}
