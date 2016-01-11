using System;

namespace Nephrite.Identity
{
	public class Subject<TKey>
	{
		public TKey ID { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public string SID { get; set; }
		public bool MustChangePassword { get; set; }
	}

	public class Subject : Subject<int>
	{

	}
}
