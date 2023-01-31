using HarmonyLib;
using Microsoft.Azure.Amqp;

namespace Example.NoProxyApp.Patches
{
    [HarmonyPatch(typeof(AmqpCbsLink), "SendTokenAsync", new[] { typeof(ICbsTokenProvider), typeof(Uri), typeof(string), typeof(string), typeof(string[]), typeof(CancellationToken) })]
    internal static class DisableCbsSaslFlowPatch1
    {
        static bool Prefix(ref Task<DateTime> __result)
        {
            __result = Task.FromResult(DateTime.MaxValue);
            return true;
        }
    }
}
