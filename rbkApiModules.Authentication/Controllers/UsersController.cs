using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Api.Controllers;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace AspNetCoreApiTemplate.Api
{
    /// <summary>
    /// Controller para acessar as funcionalidades de usuário
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Lista os usuários 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Users.ListItem[]>> All()
        {
            return HttpResponse<Users.ListItem[]>(await Mediator.Send(new GetAllUsers.Command()));
        }

        /// <summary>
        /// Lista os detalhes de um usuário
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Users.Details>> Details(Guid id)
        {
            return HttpResponse<Users.Details>(await Mediator.Send(new GetUserDetails.Command { Id = id }));
        }

        ///// <summary>
        ///// Cria um novo usuário
        ///// </summary>
        //[HttpPost]
        //public async Task<ActionResult<SimpleNamedEntity>> Create(CreateUser.Command data)
        //{
        //    return HttpResponse<SimpleNamedEntity>(await Mediator.Send(data));
        //}

        /// <summary>
        /// Sobrescreve um controle de acesso em um usuário
        /// </summary>
        [HttpPost("claim/add")]
        public async Task<ActionResult> AddClaim(AddClaimToUser.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Remove um controle de acesso sobrescrito em um usuário
        /// </summary>
        [HttpPost("claim/remove")]
        public async Task<ActionResult> RemoveClaim(RemoveClaimFromUser.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Adiciona uma regra de acesso a um usuário
        /// </summary>
        [HttpPost("role/add")]
        public async Task<ActionResult> AddRole(AddClaimToUser.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        /// <summary>
        /// Remove uma regra de acesso de um usuário
        /// </summary>
        [HttpPost("role/remove")]
        public async Task<ActionResult> RemoveRole(RemoveClaimFromUser.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        } 
    }
}
