//This file has been borrowed from
//https://github.com/lfoust/AccountControllerTests
//It is implementing an interface we use that is a static interface
//This is done so we can do unit testing on our controllers.

namespace SocialNetworkApp.Models
{
    using System.Collections.Generic;
    using DotNetOpenAuth.AspNet;
    using Microsoft.Web.WebPages.OAuth;

    public interface IOAuthWebSecurity
    {
        string GetUserName(string providerName, string providerUserId);
        bool HasLocalAccount(int userId);
        ICollection<OAuthAccount> GetAccountsFromUserName(string userName);
        bool DeleteAccount(string providerName, string providerUserId);
        AuthenticationResult VerifyAuthentication(string returnUrl);
        bool Login(string providerName, string providerUserId, bool createPersistentCookie);
        void CreateOrUpdateAccount(string providerName, string providerUserId, string userName);
        string SerializeProviderUserId(string providerName, string providerUserId);
        AuthenticationClientData GetOAuthClientData(string providerName);
        bool TryDeserializeProviderUserId(string data, out string providerName, out string providerUserId);
        ICollection<AuthenticationClientData> RegisteredClientData { get; }
        void RequestAuthentication(string provider, string returnUrl);
    }
}