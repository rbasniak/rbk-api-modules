using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.SharedUI
{
    public interface IAuthenticationService
    {
        string User { get; }
        Task Initialize();
        Task Login(string username);
        Task Logout();
    }

    public class AuthenticationService : IAuthenticationService
    {
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;


        public AuthenticationService(NavigationManager navigationManager, ILocalStorageService localStorageService)
        {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public string User { get; set; }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<string>("blazor-admin");
        }

        public async Task Login(string username)
        {
            User = username;
            await _localStorageService.SetItem("blazor-admin", username);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("blazor-admin");
            _navigationManager.NavigateTo("login");
        }
    }
}
