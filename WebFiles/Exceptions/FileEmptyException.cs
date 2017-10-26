using System;

namespace Cstieg.WebFiles
{
    public class FileEmptyException : Exception
    {
        public FileEmptyException() : base() { }

        public FileEmptyException(string message = "File is empty") : base(message) { }
    }
}