using rbkApiModules.Comments;
using System.Collections.Generic;

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
