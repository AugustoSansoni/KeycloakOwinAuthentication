﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Owin.Security.Keycloak.Models.Messages
{
    internal abstract class GenericMessage<T>
    {
        protected IOwinRequest Request { get; }
        protected KeycloakAuthenticationOptions Options { get; }

        protected GenericMessage(IOwinRequest request, KeycloakAuthenticationOptions options)
        {
            if (request == null) throw new ArgumentNullException();
            if (options == null) throw new ArgumentNullException();
            Request = request;
            Options = options;
        }

        public abstract Task<T> ExecuteAsync();

        protected async Task<HttpResponseMessage> SendHttpPostRequest(Uri uri, HttpContent content = null)
        {
            if (content != null)
            {
                var test = await content.ReadAsStringAsync();
            }

            HttpResponseMessage response;
            try
            {
                var client = new HttpClient();
                response = await client.PostAsync(uri, content);
            }
            catch (Exception exception)
            {
                throw new Exception("HTTP client URI is inaccessible", exception);
            }

            if (response != null)
            {
                var test = await response.Content.ReadAsStringAsync();
            }

            // Check for HTTP errors
            if (!response.IsSuccessStatusCode)
                throw new Exception("HTTP client returned an error");

            return response;
        }

        protected Task GenerateErrorResponseAsync(ref IOwinResponse response, HttpStatusCode statusCode,
            string errorMessage)
        {
            // Generate error response
            var task = response.WriteAsync(errorMessage);
            response.StatusCode = (int) statusCode;
            response.ContentType = "text/plain";
            return task;
        }
    }
}
