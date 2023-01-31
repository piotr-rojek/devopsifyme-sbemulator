namespace Xim.Simulators.ServiceBus.Security
{
    public interface ITokenValidator
    {
        void Validate(string token);
    }
}
