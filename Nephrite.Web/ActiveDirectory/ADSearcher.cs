using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;

namespace Nephrite.Web.ActiveDirectory
{
    public static class ADSearcher
    {
        public static List<ADUser> Search(string surname, string firstname, string login)
        {
            DirectorySearcher deSearch = new DirectorySearcher();
			deSearch.SearchRoot = new DirectoryEntry(Settings.ADDomain != String.Empty ? "LDAP://" + Settings.ADDomain : String.Empty);
			deSearch.Filter = String.Format("(&(|(objectClass=user)(objectClass=group))(|(samaccountname={0}*)(givenname={1}*)(sn={2}*)))", PurifyString(login), PurifyString(firstname), PurifyString(surname));
            deSearch.Sort.Direction = SortDirection.Ascending;
            deSearch.Sort.PropertyName = "displayname";
            deSearch.PropertiesToLoad.AddRange(new string[] {"sn", "givenname", "samaccountname", "objectsid", "displayname" });
            SearchResultCollection results = deSearch.FindAll();

            List<ADUser> list = new List<ADUser>();
			foreach (SearchResult result in results)
				list.Add(new ADUser(result));
			
            return list;
        }

		public static ADUser FindByLogin(string login)
		{
			DirectorySearcher deSearch = new DirectorySearcher();
			deSearch.Filter = String.Format("(&(objectClass=user)(samaccountname={0}))", PurifyString(login));
			deSearch.Sort.Direction = SortDirection.Ascending;
			deSearch.Sort.PropertyName = "displayname";
			deSearch.PropertiesToLoad.AddRange(new string[] { "sn", "givenname", "samaccountname", "objectsid", "displayname" });
			SearchResultCollection results = deSearch.FindAll();

			List<ADUser> list = new List<ADUser>();
			foreach (SearchResult result in results)
				return new ADUser(result);

			return null;
		}

        static string PurifyString(string str)
        {
            string chars = "{}()[]&*;\"'!,:%#?+\\/<>";
            for (int i = 0; i < chars.Length; i++)
                str = str.Replace(chars[i].ToString(), String.Empty);
            return str;
        }
    }

    [Serializable]
    public class ADUser
    {
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string Login { get; set; }
        public string Sid { get; set; }
		public string LoginWithDomain
		{
			get
			{
				SecurityIdentifier si = new SecurityIdentifier(Sid);
				return si.Translate(typeof(NTAccount)).Value;
			}
		}

        public ADUser(SearchResult searchResult)
        {
			if (searchResult.Properties["sn"].Count > 0) Surname = searchResult.Properties["sn"][0].ToString();
			if (searchResult.Properties["givenname"].Count > 0) FirstName = searchResult.Properties["givenname"][0].ToString();
			if (searchResult.Properties["samaccountname"].Count > 0) Login = searchResult.Properties["samaccountname"][0].ToString();
			if (searchResult.Properties["objectsid"].Count > 0) Sid = (new SecurityIdentifier((byte[])searchResult.Properties["objectsid"][0], 0)).Value;
			// Определить отчество из DisplayName
			string dn = searchResult.Properties["displayname"].Count > 0 ? searchResult.Properties["displayname"][0].ToString() : "";

			if (!String.IsNullOrEmpty(Surname) && !String.IsNullOrEmpty(FirstName))
			{
				string[] words = dn.Split(' ');
				for (int i = 0; i < words.Length; i++)
					if (words[i].ToLower() == Surname.ToLower() || words[i].ToLower() == FirstName.ToLower())
						words[i] = String.Empty;
				FatherName = String.Join(" ", words).Trim();
			}
			
        }

        public string Title
        {
            get 
			{
				return String.IsNullOrEmpty((Surname + " " + FirstName + " " + FatherName).Trim()) 
					? Login 
					: Surname + " " + FirstName + " " + FatherName + " (" + Login + ")"; 
			}
        }
    }
}
