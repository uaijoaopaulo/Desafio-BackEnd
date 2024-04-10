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
    public class EntregadorController : Controller
    {
        private readonly IEntregadorProcessor _entregadorProcessor;
        public EntregadorController(IEntregadorProcessor entregadorProcessor)
        {
            _entregadorProcessor = entregadorProcessor;
        }

        [HttpPost("CadastrarEntregador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CadastroEntregador(EntregadorModel request)
        {
            try
            {
                if (request.CNHImage.EndsWith(".png") || request.CNHImage.EndsWith(".bmp"))
                {
                    await _entregadorProcessor.CadastrarNovoEntregador(request);
                    return Created();
                }
                return BadRequest("Formato do arquivo está incorreto.");
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPut("UpdateCNHImage"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCNHImage(string caminhoImagem)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();

                if (caminhoImagem.EndsWith(".png") || caminhoImagem.EndsWith(".bmp"))
                {
                    await _entregadorProcessor.UpdateCNHImageCadastrada(user, caminhoImagem);
                    return Accepted();
                }
                return BadRequest("Formato do arquivo está incorreto.");
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