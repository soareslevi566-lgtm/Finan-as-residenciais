using ControleGastos.Api.DTOs;
using ControleGastos.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace ControleGastos.Api.Controllers;
[ApiController, Route("api/pessoas")]
public class PessoasController(PessoaService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<PessoaDto>>> Listar(CancellationToken ct) => Ok(await service.ListarAsync(ct));
    [HttpPost] public async Task<ActionResult<PessoaDto>> Criar(CriarPessoaDto dto, CancellationToken ct) { var p = await service.CriarAsync(dto, ct); return Created($"/api/pessoas/{p.Id}", p); }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Excluir(int id, CancellationToken ct) { await service.ExcluirAsync(id, ct); return NoContent(); }
}
