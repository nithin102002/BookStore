﻿namespace BookStore.Services
{
    public class BookException : Exception
    {
        public BookException() { }
        public BookException(string message) : base(message) { }
        public BookException(string message, Exception inner) : base(message, inner) { }

    }   
}
