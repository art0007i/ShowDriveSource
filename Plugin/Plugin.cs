using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;

namespace ShowDriveSource;

[ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
[BepInDependency(BepInExResoniteShim.PluginMetadata.GUID, BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BasePlugin
{
#nullable disable
    internal static new ManualLogSource Log;
    internal static ConfigEntry<bool> Pushback;
    internal static ConfigEntry<colorX> TextColor;
#nullable enable

    public override void Load()
    {
        Log = base.Log;

        Pushback = Config.Bind("General", "Pushback", false,
            "If true, clicking on a field will make the new inspector push back the original one.");
        TextColor = Config.Bind("General", "TextColor", colorX.Magenta,
            "Color of the text that pops up when you click a field. (Set alpha to 0 to disable the text)");
        
        HarmonyInstance.PatchAll();
    }
    
    [HarmonyPatch(typeof(Button))]
    [HarmonyPatch("RunPressed")]
    class SyncMemberEditorBuilder_GenerateMemberField_Patch
    {
        public static void Postfix(Button __instance)
        {
            ReferenceProxySource refProxy = __instance.Slot.GetComponent<ReferenceProxySource>();
            if (refProxy == null) return;
            IField field = refProxy.Reference.Target as IField;
            if (field == null || (!field.IsDriven && !field.IsLinked)) return;
            SyncElement syncElement = field.ActiveLink as SyncElement;
            InspectorHelper.OpenInspectorForTarget(syncElement.Component, Pushback.Value ? __instance.Slot : null, true);
            var color = TextColor.Value;
            if (MathX.Approximately(color.a, 0)) return;
            __instance.LocalUser.GetPointInFrontOfUser(out var pos, out _, null, new float3(0, 0, -0.1f));
            NotificationMessage.SpawnTextMessage(__instance.World, pos, syncElement.Name, color);
        }
    }
}
