using System;

namespace Tango.Exceptions
{
    public class PropertyInfoNotFoundException : Exception
    {
        public PropertyInfoNotFoundException(string message) : base(message)
        {
        }

        public PropertyInfoNotFoundException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}