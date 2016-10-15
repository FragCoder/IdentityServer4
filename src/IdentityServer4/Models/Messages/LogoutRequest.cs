// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;

namespace IdentityServer4.Models.Messages
{
    /// <summary>
    /// Models the request from a client to sign the user out.
    /// </summary>
    public class LogoutRequest : LogoutMessage
    {
        public LogoutRequest(string iframeUrl, LogoutMessage message)
            : base(message)
        {
            SignOutIFrameUrl = iframeUrl;
        }

        /// <summary>
        /// Gets or sets the sign out iframe URL.
        /// </summary>
        /// <value>
        /// The sign out i frame URL.
        /// </value>
        public string SignOutIFrameUrl { get; set; }

        public bool IsAuthenticatedLogout => ClientId.IsPresent();
    }
}