using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Comments.Core;

public class UserdataCommentService : IUserdataCommentService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthService _usersService;
    private readonly Dictionary<string, UserData> _userdata = new();

    public UserdataCommentService(
        IAuthService usersService, 
        IHttpContextAccessor httpContextAccessor)
    {
        _usersService = usersService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SetUserdata(List<Comment> comments, CancellationToken cancellation = default)
    {
        foreach (Comment comment in comments)
        {
            if (_userdata.TryGetValue(comment.Username, out var userInfo))
            {
                comment.SetUserdata(new BasicCommentInfo
                {
                    Username = comment.Username,
                    DisplayName = userInfo.DisplayName,
                    Avatar = userInfo.Avatar,
                    Timestamp = comment.Date
                });
            }
            else
            {
                var user = await _usersService.FindUserAsync(comment.Username, _httpContextAccessor.GetTenant(), cancellation);

                if (user != null)
                {
                    comment.SetUserdata(new BasicCommentInfo
                    {
                        Username = comment.Username,
                        DisplayName = user.DisplayName,
                        Avatar = user.Avatar,
                        Timestamp = comment.Date
                    });

                    _userdata.Add(comment.Username, new UserData(user.DisplayName, user.Avatar));
                }
                else
                {
                    comment.SetUserdata(new BasicCommentInfo
                    {
                        Username = comment.Username,
                        DisplayName = comment.Username,
                        Avatar = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgZmlsbC1ydWxlPSJldmVub2RkIiBjbGlwLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0yNCAyNGgtMjR2LTI0aDI0djI0em0tMi0yMmgtMjB2MjBoMjB2LTIwem0tNC4xMTggMTQuMDY0Yy0yLjI5My0uNTI5LTQuNDI3LS45OTMtMy4zOTQtMi45NDUgMy4xNDYtNS45NDIuODM0LTkuMTE5LTIuNDg4LTkuMTE5LTMuMzg4IDAtNS42NDMgMy4yOTktMi40ODggOS4xMTkgMS4wNjQgMS45NjMtMS4xNSAyLjQyNy0zLjM5NCAyLjk0NS0yLjA0OC40NzMtMi4xMjQgMS40OS0yLjExOCAzLjI2OWwuMDA0LjY2N2gxNS45OTNsLjAwMy0uNjQ2Yy4wMDctMS43OTItLjA2Mi0yLjgxNS0yLjExOC0zLjI5eiIvPjwvc3ZnPg==",
                        Timestamp = comment.Date
                    });
                }
            }
        }
    }
}

public record UserData(string DisplayName, string Avatar);