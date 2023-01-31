using HarmonyLib;
using Microsoft.Azure.Amqp.Sasl;

namespace Example.NoProxyApp.Patches
{
    [HarmonyPatch(typeof(SaslAnonymousHandler), MethodType.Constructor, new[] { typeof(string) })]
    internal static class ForceAnonymousSaslPatch
    {
        static void Prefix(ref string name)
        {
            name = "ANONYMOUS";
        }
    }
}
