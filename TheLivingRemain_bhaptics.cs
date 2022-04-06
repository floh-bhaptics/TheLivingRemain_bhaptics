using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;


namespace TheLivingRemain_bhaptics
{
    public class TheLivingRemain_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool isRightHanded = true;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }
        
        [HarmonyPatch(typeof(Handgun), "FireGun", new Type[] { })]
        public class bhaptics_FireHandGun
        {
            [HarmonyPostfix]
            public static void Postfix(Handgun __instance)
            {
                bool isRight = (__instance.holdingHand == VRTK.GrabAttachMechanics.Hand.right);
                tactsuitVr.Recoil("Pistol", isRight);
            }
        }

        [HarmonyPatch(typeof(GenerateExplosion), "Explode", new Type[] { })]
        public class bhaptics_GenerateExplosion
        {
            [HarmonyPostfix]
            public static void Postfix(GenerateExplosion __instance)
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
            }
        }

        [HarmonyPatch(typeof(PlayerController), "ApplyDamage", new Type[] { typeof(Damage) })]
        public class bhaptics_ApplyDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance, Damage damage)
            {
                //tactsuitVr.LOG("Damage: " + damage.damageLocation.ToString());
                //tactsuitVr.LOG("Parameters: " + damage.power.ToString() + " " + damage.position.x.ToString() + " " + __instance.transform.position.x.ToString() + " " + damage.bodyPartHit.ToString());
            }
        }

        [HarmonyPatch(typeof(PlayerController), "setHealth", new Type[] { typeof(float) })]
        public class bhaptics_SetHealth
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance, float health)
            {
                //tactsuitVr.LOG("SetHealth: " + health.ToString() + " " + __instance.regenerateHealthMax.ToString());
            }
        }

        [HarmonyPatch(typeof(MedBottle), "GiveHealth", new Type[] {  })]
        public class bhaptics_MedBottleHealth
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Healing");
            }
        }

        [HarmonyPatch(typeof(AmmoBelt), "AddAmmoToInventory", new Type[] { typeof(UnityEngine.GameObject) })]
        public class bhaptics_PutInBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBelt __instance)
            {
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackStore_L");
                else tactsuitVr.PlaybackHaptics("BackpackStore_R");
            }
        }

        [HarmonyPatch(typeof(HandedInputSelector), "SetActiveController", new Type[] { typeof(OVRInput.Controller) })]
        public class bhaptics_Handedness
        {
            [HarmonyPostfix]
            public static void Postfix(OVRInput.Controller c)
            {
                isRightHanded = (c == OVRInput.Controller.RHand);
            }
        }

        [HarmonyPatch(typeof(AmmoBeltMagazines), "AttempPutMagazineInHand", new Type[] { typeof(VRTK.VRTK_InteractGrab), typeof(VRTK.VRTK_InteractTouch), typeof(VRTK.VRTK_InteractGrab)})]
        public class bhaptics_GetAmmoFromBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBeltMagazines __instance)
            {
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackRetrieve_L");
                else tactsuitVr.PlaybackHaptics("BackpackRetrieve_R");
            }
        }

        [HarmonyPatch(typeof(AmmoBeltMagazines), "AttempPutFilamentInHand", new Type[] { typeof(VRTK.VRTK_InteractGrab), typeof(VRTK.VRTK_InteractTouch), typeof(VRTK.VRTK_InteractGrab) })]
        public class bhaptics_GetFilamentFromBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBeltMagazines __instance)
            {
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackRetrieve_L");
                else tactsuitVr.PlaybackHaptics("BackpackRetrieve_R");
            }
        }
    }
}
