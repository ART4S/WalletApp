using System;

namespace Web.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string objectName, object id) 
            : base($"{objectName} with id='{id}' not found")
        {
        }
    }
}
