using System;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Description
{
    public interface IServiceMethodBehavior
    {
        /// <summary>
        /// Метод для обработки сообщения после его получения
        /// </summary>
        /// <param name="request"></param>
        bool AfterReceiveRequest(ref Message request);

        /// <summary>
        /// Метод для обработки сообщения перед его отправкой
        /// </summary>
        /// <param name="response"></param>
        void BeforeSendResponse(ref Message response);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ServiceMethodBehaviorAttribute : Attribute, IServiceMethodBehavior
    {
        public virtual bool AfterReceiveRequest(ref Message request) { return true; }

        public virtual void BeforeSendResponse(ref Message response) { }
    }
}