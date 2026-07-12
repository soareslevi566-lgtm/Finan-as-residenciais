using ControleGastos.Api.DTOs; using ControleGastos.Api.Enums; using ControleGastos.Api.Services; using Microsoft.AspNetCore.Mvc;
namespace ControleGastos.Api.Controllers;
[ApiController, Route("api/transacoes")]
public class TransacoesController(TransacaoService service) : ControllerBase
{
 [HttpGet] public async Task<ActionResult<IReadOnlyList<TransacaoDto>>> Listar([FromQuery]int? pessoaId, [FromQuery]TipoTransacao? tipo, [FromQuery]CategoriaTransacao? categoria, [FromQuery]DateTime? inicio, [FromQuery]DateTime? fim, [FromQuery]string? busca, CancellationToken ct) => Ok(await service.ListarAsync(pessoaId, tipo, categoria, inicio, fim, busca, ct));
 [HttpPost] public async Task<ActionResult<TransacaoDto>> Criar(CriarTransacaoDto dto, CancellationToken ct) { var t=await service.CriarAsync(dto,ct); return Created($"/api/transacoes/{t.Id}",t); }
 [HttpDelete("{id:int}")] public async Task<IActionResult> Excluir(int id, CancellationToken ct) { await service.ExcluirAsync(id, ct); return NoContent(); }
}
