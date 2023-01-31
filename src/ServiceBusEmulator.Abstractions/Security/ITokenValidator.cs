namespace ServiceBusEmulator.Abstractions.Security
{
    public interface ITokenValidator
    {
        void Validate(string token);
    }
}
