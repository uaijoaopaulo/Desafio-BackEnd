using Azure.Core;
using Desa.Core.Authentication.Basic.Attributes;
using Desa.Core.Authentication.Resorces;
using Desa.Core.Exceptions;
using Desa.Core.Processors.Interfaces;
using Desa.Core.Repositories.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;

namespace Desa.Rest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MotoController : Controller
    {
        private readonly IMotoProcessor _motoProcessor;
        public MotoController(IMotoProcessor motoProcessor)
        {
            _motoProcessor = motoProcessor;
        }

        [HttpGet("{offset}&{limit}"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(int offset, int limit)
        {
            try
            {
                if (offset < 0)
                    return BadRequest($"{nameof(offset)} size must be equals or greater than 0.");
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user != null && !user.Admin)
                    return Unauthorized();
                return Ok(await _motoProcessor.GetAll(offset, limit));
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpGet("{licensePlate}"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string licensePlate)
        {
            try
            {
                if (string.IsNullOrEmpty(licensePlate))
                    return BadRequest($"{nameof(licensePlate)} must be informed.");

                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user != null && !user.Admin)
                    return Unauthorized();
                return Ok(await _motoProcessor.GetByLicensePlate(licensePlate));
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("CadastrarMoto"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CadastroMoto([FromBody]MotoModel request)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user != null && !user.Admin)
                    return Unauthorized();

                await _motoProcessor.CadastrarNovaMoto(request);

                return Created();
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpPut("AtualizarMoto"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarMoto([FromRoute] string licensePlate, [FromBody]MotoModel request)
        {
            try
            {
                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user != null && !user.Admin)
                    return Unauthorized();

                await _motoProcessor.UpdateMoto(request);

                return Accepted();
            }
            catch (Exception e)
            {
                if (e is DesaException desaException)
                    return StatusCode(desaException.StatusCode, desaException.Message);
                return StatusCode(500);
            }
        }

        [HttpDelete("DeleteMoto"), BasicAuthorizationAttributes]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletarMoto(string licensePlate)
        {
            try
            {
                if (string.IsNullOrEmpty(licensePlate))
                    return BadRequest($"licensePlate must be informed.");

                var user = JsonSerializer.Deserialize<UserModel>(HttpContext.User.FindFirstValue(CustomClaimTypes.User) ?? "");
                if (user != null && !user.Admin)
                    return Unauthorized();

                await _motoProcessor.RemoveMoto(licensePlate);

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