using rbkApiModules.Comments;
using System.Collections.Generic;

namespace rbkApiModules.Demo.Services
{
    public class UserdataCommentService : BaseUserdataCommentService, IUserdataCommentService
    {
        public override string LoadAvatar(string username)
        {
            return "XXXXX BASE64 XXXXX";
        }
    }
}
