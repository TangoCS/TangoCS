using System;

namespace Nephrite.Web
{
	public interface ICartItem : IModelObject
	{
		decimal? Price { get; }
		int? ParentObjectID { get; }
	}
}
