//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
namespace System.ServiceModel.Description
{
	using System.Collections.ObjectModel;

	public class FaultDescriptionCollection : Collection<FaultDescription>
    {
        internal FaultDescriptionCollection()
        {            
        }

        public FaultDescription Find(string action)
        {
            foreach (FaultDescription description in this)
            {
                if (description != null && action == description.Action)
                    return description;
            }

            return null;
        }

        public Collection<FaultDescription> FindAll(string action)
        {
            Collection<FaultDescription> descriptions = new Collection<FaultDescription>();
            foreach (FaultDescription description in this)
            {
                if (description != null && action == description.Action)
                    descriptions.Add(description);
            }

            return descriptions;
        }
    }
}
