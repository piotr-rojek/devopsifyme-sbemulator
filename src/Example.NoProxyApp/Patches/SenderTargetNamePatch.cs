using Azure.Messaging.ServiceBus;
using HarmonyLib;
using System.Reflection;

namespace Example.NoProxyApp.Patches
{
    [HarmonyPatch]
    internal static class SenderTargetNamePatch
    {
        public static MethodBase TargetMethod()
        {
            var type = typeof(ServiceBusSender);
            return AccessTools.FirstConstructor(type, it => it.IsAssembly);
        }

        static void Prefix(ref string entityPath)
        {
            entityPath = $"queue/{entityPath}";
        }
    }
}
