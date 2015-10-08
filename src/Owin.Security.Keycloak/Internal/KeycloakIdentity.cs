﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Owin.Security.Keycloak.Internal.ClaimMapping;
using Owin.Security.Keycloak.Models;
using Owin.Security.Keycloak.Models.Responses;

namespace Owin.Security.Keycloak.Internal
{
    internal class KeycloakIdentity
    {
        private readonly TokenResponse _keycloakToken;

        public KeycloakIdentity(string encodedTokenResponse)
            : this(new TokenResponse(encodedTokenResponse))
        {
        }

        public KeycloakIdentity(TokenResponse tokenResponse)
        {
            _keycloakToken = tokenResponse;
        }

        public async Task<ClaimsIdentity> ValidateIdentity(KeycloakAuthenticationOptions options)
        {
            var uriManager = await OidcDataManager.GetCachedContext(options);
            var signingKeys = uriManager.GetJsonWebKeys();

            // Validate all of the JWTs
            _keycloakToken.IdToken.Validate(signingKeys);
            _keycloakToken.AccessToken.Validate(signingKeys);
            _keycloakToken.RefreshToken.Validate(signingKeys);

            // Create the new claims identity
            return new ClaimsIdentity(GenerateJwtClaims(options), options.SignInAsAuthenticationType);
        }

        public IEnumerable<Claim> GenerateJwtClaims(KeycloakAuthenticationOptions options)
        {
            // Add generic claims
            yield return new Claim(Constants.ClaimTypes.AuthenticationType, options.AuthenticationType);
            yield return new Claim(Constants.ClaimTypes.Version, Global.GetVersion());

            // Save the recieved tokens as claims
            if (options.SaveTokensAsClaims)
            {
                yield return new Claim(Constants.ClaimTypes.IdToken, _keycloakToken.IdToken.EncodedJwt);
                yield return new Claim(Constants.ClaimTypes.AccessToken, _keycloakToken.AccessToken.EncodedJwt);
                yield return new Claim(Constants.ClaimTypes.RefreshToken, _keycloakToken.RefreshToken.EncodedJwt);
            }

            // Add OIDC token claims
            foreach (var claim in ProcessOidcToken(_keycloakToken.IdToken, ClaimMappings.IdTokenMappings))
                yield return claim;
            foreach (var claim in ProcessOidcToken(_keycloakToken.AccessToken, ClaimMappings.AccessTokenMappings))
                yield return claim;
            foreach (var claim in ProcessOidcToken(_keycloakToken.RefreshToken, ClaimMappings.RefreshTokenMappings))
                yield return claim;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Claim> ProcessOidcToken(JsonWebToken webToken, IEnumerable<ClaimLookup> claimMappings)
        {
            // Process claim mappings
            return claimMappings.SelectMany(lookupClaim => lookupClaim.ProcessClaimLookup(webToken.Payload));
        }
    }
}
