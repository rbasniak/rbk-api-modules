using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Comments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : BaseController
    {

        /// <summary>
        /// Cria um novo comentário
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TreeNode[]>> CreateComment([FromBody] CommentEntity.Command data)
        {
            return HttpResponse<TreeNode[]>(await Mediator.Send(data));
        }

        /// <summary>
        /// Retorna a lista de todos os comentários do documento
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TreeNode[]>> AllComments(Guid id)
        {
            return HttpResponse<TreeNode[]>(await Mediator.Send(new GetComments.Command { EntityId = id }));
        }
    }
}
