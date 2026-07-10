using ControleGastos.Api.DTOs; using ControleGastos.Api.Services; using Microsoft.AspNetCore.Mvc;
namespace ControleGastos.Api.Controllers;
[ApiController, Route("api/totais")]
public class TotaisController(TotaisService service) : ControllerBase
{ [HttpGet] public async Task<ActionResult<TotaisDto>> Obter(CancellationToken ct) => Ok(await service.ObterAsync(ct)); }
