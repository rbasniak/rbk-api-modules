using System.Collections.Generic;

namespace rbkApiModules.Comments
{
    public interface IUserdataCommentService
    {
        void SetUserdata(List<Comment> comments);
    }
}
