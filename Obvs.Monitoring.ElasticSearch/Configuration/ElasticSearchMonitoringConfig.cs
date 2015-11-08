using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Obvs.Configuration;

namespace Obvs.Monitoring.ElasticSearch.Configuration
{
    public interface ICanSpecifyElasticSearchMonitoringType<TMessage, TCommand, TEvent, TRequest, TResponse> :
        ICanSpecifyElasticSearchMonitoringIndex<TMessage, TCommand, TEvent, TRequest, TResponse>
        where TMessage : class
            where TCommand : class, TMessage
            where TEvent : class, TMessage
            where TRequest : class, TMessage
            where TResponse : class, TMessage
    {
        ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse> AddCounter<T>() where T : class, TMessage;
    }

    public interface ICanSpecifyElasticSearchMonitoringIndex<TMessage, TCommand, TEvent, TRequest, TResponse>
        where TMessage : class
            where TCommand : class, TMessage
            where TEvent : class, TMessage
            where TRequest : class, TMessage
            where TResponse : class, TMessage
    {
        ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse> SaveToIndex(string indexPrefix);
    }

    public interface ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse>
        where TMessage : class
        where TCommand : class, TMessage
        where TEvent : class, TMessage
        where TRequest : class, TMessage
        where TResponse : class, TMessage
    {
        ICanAddEndpointOrLoggingOrCorrelationOrCreate<TMessage, TCommand, TEvent, TRequest, TResponse> ConnectToServer(string uri);
    }

    internal class ElasticSearchMonitoringConfig<TMessage, TCommand, TEvent, TRequest, TResponse> :
        ICanSpecifyElasticSearchMonitoringType<TMessage, TCommand, TEvent, TRequest, TResponse>,
        ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse>
        where TMessage : class
        where TCommand : class, TMessage
        where TEvent : class, TMessage
        where TRequest : class, TMessage
        where TResponse : class, TMessage
    {
        private readonly ICanAddEndpointOrLoggingOrCorrelationOrCreate<TMessage, TCommand, TEvent, TRequest, TResponse> _config;
        private string _indexPrefix;
        private readonly List<Type> _types = new List<Type>();

        public ElasticSearchMonitoringConfig(ICanAddEndpointOrLoggingOrCorrelationOrCreate<TMessage, TCommand, TEvent, TRequest, TResponse> config)
        {
            _config = config;
        }

        public ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse> SaveToIndex(string indexPrefix)
        {
            _indexPrefix = indexPrefix;
            return this;
        }

        public ICanAddEndpointOrLoggingOrCorrelationOrCreate<TMessage, TCommand, TEvent, TRequest, TResponse> ConnectToServer(string uri)
        {
            _config.UsingMonitor(new ElasticSearchMonitorFactory<TMessage>(uri, _indexPrefix, _types, Scheduler.Default));
            return _config;
        }

        public ICanSpecifyElasticSearchMonitoringUri<TMessage, TCommand, TEvent, TRequest, TResponse> AddCounter<T>() where T : class, TMessage
        {
            _types.Add(typeof(T));
            return this;
        }
    }
}