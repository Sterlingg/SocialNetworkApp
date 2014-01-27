//This file has been borrowed from
//https://github.com/lfoust/AccountControllerTests
//It is implementing an interface we use that is a static interface
//This is done so we can do unit testing on our controllers.

namespace SocialNetworkApp.Models
{
    using System.Security.Principal;

    public interface IWebSecurity
    {
        bool Login(string userName, string password, bool persistCookie = false);
        void Logout();
        string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false);
        int GetUserId(string userName);
        bool ChangePassword(string userName, string currentPassword, string newPassword);
        string CreateAccount(string userName, string password, bool requireConfirmationToken = false);

        IPrincipal CurrentUser { get; }
    }
}