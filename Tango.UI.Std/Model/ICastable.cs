using System;
using System.Collections.Generic;
using System.Text;

namespace Tango.Logic
{
    public interface ICastable<TFrom, TTo>
    {
        TTo Cast(TFrom obj);
    }
}
