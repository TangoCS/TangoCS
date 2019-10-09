//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel
{
	public enum SessionMode
    {
        Allowed,
        Required,
        NotAllowed,
    }

    static class SessionModeHelper
    {
        public static bool IsDefined(SessionMode sessionMode)
        {
            return (sessionMode == SessionMode.NotAllowed ||
                    sessionMode == SessionMode.Allowed ||
                    sessionMode == SessionMode.Required);
        }
    }
}
