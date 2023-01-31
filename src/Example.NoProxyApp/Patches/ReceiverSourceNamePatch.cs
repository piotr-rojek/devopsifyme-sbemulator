using Azure.Messaging.ServiceBus;
using HarmonyLib;
using System.Reflection;

namespace Example.NoProxyApp.Patches
{
    [HarmonyPatch]
    internal static class ReceiverSourceNamePatch
    {
        public static MethodBase TargetMethod()
        {
            var type = typeof(ServiceBusReceiver);
            return AccessTools.FirstConstructor(type, it => it.IsAssembly);
        }

        public static void Prefix(ref string entityPath)
        {
            entityPath = $"queue/{entityPath}";
        }
    }
}
