using GET.Entities;
using GET.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace GET.Hubs
{
    public class LibraryHub : Hub
    {
        private readonly ILibraryService _libraryService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibraryHub(UserManager<ApplicationUser> userManager, ILibraryService libraryService)
        {
            _userManager = userManager;
            _libraryService = libraryService;
        }


        public override async Task OnConnectedAsync()
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var currentUser = await _userManager.GetUserAsync(Context.User);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

