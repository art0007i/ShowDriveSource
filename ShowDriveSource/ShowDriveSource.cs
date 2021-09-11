using HarmonyLib;
using NeosModLoader;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using FrooxEngine;
using FrooxEngine.LogiX;
using BaseX;
using FrooxEngine.UIX;

namespace ShowDriveSource
{
    public class ShowDriveSource : NeosMod
    {
        public override string Name => "ShowDriveSource";
        public override string Author => "art0007i";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/art0007i/NeosTemplate/";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.art0007i.ShowDriveSource");
            harmony.PatchAll();

        }
        [HarmonyPatch(typeof(SyncMemberEditorBuilder))]
        [HarmonyPatch("GenerateMemberField")]
        class SyncMemberEditorBuilder_GenerateMemberField_Patch
        {
            private static void ShowDrive(IButton button, ButtonEventData data)
            {
                var refProxy = button.Slot.GetComponent<ReferenceProxySource>();
                if (refProxy == null) return;
                var field = refProxy.Reference.Target as IField;
                if (field == null || (!field.IsDriven && !field.IsLinked)) return;
                SyncElement syncElement = field.ActiveLink as SyncElement;
                InspectorHelper.OpenInspectorForTarget(syncElement.Component, null, true);
                float3 pos = float3.Zero;
                floatQ rot = floatQ.Identity;
                (button as Component).LocalUser.GetPointInFrontOfUser(out pos, out rot, null, new float3(0, 0, -0.1f));
                NotificationMessage.SpawnTextMessage((button as Component).World, pos, syncElement.Name, color.Magenta);
            }
            public static void Postfix(UIBuilder ui, string name)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    ui.Root.GetComponentInChildren<ReferenceProxySource>().Slot.GetComponent<Button>().LocalPressed += ShowDrive;
                }
            }
        }
    }
}