
using BlazorApp.Models.Account;
using Client.Models;
using Client.Models.Account;
using Client.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Services
{
    public interface IAccountService
    {
        User User { get; }
        Task Initialize();
        Task Login(Login model);
        Task Logout();
        Task Register(AddUser model);
        Task<IList<User>> GetAll();
        Task<User> GetById(string id);
        Task Update(string id, EditUser model);
        Task Delete(string id);
    }

    public class AccountService : IAccountService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private string _userKey = "user";

        public User User { get; private set; }

        public AccountService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            User = new User();
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<User>(_userKey);
        }

        public async Task Login(Login model)
        {
            User = await _httpService.Post<User>("/api/auth", model);
            await _localStorageService.SetItem(_userKey, User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(_userKey);
            _navigationManager.NavigateTo("/");
        }

        public async Task Register(AddUser model)
        {
            await _httpService.Post("/api/signup", model);
        }

        public async Task<IList<User>> GetAll()
        {
            return await _httpService.Get<IList<User>>("/users");
        }

        public async Task<User> GetById(string username)
        {
            return await _httpService.Get<User>($"/users/{username}");
        }

        public async Task Update(string username, EditUser model)
        {
            await _httpService.Put($"/users/{username}", model);

            // update stored user if the logged in user updated their own record
            if (username == User.Username) 
            {
                // update local storage
                
                User.Email = model.Email;
                User.Username = model.Username;
                await _localStorageService.SetItem(_userKey, User);
            }
        }

        public async Task Delete(string username)
        {
            await _httpService.Delete($"/users/{username}");

            // auto logout if the logged in user deleted their own record
            if (username == User.Username)
                await Logout();
        }
    }
}