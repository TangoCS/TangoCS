using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Web.RSS
{
    /// <summary>
    /// Элемент RSS
    /// </summary>
    public class RssItem
    {
        /// <summary>
        /// Заголовок сообщения
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// URL сообщения
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Краткий обзор сообщения
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Строка, уникальным образом идентифицирующая сообщение
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Автор сообщения
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Дата публикации сообщения
        /// </summary>
        public DateTime PubDate { get; set; }
    }
}
