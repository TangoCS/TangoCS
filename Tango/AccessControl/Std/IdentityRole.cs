namespace Tango.AccessControl.Std
{
	public class IdentityRole<TKey>
	{
		public TKey Id { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
	}
}
