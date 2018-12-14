﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Omise.Models;
using System.IO;
using System.Text;

namespace Omise
{
    public class Requester : IRequester
    {
        readonly string userAgent;

        public Credentials Credentials { get; private set; }
        public string APIVersion { get; set; }

        public IRoundtripper Roundtripper { get; private set; }
        public IEnvironment Environment { get; private set; }
        public Serializer Serializer { get; private set; }

        public Requester(
            Credentials creds,
            IEnvironment env,
            IRoundtripper roundtripper = null,
            string apiVersion = null
        )
        {
            if (creds == null) throw new ArgumentNullException(nameof(creds));
            if (env == null) throw new ArgumentNullException(nameof(env));

            userAgent = buildRequestMetadata()
                .Aggregate("", (acc, pair) => $"{acc} {pair.Key}/{pair.Value}")
                .Trim();

            Credentials = creds;
            APIVersion = apiVersion;
            Environment = env;

            Roundtripper = roundtripper ?? new DefaultRoundtripper();
            Serializer = new Serializer();
        }

        IDictionary<string, string> buildRequestMetadata()
        {
            var thisAsmName = GetType().GetTypeInfo().Assembly.GetName();
            var clrAsmName = typeof(object).GetTypeInfo().Assembly.GetName();

            return new Dictionary<string, string>
            {
                { "Omise.Net", thisAsmName.Version.ToString() },
                { ".Net", clrAsmName.Version.ToString() },
            };
        }


        public async Task<TResult> Request<TResult>(
            Endpoint endpoint,
            string method,
            string path)
            where TResult : class
        {
            return await Request<object, TResult>(endpoint, method, path, null);
        }

        public async Task<TResult> Request<TPayload, TResult>(
            Endpoint endpoint,
            string method,
            string path,
            TPayload payload)
            where TPayload : class
            where TResult : class
        {
            var apiPrefix = Environment.ResolveEndpoint(endpoint);
            var key = Environment.SelectKey(endpoint, Credentials);

            // creates initial request
            // TODO: Dispose request.
            var request = Roundtripper.CreateRequest(method, apiPrefix + path);
            request.Headers.Add("Authorization", key.EncodeForAuthorizationHeader());
            request.Headers.Add("User-Agent", userAgent);

            if (!string.IsNullOrEmpty(APIVersion)) request.Headers.Add("Omise-Version", APIVersion);
            if (payload != null)
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.JsonSerialize(ms, payload);

                    var buffer = ms.ToArray();
                    var content = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    request.Content = new StringContent(content, Encoding.UTF8, "application/json");
                }
            }

            // roundtrips the request
            string s_content = default(string);
            try
            {
                var response = await Roundtripper.Roundtrip(request);
                var stream = await response.Content.ReadAsStreamAsync();
                s_content = GenerateStreamToString(stream);
                stream.Position = 0;
                if (!response.IsSuccessStatusCode)
                {
                    var error = Serializer.JsonDeserialize<ErrorResult>(stream);
                    error.HttpStatusCode = response.StatusCode;
                    throw new OmiseError(error, null);
                }

                s_content = GenerateStreamToString(stream);
                stream.Position = 0;
                var result = Serializer.JsonDeserialize<TResult>(stream);
                var model = result as ModelBase;
                if (model != null)
                {
                    model.Requester = this;
                }

                return result;
            }
            catch (HttpRequestException e)
            {
                throw new OmiseException("Error while making HTTP request", e);
            }
            catch (Newtonsoft.Json.JsonReaderException j)
            {
                throw new OmiseException("Error Unexpected character encountered while parsing value is " + s_content, j);
            }
            catch (Exception ex)
            {
                throw new OmiseException(ex.Message, ex);
            }
        }

        public static Stream GenerateStringToStream(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string GenerateStreamToString(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            return new StreamReader(s).ReadToEnd();
        }
    }
}