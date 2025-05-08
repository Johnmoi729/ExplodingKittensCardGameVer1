using System;

namespace ExplodingKittens.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when an action cannot be performed in the current game state
    /// </summary>
    public class InvalidGameStateException : Exception
    {
        public InvalidGameStateException(string message) : base(message)
        {
        }
    }
}