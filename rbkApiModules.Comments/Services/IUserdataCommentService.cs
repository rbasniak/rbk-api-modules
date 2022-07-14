using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Comments
{
    public interface IUserdataCommentService 
    {
        void SetUserdata(List<Comment> comments);
    }

    public abstract class BaseUserdataCommentService
    {
        public virtual void SetUserdata(List<Comment> comments)
        {
            var usernames = comments.Select(x => x.Username).Distinct().ToArray(); 

            foreach (var comment in comments)
            {
                comment.SetUserdata(new BasicCommentInfo(comment.Username, LoadAvatar(comment.Username), comment.Date));
            }
        }

        public abstract string LoadAvatar(string username);
    }
}
