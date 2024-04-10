using Azure.Core;
using Desa.Core.Authentication.Basic.Attributes;
using Desa.Core.Authentication.Resorces;
using Desa.Core.Exceptions;
using Desa.Core.Processors;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Models;
using Desa.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Text.Json;

namespace Desa.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PedidoController : Controller
    {
        private readonly IPedidoProcessor _pedidoProcessor;
        public PedidoController(IPedidoProcessor pedidoProcessor)
        {
            _pedidoProcessor = pedidoProcessor;
        }

        [HttpGet("EntregadoresNotificados"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EntregadoresNotificados(int idPedido)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || !user.Admin)
                    return Unauthorized();
                return Ok(await _pedidoProcessor.EntregadoresNotificados(idPedido));
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("CriarPedido"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarPedido(PedidoModel pedido)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || !user.Admin)
                    return Unauthorized();
                await _pedidoProcessor.CriarPedido(pedido);
                return Created();
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("AceitarPedido"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AceitarPedido(int idPedido)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();
                await _pedidoProcessor.GerenciarStatusPedido(user, idPedido, OrderSituationEnum.Accepted);
                return Accepted();
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("EntregarPedido"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EntregarPedido(int idPedido)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user == null || user.IdEntregador == null)
                    return Unauthorized();
                await _pedidoProcessor.GerenciarStatusPedido(user, idPedido, OrderSituationEnum.Delivered);
                return Accepted();
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