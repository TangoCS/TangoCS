using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tango.Html;

namespace Tango.RealTime
{
	public static class SignarlRExtensions
	{

		/// <summary>
		/// В фоновом режиме запускается backgroundworker.js, который в свою очередь запускает метод контроллера, процесс выполнения которого необходимо слушать
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="a"></param>
		/// <param name="action">Запускаемый метод контроллера</param>
		/// <param name="actionId">Идентификатор события</param>
		/// <param name="form">Форма с которой запускается backgroundworker.js</param>
		/// <param name="messagemethod">Формат отображения сообщений. По умолчанию = toast (всплывающие сообщения)</param>
		/// <returns></returns>
		public static TagAttributes<T> RunWithSignalR<T>(this TagAttributes<T> a, string action, int actionId, string form, string messageAction = null, MessageMethod messagemethod = MessageMethod.Popupmessage)
			where T : TagAttributes<T>
		{
			return a.Data("action", action).Data("id", actionId).Data("messageaction", messageAction).Data("messagemethod", messagemethod).OnClick($"return backgroundworker.firstinit(this,{form});");
			
		}
		
		
	}
	public enum MessageMethod
	{
		[Description("Всплывающие сообщения")]
		Popupmessage,
		[Description("Сообщения через контроллер")]
		Сontrollermessage
	}

}
