using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Elasticsearch.Net;

namespace NCI.OCPL.Api.BestBets.Tests.Util
{
    public class ElasticsearchInterceptingConnection : IConnection
    {
        private Dictionary<Type, object> _callbackHandlers = new Dictionary<Type, object>();

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Register a Request Handler for a Given return type.
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="callback"></param>
        public void RegisterRequestHandlerForType<TReturn>(Action<RequestData, ResponseBuilder<TReturn>> callback)
            where TReturn : class
        {
            _callbackHandlers.Add(typeof(TReturn), (object)callback);
        }


        ElasticsearchResponse<TReturn> IConnection.Request<TReturn>(RequestData requestData)            
        {
            //TODO: Fix this to handle proper type inheritance...
            if (!_callbackHandlers.ContainsKey(typeof(TReturn)))
                throw new ArgumentOutOfRangeException("There is no callback handler for defined for type, " + typeof(TReturn).ToString());

            ResponseBuilder<TReturn> builder = new ResponseBuilder<TReturn>(requestData);


            Action<RequestData, ResponseBuilder<TReturn>> callback = (Action<RequestData, ResponseBuilder<TReturn>>)_callbackHandlers[typeof(TReturn)];
            callback(
                requestData, 
                builder
            );

            return builder.ToResponse();
        }

        async Task<ElasticsearchResponse<TReturn>> IConnection.RequestAsync<TReturn>(RequestData requestData)
        {
            if (!_callbackHandlers.ContainsKey(typeof(TReturn)))
                throw new ArgumentOutOfRangeException("There is no callback handler for defined for type, " + typeof(TReturn).ToString());

            ResponseBuilder<TReturn> builder = new ResponseBuilder<TReturn>(requestData);

            Action<RequestData, ResponseBuilder<TReturn>> callback = (Action<RequestData, ResponseBuilder<TReturn>>)_callbackHandlers[typeof(TReturn)];
            callback(
                requestData,
                builder
            );

            return await builder.ToResponseAsync().ConfigureAwait(false);
        }
    }
}
