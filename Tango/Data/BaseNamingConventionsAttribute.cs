using System;

namespace Tango.Data
{
    public class BaseNamingConventionsAttribute : Attribute
    {
        public BaseNamingEntityCategory Category { get; set; }
    }
}