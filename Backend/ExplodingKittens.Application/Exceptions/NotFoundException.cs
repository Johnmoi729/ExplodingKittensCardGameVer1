using System;

namespace ExplodingKittens.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}