//This file has been borrowed from
//https://github.com/lfoust/AccountControllerTests
//It is implementing an interface we use that is a static interface
//This is done so we can do unit testing on our controllers.

namespace SocialNetworkApp.Models
{
    using System.Security.Principal;
    using System.Web;
    using WebMatrix.WebData;

    public class WebSecurityWrapper : IWebSecurity
    {
        public bool Login(string userName, string password, bool persistCookie = false)
        {
            return WebSecurity.Login(userName, password, persistCookie);
        }

        public void Logout()
        {
            WebSecurity.Logout();
        }

        public string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
        {
            return WebSecurity.CreateUserAndAccount(userName, password, propertyValues);
        }

        public int GetUserId(string userName)
        {
            return WebSecurity.GetUserId(userName);
        }

        public bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return WebSecurity.ChangePassword(userName, currentPassword, newPassword);
        }

        public string CreateAccount(string userName, string password, bool requireConfirmationToken = false)
        {
            return WebSecurity.CreateAccount(userName, password, requireConfirmationToken);
        }

        public IPrincipal CurrentUser
        {
            get { return HttpContext.Current.User; }
        }
    }
}