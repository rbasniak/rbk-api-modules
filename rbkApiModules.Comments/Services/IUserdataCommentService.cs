using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Comments
{
    public interface IUserdataCommentService
    {
        void SetUserdata(List<Comment> comments);
    }
}
