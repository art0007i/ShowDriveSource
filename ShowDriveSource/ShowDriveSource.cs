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
        public override string Version => "1.1.0";
        public override string Link => "https://github.com/art0007i/NeosTemplate/";
        public override void OnEngineInit()
        {
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
                InspectorHelper.OpenInspectorForTarget(syncElement.Component, null, true);
                float3 pos = float3.Zero;
                floatQ rot = floatQ.Identity;
                __instance.LocalUser.GetPointInFrontOfUser(out pos, out rot, null, new float3(0, 0, -0.1f));
                NotificationMessage.SpawnTextMessage(__instance.World, pos, syncElement.Name, color.Magenta);
            }
        }
    }
}