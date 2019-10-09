//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Description
{
	using System.Collections.ObjectModel;
	using System.Xml;

	public class MessagePartDescriptionCollection : KeyedCollection<XmlQualifiedName, MessagePartDescription>
    {
        internal MessagePartDescriptionCollection()
            : base(null, 4)
        {

        }

        protected override XmlQualifiedName GetKeyForItem(MessagePartDescription item)
        {
            return new XmlQualifiedName(item.Name, item.Namespace);
        }
    }
}
