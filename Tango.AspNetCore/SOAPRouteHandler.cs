using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Tango.AspNetCore
{
	public static partial class Handlers
	{
        const int messagesize = 120 * 1024 * 1024; // максимальный размер данных

		static MessageEncoder GetEncoder(Binding binding) =>
			binding.CreateBindingElements().Find<MessageEncodingBindingElement>()?.CreateMessageEncoderFactory().Encoder;

		static XmlDictionaryReaderQuotas ReaderQuotas => new XmlDictionaryReaderQuotas {
			MaxArrayLength = messagesize,
			MaxBytesPerRead = messagesize,
			MaxStringContentLength = messagesize
		};

		public static async Task SoapHandler12<T>(HttpContext context)
		{
			if (context.Request.Method == "GET")
			{
				await SendWSDL<T>(context);
				return;
			}

			try
			{
				var binding = new WSHttpBinding
				{
					ReaderQuotas = ReaderQuotas,
					OpenTimeout = new TimeSpan(0, 3, 0),
					CloseTimeout = new TimeSpan(0, 3, 0),
					SendTimeout = new TimeSpan(0, 10, 0),
					ReceiveTimeout = new TimeSpan(0, 10, 0),
				};

				var encoder = GetEncoder(binding);
				var requestMessage = encoder.ReadMessage(context.Request.Body, 0x10000, context.Request.ContentType);
				var ct = context.Request.ContentType;
				var i = ct.IndexOf("action=\"") + 8;
				requestMessage.Headers.Action = ct.Substring(i, ct.IndexOf("\"", i) - i);
				var responseMessage = Run<T>(context.RequestServices, requestMessage, false);
				context.Response.ContentType = context.Request.ContentType;
				encoder.WriteMessage(responseMessage, context.Response.Body);
			}
			catch (Exception e)
			{
				await context.Response.WriteAsync(e.ToString());
			}
			await Task.FromResult(0);
		}

		public static async Task SoapHandler11<T>(HttpContext context)
		{
			if (context.Request.Method == "GET")
			{
				await SendWSDL<T>(context);
				return;
			}

			try
			{
                CustomBinding cust = new CustomBinding(new BasicHttpBinding
                {
                    ReaderQuotas = ReaderQuotas,
                    OpenTimeout = new TimeSpan(0, 3, 0),
                    CloseTimeout = new TimeSpan(0, 3, 0),
                    SendTimeout = new TimeSpan(0, 10, 0),
                    ReceiveTimeout = new TimeSpan(0, 10, 0),
                });
				MessageEncodingBindingElement enc = cust.Elements.Find<MessageEncodingBindingElement>();
                enc.MessageVersion = MessageVersion.Soap11WSAddressing10;

				var encoder = GetEncoder(cust);
				var requestMessage = encoder.ReadMessage(context.Request.Body, 0x10000, context.Request.ContentType);
				requestMessage.Headers.Action = context.Request.Headers["SOAPAction"].ToString().Trim('\"');
				var responseMessage = Run<T>(context.RequestServices, requestMessage, true);
				context.Response.Headers["SOAPAction"] = responseMessage.Headers.Action;
				context.Response.ContentType = context.Request.ContentType;
                responseMessage.Headers.Clear();
                encoder.WriteMessage(responseMessage, context.Response.Body);
			}
			catch (Exception e)
			{
				await context.Response.WriteAsync(e.ToString());
			}

			await Task.FromResult(0);
		}

        public static async Task SoapHandler11AuthHeader<T, Th>(HttpContext context, string login, string password)
        {
            try
            {
                CustomBinding cust = new CustomBinding(new BasicHttpBinding
                {
                    ReaderQuotas = new XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = messagesize,
                        MaxBytesPerRead = messagesize,
                        MaxStringContentLength = messagesize
                    },
                    OpenTimeout = new TimeSpan(0, 3, 0),
                    CloseTimeout = new TimeSpan(0, 3, 0),
                    SendTimeout = new TimeSpan(0, 10, 0),
                    ReceiveTimeout = new TimeSpan(0, 10, 0),
                });
                MessageEncodingBindingElement enc = cust.Elements.Find<MessageEncodingBindingElement>();
                enc.MessageVersion = MessageVersion.Soap11WSAddressing10;

                var encoder = cust.CreateBindingElements().Find<MessageEncodingBindingElement>()?.CreateMessageEncoderFactory().Encoder;

                var requestMessage = encoder.ReadMessage(context.Request.Body, 0x10000, context.Request.ContentType);
                requestMessage.Headers.Action = context.Request.Headers["SOAPAction"].ToString().Trim('\"');

                string l = "";
                string p = "";
                int idx = 0;
                foreach(var header in requestMessage.Headers)
                {
                    if (header.Name == typeof(Th).Name)
                    {
                        Th h = requestMessage.Headers.GetHeader<Th>(header.Name, header.Namespace);
                        var type = h.GetType();
                        l = (string)type.InvokeMember("Login", BindingFlags.GetProperty, null, h, null);
                        p = (string)type.InvokeMember("Password", BindingFlags.GetProperty, null, h, null);
                        break;
                    }
                    idx++;
                }
                Message responseMessage;
                if (l == login && p == password)
                    responseMessage = Run<T>(context.RequestServices, requestMessage, true);
                else
                    responseMessage = Message.CreateMessage(requestMessage.Version, new FaultCode("EXCEPTION"), "Ошибка аутентификации: неправильный логин или пароль", "");

                context.Response.Headers["SOAPAction"] = responseMessage.Headers.Action;
                context.Response.ContentType = context.Request.ContentType;
                responseMessage.Headers.Clear();
                encoder.WriteMessage(responseMessage, context.Response.Body);
            }
            catch (Exception e)
            {
                await context.Response.WriteAsync(e.ToString());
            }

            await Task.FromResult(0);
        }

		static async Task SendWSDL<T>(HttpContext context)
		{
			var env = context.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
			var localPath = $"{Path.DirectorySeparatorChar}wsdls{Path.DirectorySeparatorChar}{typeof(T).Name}.wsdl";
			var path = $"{env.WebRootPath}{localPath}";
			if (File.Exists(path))
				await context.Response.WriteAsync(File.ReadAllText(path));
			else
				await context.Response.WriteAsync($"WSDL not found ({localPath})");
		}

		static Message Run<T>(IServiceProvider srv, Message requestMessage, bool addWrapper)
		{
			OperationDescription operation = null;
            try
            {
                var serviceType = typeof(T);
                // Get service type
                var serviceInstance = srv.GetService(serviceType);
                if (serviceInstance == null)
                {
                    throw new InvalidOperationException($"No service found for specified type: {serviceType.Name}");
                }
                if (serviceInstance is IWithPropertyInjection)
                    (serviceInstance as IWithPropertyInjection).InjectProperties(srv);

                //var service = ServiceDescription.GetService(serviceInstance.GetType());
                var contract = ContractDescription.GetContract(serviceType, serviceInstance.GetType());
                operation = contract.Operations.Where(o => o.Messages.First(m => m.Direction == MessageDirection.Input)
                    .Action.Equals(requestMessage.Headers.Action, StringComparison.Ordinal)).FirstOrDefault();
                if (operation == null)
                {
                    throw new InvalidOperationException($"No operation found for specified action: {requestMessage.Headers.Action}");
                }

                // get service attribute behavior
                var attrmethod = operation.SyncMethod.GetCustomAttribute<ServiceMethodBehaviorAttribute>(true);
                if (attrmethod != null)
                {
                    var bufferedcopy = requestMessage.CreateBufferedCopy(int.MaxValue);

                    var message = bufferedcopy.CreateMessage();
                    // run attribute method after receive request
                    attrmethod.AfterReceiveRequest(ref message);

                    requestMessage = bufferedcopy.CreateMessage();
                }

                // Get operation arguments from message
                var parameters = operation.SyncMethod.GetParameters();
                var arguments = new List<object>();

                bool isxmlserializer = operation.SyncMethod.GetCustomAttribute<XmlSerializerFormatAttribute>() != null ||
                                       contract.ContractType.GetCustomAttribute<XmlSerializerFormatAttribute>() != null;
                
                // Deserialize request wrapper and object
                using (var xmlReader = requestMessage.GetReaderAtBodyContents())
				{
					// Find the element for the operation's data
					xmlReader.ReadStartElement(operation.Name, operation.DeclaringContract.Namespace);

                    if (isxmlserializer)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            string name = parameters[i].GetCustomAttribute<XmlElementAttribute>()?.ElementName;
                            string parameterName = string.IsNullOrWhiteSpace(name) ? parameters[i].Name : name;
                            string nameSpace = parameters[i].GetCustomAttribute<XmlElementAttribute>()?.Namespace ?? operation.DeclaringContract.Namespace;

                            xmlReader.MoveToStartElement(parameterName, nameSpace);
                            if (xmlReader.IsStartElement(parameterName, nameSpace))
                            {
                                var serializer = new XmlSerializer(parameters[i].ParameterType, nameSpace);
                                var obj = serializer.Deserialize(xmlReader);
                                arguments.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            string parameterName = parameters[i].ParameterType.GetCustomAttribute<DataContractAttribute>()?.Name ?? parameters[i].Name;
                            string nameSpace = parameters[i].ParameterType.GetCustomAttribute<DataContractAttribute>()?.Namespace ?? operation.DeclaringContract.Namespace;

                            xmlReader.MoveToStartElement(parameterName, nameSpace);
                            if (xmlReader.IsStartElement(parameterName, nameSpace))
                            {
                                var serializer = new DataContractSerializer(parameters[i].ParameterType, parameterName, nameSpace);
                                var obj = serializer.ReadObject(xmlReader, verifyObjectName: true);
                                arguments.Add(obj);
                            }
                        }
                    }
                }

                // Invoke Operation method
                var responseObject = operation.SyncMethod.Invoke(serviceInstance, arguments.ToArray());

                // Create response message
                bool iswrapped;
                string resultName;
                if (isxmlserializer)
                {
                    iswrapped = operation.SyncMethod.ReturnParameter.GetCustomAttribute<XmlElementAttribute>() != null ? false : addWrapper;
                    resultName = operation.SyncMethod.ReturnParameter.GetCustomAttribute<XmlElementAttribute>()?.ElementName ?? operation.Name + "Result";
                }
                else
                {
                    iswrapped = operation.SyncMethod.ReturnParameter.ParameterType.GetCustomAttribute<MessageContractAttribute>()?.IsWrapped ?? addWrapper;
                    resultName = operation.SyncMethod.ReturnParameter.ParameterType.GetCustomAttribute<MessageContractAttribute>()?.WrapperName ?? operation.Name + "Result";
                }
                var bodyWriter = new ServiceBodyWriter(operation.DeclaringContract.Namespace, iswrapped ? operation.Name + "Response" : null, resultName, responseObject, isxmlserializer);
				var replyAction = operation.Messages.First(m => m.Direction == MessageDirection.Output).Action;

                var responseMessage = Message.CreateMessage(requestMessage.Version, replyAction, bodyWriter);

                if (attrmethod != null)
                    // run attribute method before send response
                    attrmethod.BeforeSendResponse(ref responseMessage);

                return responseMessage;
			}
			catch (Exception e)
			{
				return Message.CreateMessage(requestMessage.Version, new FaultCode("EXCEPTION"), e.ToString(), "");
			}
		}
	}

	public class ServiceBodyWriter : BodyWriter
	{
		string ServiceNamespace;
		string EnvelopeName;
		string ResultName;
		object Result;
        bool isXml;

		public ServiceBodyWriter(string serviceNamespace, string envelopeName, string resultName, object result, bool isxml) : base(isBuffered: true)
		{
			ServiceNamespace = serviceNamespace;
			EnvelopeName = envelopeName;
			ResultName = resultName;
			Result = result;
            isXml = isxml;
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			if (EnvelopeName != null)
				writer.WriteStartElement(EnvelopeName, ServiceNamespace);
			if (Result != null)
			{
                if (isXml)
                {
                    var serializer = new XmlSerializer(Result.GetType(), ServiceNamespace);
                    serializer.Serialize(writer, Result);
                }
                else
                {
                    var serializer = new DataContractSerializer(Result.GetType(), ResultName, ServiceNamespace);
                    serializer.WriteObject(writer, Result);
                }
            }
			else
			{
				writer.WriteStartElement(ResultName, ServiceNamespace);
				writer.WriteEndElement();
			}
			if (EnvelopeName != null)
				writer.WriteEndElement();
		}
	}
}
