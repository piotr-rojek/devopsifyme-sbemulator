using HarmonyLib;
using Microsoft.Azure.Amqp;

namespace Example.NoProxyApp.Patches
{
    [HarmonyPatch(typeof(SendingAmqpLink), MethodType.Constructor, new[] { typeof(AmqpSession), typeof(AmqpLinkSettings) })]
    internal static class SetMaxMessageSizePatch
    {
        static void Prefix(ref AmqpSession session, ref AmqpLinkSettings settings)
        {
            settings.MaxMessageSize = Int32.MaxValue;
        }
    }
}
