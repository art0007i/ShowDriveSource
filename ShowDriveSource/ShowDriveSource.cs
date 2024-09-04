using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;
using Elements.Core;
using FrooxEngine.UIX;

namespace ShowDriveSource;

public class ShowDriveSource : ResoniteMod
{
    public override string Name => "ShowDriveSource";
    public override string Author => "art0007i";
    public override string Version => "2.0.2";
    public override string Link => "https://github.com/art0007i/ShowDriveSource/";

    [AutoRegisterConfigKey]
    public static ModConfigurationKey<bool> KEY_PUSHBACK = new("pushback", "Clicking on a field will make the new inspector push back the original one.", () => false);
    [AutoRegisterConfigKey]
    public static ModConfigurationKey<colorX> KEY_TEXT_COLOR = new("text_color", "Color of the text that pops up when you click a field. (Set alpha to 0 to disable the text)", () => colorX.Magenta);

    public static ModConfiguration config;

    public override void OnEngineInit()
    {
        config = GetConfiguration();
        Harmony harmony = new Harmony("me.art0007i.ShowDriveSource");
        harmony.PatchAll();
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
            InspectorHelper.OpenInspectorForTarget(syncElement.Component, config.GetValue(KEY_PUSHBACK) ? __instance.Slot : null, true);
            var color = config.GetValue(KEY_TEXT_COLOR);
            if (MathX.Approximately(color.a, 0)) return;
            __instance.LocalUser.GetPointInFrontOfUser(out var pos, out _, null, new float3(0, 0, -0.1f));
            NotificationMessage.SpawnTextMessage(__instance.World, pos, syncElement.Name, color);
        }
    }
}