using Client.Models;
using Client.Models.Account;
using Microsoft.AspNetCore.Components;

namespace Client.Services
{
    public interface IAccountService
    {
        User User { get; }
        Task<User> Initialize();
        Task Login(Login model);
        Task Logout();
        Task Register(AddUser model);
        Task<IList<User>> GetAll();
        Task<User> GetById(string id);
        Task Delete(string id);

        bool GetIsLoggedIn();
    }

    public class AccountService : IAccountService
    {
        private readonly IHttpService _httpService;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalStorageService _localStorageService;
        private string _userKey = "user";
        public static bool isLoggedIn { get; set; }
        public User User { get; private set; }

        public AccountService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task<User> Initialize()
        {
            User = await _localStorageService.GetItem<User>(_userKey);
            return User;
        }

        public bool GetIsLoggedIn()
        {
            return isLoggedIn;
        }

        public async Task Login(Login model)
        {
            User = await _httpService.Post<User>("/api/auth", model);
            await _localStorageService.SetItem(_userKey, User);
            isLoggedIn = true;
            _navigationManager.NavigateTo("/");
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(_userKey);
            isLoggedIn = false;
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

        public async Task Delete(string id)
        {
            await _httpService.Delete($"/users/{id}");
        }
    }
}