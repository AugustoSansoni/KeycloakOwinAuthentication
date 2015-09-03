﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace Owin.Security.Keycloak.Internal.ClaimMapping
{
    internal static class ClaimMappings
    {
        public static IEnumerable<ClaimLookup> JwtTokenMappings { get; } = new List<ClaimLookup>
        {
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.AccessToken,
                JSelectQuery = "access_token"
            },
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.IdToken,
                JSelectQuery = "id_token"
            },
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.RefreshToken,
                JSelectQuery = "refresh_token"
            },
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.AccessTokenExpiration,
                JSelectQuery = "expires_in",
                Transformation = delegate(JToken token)
                {
                    var expiresInSec = (token.Value<double?>() ?? 1) - 1;
                    var dateTime = DateTime.Now.AddSeconds(expiresInSec);
                    return dateTime.ToString(CultureInfo.InvariantCulture);
                }
            },
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.RefreshTokenExpiration,
                JSelectQuery = "refresh_expires_in",
                Transformation = delegate(JToken token)
                {
                    var expiresInSec = (token.Value<double?>() ?? 1) - 1;
                    var dateTime = DateTime.Now.AddSeconds(expiresInSec);
                    return dateTime.ToString(CultureInfo.InvariantCulture);
                }
            }
        };

        public static IEnumerable<ClaimLookup> AccessTokenMappings { get; } = new List<ClaimLookup>
        {
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.Audience,
                JSelectQuery = "aud"
            },
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.IssuedAt,
                JSelectQuery = "iat",
                Transformation = delegate(JToken token)
                {
                    var unixTime = (token.Value<double?>() ?? 1) - 1;
                    return unixTime.ToDateTime().ToString(CultureInfo.InvariantCulture);
                }
            },
            new ClaimLookup
            {
                ClaimName = ClaimTypes.Role,
                JSelectQuery = "resource_access.{0}.roles",
                IsPluralQuery = true
            }
        };

        public static IEnumerable<ClaimLookup> IdTokenMappings { get; } = new List<ClaimLookup>
        {
            new ClaimLookup
            {
                ClaimName = Constants.ClaimTypes.SubjectId,
                JSelectQuery = "sub"
            },
            new ClaimLookup
            {
                ClaimName = ClaimTypes.Name,
                JSelectQuery = "preferred_username"
            },
            new ClaimLookup
            {
                ClaimName = ClaimTypes.GivenName,
                JSelectQuery = "given_name"
            },
            new ClaimLookup
            {
                ClaimName = ClaimTypes.Surname,
                JSelectQuery = "family_name"
            },
            new ClaimLookup
            {
                ClaimName = ClaimTypes.Email,
                JSelectQuery = "email"
            }
        };
    }
}