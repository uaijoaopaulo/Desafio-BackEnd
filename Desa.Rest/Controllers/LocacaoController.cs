using Azure.Core;
using Desa.Core.Authentication.Basic.Attributes;
using Desa.Core.Authentication.Resorces;
using Desa.Core.Exceptions;
using Desa.Core.Processors;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Text.Json;

namespace Desa.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LocacaoController : Controller
    {
        private readonly ILocacaoProcessor _locacaoProcessor;
        public LocacaoController(ILocacaoProcessor locacaoProcessor)
        {
            _locacaoProcessor = locacaoProcessor;
        }

        [HttpGet("ConsultarMotosDisponiveis"), BasicAuthorizationAttributes]
        public async Task<IActionResult> ConsultarMotosDisponiveis()
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();
                return Ok(await _locacaoProcessor.ConsultarMotosDisponiveisParaLocacao());
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("ValorLocacao"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ValorLocacao(string dataDevolucao) //expecting date format yyyy-mm-dd
        {
            try
            {
                if (!DateTime.TryParse(dataDevolucao, out var date))
                    throw new DesaException(400, "Date format not supported");

                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();
                return Ok(await _locacaoProcessor.CalcularValorLocacao(user, date));
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("LocarMoto"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LocarMoto(int diasLocacao)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();
                await _locacaoProcessor.LocarMoto(user, diasLocacao);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }
    }
}