﻿using Microsoft.AspNetCore.Authorization;

namespace SkippyBackend.Helpers
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string[] Scopes { get; }

        public HasScopeRequirement(string issuer, params string[] scopes)
        {
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }
}