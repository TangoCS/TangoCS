//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace System.ServiceModel.Description
{
	using System.Collections.ObjectModel;

	public class MessageDescriptionCollection : Collection<MessageDescription>
    {
        internal MessageDescriptionCollection()
        {            
        }
        
        public MessageDescription Find(string action)
        {
            foreach (MessageDescription description in this)
            {
                if (description != null && action == description.Action)
                    return description;
            }

            return null;
        }

        public Collection<MessageDescription> FindAll(string action)
        {
            Collection<MessageDescription> descriptions = new Collection<MessageDescription>();
            foreach (MessageDescription description in this)
            {
                if (description != null && action == description.Action)
                    descriptions.Add(description);
            }

            return descriptions;
        }
    }
}
