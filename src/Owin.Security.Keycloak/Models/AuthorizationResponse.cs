﻿using System;
using System.Collections.Specialized;
using System.Web;
using Microsoft.IdentityModel.Protocols;

namespace Owin.Security.Keycloak.Models
{
    internal class AuthorizationResponse : OidcResponse
    {
        public string Code { get; private set; }
        public string State { get; private set; }

        public AuthorizationResponse(string query)
        {
            Init(HttpUtility.ParseQueryString(query));

            if (!Validate())
            {
                throw new ArgumentException("Invalid query string used to instantiate an AuthorizationResponse");
            }
        }

        public AuthorizationResponse(NameValueCollection authResult)
        {
            Init(authResult);
        }

        protected new void Init(NameValueCollection authResult)
        {
            base.Init(authResult);

            Code = authResult.Get(OpenIdConnectParameterNames.Code);
            State = authResult.Get(OpenIdConnectParameterNames.State);
        }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(State);
        }
    }
}
