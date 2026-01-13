using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SharedModule.HttpApi.Helpers;

namespace EleonsoftProxy.Client
{
    public sealed class ApiClientOptions
    {
        private readonly List<TokenBase> _tokens = new();
        private readonly List<Action<HttpClient>> _httpClientConfigurators = new();
        private readonly List<Action<IHttpClientBuilder>> _httpClientBuilderConfigurators = new();
        private Action<JsonSerializerOptions>? _jsonOptionsConfigurator;
        private Type? _tokenProviderType;

        public EleoncoreSdkConfig SdkConfig { get; } = new();

        public IReadOnlyList<TokenBase> Tokens => _tokens;
        internal IReadOnlyList<Action<HttpClient>> HttpClientConfigurators => _httpClientConfigurators;
        internal IReadOnlyList<Action<IHttpClientBuilder>> HttpClientBuilderConfigurators => _httpClientBuilderConfigurators;
        internal Action<JsonSerializerOptions>? JsonOptionsConfigurator => _jsonOptionsConfigurator;
        internal Type? TokenProviderType => _tokenProviderType;

        public void AddTokens(params TokenBase[] tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            _tokens.AddRange(tokens);
        }

        public void AddTokens(IEnumerable<TokenBase> tokens)
        {
            if (tokens == null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            _tokens.AddRange(tokens);
        }

        public void UseProvider<TProvider, TToken>()
            where TProvider : TokenProvider<TToken>
            where TToken : TokenBase
        {
            _tokenProviderType = typeof(TProvider);
        }

        public void ConfigureJsonOptions(Action<JsonSerializerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            _jsonOptionsConfigurator = configure;
        }

        public void AddApiHttpClients(Action<HttpClient> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            _httpClientConfigurators.Add(configure);
        }

        public void AddApiHttpClients(Action<IHttpClientBuilder> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            _httpClientBuilderConfigurators.Add(configure);
        }

        public void ConfigureSdk(Action<EleoncoreSdkConfig> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            configure(SdkConfig);
        }
    }
}
