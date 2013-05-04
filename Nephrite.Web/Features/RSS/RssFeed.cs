using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Globalization;

namespace Nephrite.Web.RSS
{
    public class RssFeed
    {
        /// <summary>
        /// Название канала
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// URL веб-сайта, связанного с каналом
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Фраза или предложение для описания канала
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Язык, на котором написан канал
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Информация об авторском праве на канал
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Адрес электронной почты веб-мастера
        /// </summary>
        public string WebMaster { get; set; }
        /*
        /// <summary>
        /// Дата публикации текста в канале
        /// </summary>
        public DateTime PubDate { get; set; }

        /// <summary>
        /// Время Последнего изменения содержимого канала
        /// </summary>
        public DateTime LastBuildDate { get; set; }
        */
        /// <summary>
        /// Время жизни; количество минут, на которые канал может кешироваться перед обновлением с ресурса
        /// </summary>
        public int Ttl { get; set; }

        /// <summary>
        /// Элементы канала
        /// </summary>
        public List<RssItem> Items { get; set; }


        public RssFeed()
        {
            Language = "ru-ru";
            //PubDate = DateTime.Now;
            //LastBuildDate = DateTime.Now;
            Ttl = 60;
            Items = new List<RssItem>();
        }

        /// <summary>
        /// Сгенерировать XML
        /// </summary>
        /// <returns></returns>
        public XDocument Generate()
        {
            XDocument xd = new XDocument();
            xd.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            var rss = new XElement("rss");
            rss.SetAttributeValue("version", "2.0");
            //rss.SetAttributeValue("xmlns:atom", "http://www.w3.org/2005/Atom");
            xd.Add(rss);

            var cnl = new XElement("channel");
            rss.Add(cnl);
            
            cnl.Add(new XElement("title", Title));
            cnl.Add(new XElement("link", Link));
            cnl.Add(new XElement("description", Description));
            cnl.Add(new XElement("webMaster", WebMaster));

            // Language
            if (!String.IsNullOrEmpty(Language))
                cnl.Add(new XElement("language", Language));

            // Copyright
            if (!String.IsNullOrEmpty(Copyright))
                cnl.Add(new XElement("copyright", Copyright));

            if (Ttl > 0)
                cnl.Add(new XElement("ttl", Ttl));

            DateTime dt = DateTime.MinValue;

            var x = new XElement("lastBuildDate", DateTime.Now.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " +0400");
            cnl.Add(x);
            foreach (var item in Items)
            {
                var xe = new XElement("item");
                cnl.Add(xe);

                xe.Add(new XElement("title", item.Title));
                xe.Add(new XElement("link", item.Link));
                xe.Add(new XElement("description", item.Description));
                if (!String.IsNullOrEmpty(item.Guid))
                    xe.Add(new XElement("guid", item.Guid));
                if (!String.IsNullOrEmpty(item.Author))
                    xe.Add(new XElement("author", item.Author));
                xe.Add(new XElement("pubDate", item.PubDate.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " +0400"));
                if (item.PubDate > dt)
                    dt = item.PubDate;
            }

            x.AddAfterSelf(new XElement("pubDate", dt.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " +0400"));
            
            return xd;
        }

        /// <summary>
        /// Экспорт в выходной поток
        /// </summary>
        public void Render()
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Write("<?xml version='1.0' encoding='UTF-8'?>" + Generate().ToString());
            response.ContentType = "application/rss+xml";
            response.End();
        }
    }
}
