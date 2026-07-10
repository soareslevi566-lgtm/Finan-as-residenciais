using ControleGastos.Api.DTOs; using ControleGastos.Api.Services; using Microsoft.AspNetCore.Mvc;
namespace ControleGastos.Api.Controllers;
[ApiController, Route("api/transacoes")]
public class TransacoesController(TransacaoService service) : ControllerBase
{
 [HttpGet] public async Task<ActionResult<IReadOnlyList<TransacaoDto>>> Listar([FromQuery]int? pessoaId, CancellationToken ct) => Ok(await service.ListarAsync(pessoaId, ct));
 [HttpPost] public async Task<ActionResult<TransacaoDto>> Criar(CriarTransacaoDto dto, CancellationToken ct) { var t=await service.CriarAsync(dto,ct); return Created($"/api/transacoes/{t.Id}",t); }
}
