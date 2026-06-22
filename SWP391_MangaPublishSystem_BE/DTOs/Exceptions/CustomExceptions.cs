using System;
using System.Collections.Generic;

namespace DTOs.Exceptions
{
    public class BadRequestException : Exception
    {
        public List<string>? Errors { get; }

        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}
