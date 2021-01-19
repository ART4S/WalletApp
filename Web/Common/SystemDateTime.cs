using System;

namespace Web.Common
{
    class SystemDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
