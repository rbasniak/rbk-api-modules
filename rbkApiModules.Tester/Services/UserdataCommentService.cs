using rbkApiModules.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Tester.Services
{
    public class UserdataCommentService : IUserdataCommentService
    {
        public void SetUserdata(List<Comment> comments)
        {
            foreach (var comment in comments)
            {
                comment.SetUserdata(new { Name = "Fulano" });
            }
        }
    }
}
