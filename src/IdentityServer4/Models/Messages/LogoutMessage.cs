using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Validation.Models;

namespace IdentityServer4.Models.Messages
{
    /// <summary>
    /// Models the validated singout context.
    /// </summary>
    public class LogoutMessage
    {
        public LogoutMessage()
        {
        }

        public LogoutMessage(ValidatedEndSessionRequest request)
        {
            if (request != null)
            {
                ClientId = request.Client?.ClientId;
                if (request.PostLogOutUri != null)
                {
                    PostLogoutRedirectUri = request.PostLogOutUri;
                    if (request.State != null)
                    {
                        PostLogoutRedirectUri = PostLogoutRedirectUri.AddQueryString(OidcConstants.EndSessionRequest.State, request.State);
                    }
                }
            }
        }

        public LogoutMessage(LogoutMessage message)
        {
            ClientId = message?.ClientId;
            PostLogoutRedirectUri = message?.PostLogoutRedirectUri;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }
        
        /// <summary>
        /// Gets or sets the post logout redirect URI.
        /// </summary>
        /// <value>
        /// The post logout redirect URI.
        /// </value>
        public string PostLogoutRedirectUri { get; set; }
    }
}