using HyDrata.GestaoApi.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/planos")]
public class PlanosApiController : ControllerBase
{
    private readonly IPlanoRepository _repo;

    public PlanosApiController(IPlanoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var planos = _repo.GetAll().Select(p => new
        {
            p.Id, p.Nome, p.ValorMensalidade, p.Descricao, p.Status
        });
        return Ok(planos);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var plano = _repo.GetById(id);
        if (plano == null) return NotFound($"Plano {id} não encontrado.");

        return Ok(new { plano.Id, plano.Nome, plano.ValorMensalidade, plano.Descricao, plano.Status });
    }

    [HttpPost]
    public IActionResult Cadastrar([FromBody] CadastrarPlanoDto dto)
    {
        var plano = new Plano
        {
            Nome             = dto.Nome,
            ValorMensalidade = dto.ValorMensalidade,
            Descricao        = dto.Descricao,
            Status           = dto.Status
        };

        _repo.Add(plano);

        return CreatedAtAction(nameof(GetById), new { id = plano.Id }, new
        {
            plano.Id, plano.Nome, plano.ValorMensalidade, plano.Descricao, plano.Status
        });
    }

    [HttpPut("{id:int}")]
    public IActionResult Atualizar(int id, [FromBody] AtualizarPlanoDto dto)
    {
        var plano = _repo.GetById(id);
        if (plano == null) return NotFound($"Plano {id} não encontrado.");

        plano.Nome             = dto.Nome;
        plano.ValorMensalidade = dto.ValorMensalidade;
        plano.Descricao        = dto.Descricao;
        plano.Status           = dto.Status;

        _repo.Update(plano);

        return Ok(new { plano.Id, plano.Nome, plano.ValorMensalidade, plano.Descricao, plano.Status });
    }

    [HttpDelete("{id:int}")]
    public IActionResult Deletar(int id)
    {
        var plano = _repo.GetById(id);
        if (plano == null) return NotFound($"Plano {id} não encontrado.");

        try
        {
            _repo.Delete(id);
            return NoContent();
        }
        catch (Exception ex) when (ex.InnerException?.Message.Contains("ORA-02292") == true)
        {
            return Conflict(new { erro = "Não é possível remover: plano está em uso por propriedades." });
        }
    }
}
