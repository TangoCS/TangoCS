//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Description
{
	public class MessagePropertyDescription : MessagePartDescription
    {
        public MessagePropertyDescription(string name)
            : base(name, "")
        {
        }

        internal MessagePropertyDescription(MessagePropertyDescription other)
            : base(other)
        {
        }

        internal override MessagePartDescription Clone()
        {
            return new MessagePropertyDescription(this);
        }
    }
}
