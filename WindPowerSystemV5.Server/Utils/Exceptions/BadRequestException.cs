namespace WindPowerSystemV5.Server.Utils.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
