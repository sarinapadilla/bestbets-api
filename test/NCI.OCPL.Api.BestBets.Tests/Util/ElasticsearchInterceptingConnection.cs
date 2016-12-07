using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Elasticsearch.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NCI.OCPL.Api.BestBets.Tests.Util
{
    public class ElasticsearchInterceptingConnection : IConnection
    {
        private Dictionary<Type, object> _callbackHandlers = new Dictionary<Type, object>();
        private Action<RequestData, object> _defCallbackHandler = null;

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

        public void RegisterDefaultHandler(Action<RequestData, object> callback)
        {
            if (_defCallbackHandler != null)
                throw new ArgumentException("Cannot add more than one default handler");

            this._defCallbackHandler = callback;
        }

        private void ProcessRequest<TReturn>(RequestData requestData, ResponseBuilder<TReturn> builder)
            where TReturn : class
        {
            //TODO: Fix this to handle proper type inheritance...
            if (!_callbackHandlers.ContainsKey(typeof(TReturn)) && _defCallbackHandler == null)
                throw new ArgumentOutOfRangeException("There is no callback handler for defined for type, " + typeof(TReturn).ToString());

            if (_callbackHandlers.ContainsKey(typeof(TReturn))) {
                Action<RequestData, ResponseBuilder<TReturn>> callback =
                    (Action<RequestData, ResponseBuilder<TReturn>>)_callbackHandlers[typeof(TReturn)];

                callback(
                    requestData,
                    builder
                );
            }
            else
            {
                _defCallbackHandler(
                    requestData,
                    builder
                );
            }
        }

        ElasticsearchResponse<TReturn> IConnection.Request<TReturn>(RequestData requestData)            
        {
            ResponseBuilder<TReturn> builder = new ResponseBuilder<TReturn>(requestData);

            this.ProcessRequest<TReturn>(requestData, builder);

            return builder.ToResponse();
        }

        async Task<ElasticsearchResponse<TReturn>> IConnection.RequestAsync<TReturn>(RequestData requestData)
        {

            ResponseBuilder<TReturn> builder = new ResponseBuilder<TReturn>(requestData);

            this.ProcessRequest<TReturn>(requestData, builder);

            return await builder.ToResponseAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Helper function to extract the postbody (as JObject) from a request.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public object GetRequestPost(RequestData requestData)
        {
            //Some requests can have this as null.  That is ok...
            if (requestData.PostData == null)
                return null;

            String postBody = string.Empty;

            using (MemoryStream stream = new MemoryStream())
            {

                //requestData.PostBody
                requestData.PostData.Write(stream, requestData.ConnectionSettings);

                postBody = Encoding.UTF8.GetString(stream.ToArray());

            }

            dynamic postObj = JObject.Parse(postBody);

            return postObj;
        }
    }
}
