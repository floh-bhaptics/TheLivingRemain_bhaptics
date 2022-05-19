using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;


namespace TheLivingRemain_bhaptics
{
    public class TheLivingRemain_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool isRightHanded = true;
        public static Stopwatch timerBackpack = new Stopwatch();
        public static bool rightFootStep = true;


        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            timerBackpack.Start();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }
        
        #region Health

        [HarmonyPatch(typeof(PlayerController), "UpdatePlayerVisionAndAudio", new Type[] { })]
        public class bhaptics_UpdateVisionAndAudio
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance)
            {
                //tactsuitVr.LOG("ApplyDamageFloat");
                if (__instance.hurtStatus == HurtStatus.hurt) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
            }
        }

        [HarmonyPatch(typeof(MedBottle), "Update", new Type[] { })]
        public class bhaptics_MedBottleHealth
        {
            [HarmonyPostfix]
            public static void Postfix(MedBottle __instance)
            {
                if (__instance.used) return;
                if (!__instance.CanGiveHeath()) return;
                if (!tactsuitVr.IsPlaying("Healing")) tactsuitVr.PlaybackHaptics("Healing");
            }
        }

        [HarmonyPatch(typeof(PlayerController), "playerDead", new Type[] { })]
        public class bhaptics_PlayerDead
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }

        #endregion

        #region Recoil

        [HarmonyPatch(typeof(Handgun), "FireGun", new Type[] { })]
        public class bhaptics_FireHandGun
        {
            [HarmonyPostfix]
            public static void Postfix(Handgun __instance)
            {
                if (__instance.canFire) return;
                bool isRight = (__instance.holdingHand == VRTK.GrabAttachMechanics.Hand.right);
                tactsuitVr.Recoil("Pistol", isRight);
            }
        }

        [HarmonyPatch(typeof(HandgunRevolver), "FireProjectile", new Type[] { })]
        public class bhaptics_FireRevolverGun
        {
            [HarmonyPostfix]
            public static void Postfix(HandgunRevolver __instance)
            {
                bool isRight = (__instance.holdingHand == HandgunRevolver.HoldingHand.right);
                tactsuitVr.Recoil("Pistol", isRight);
            }
        }

        [HarmonyPatch(typeof(MachineGunBack), "FireGun", new Type[] { })]
        public class bhaptics_FireShotgun
        {
            [HarmonyPostfix]
            public static void Postfix(MachineGunBack __instance)
            {
                if (__instance.canFire) return;
                bool isRight = (__instance.holdingHand == VRTK.GrabAttachMechanics.Hand.right);
                tactsuitVr.Recoil("Shotgun", isRight);
            }
        }

        [HarmonyPatch(typeof(MachineGunFront), "GunFired", new Type[] { })]
        public class bhaptics_FireShotgunFront
        {
            [HarmonyPostfix]
            public static void Postfix(MachineGunFront __instance)
            {
                if (__instance.controllerReference == null) return;
                if (!__instance.grabbed) return;
                bool isRight = (__instance.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Right);
                tactsuitVr.Recoil("Shotgun", isRight, true);
            }
        }
        [HarmonyPatch(typeof(Minigun), "FireProjectile", new Type[] { })]
        public class bhaptics_FireMinigun
        {
            [HarmonyPostfix]
            public static void Postfix(Minigun __instance)
            {
                if (__instance.isHeldByLeftController) tactsuitVr.Recoil("Pistol", false);
                if (__instance.isHeldByRightController) tactsuitVr.Recoil("Pistol", true);
            }
        }

        [HarmonyPatch(typeof(Melee), "PlayHapticsWithDelay", new Type[] { typeof(float) })]
        public class bhaptics_KnifeHapticsDelay
        {
            [HarmonyPostfix]
            public static void Postfix(Melee __instance)
            {
                tactsuitVr.LOG("DelayHaptics");
                bool isRight = (__instance.hand == VRTK.GrabAttachMechanics.Hand.right);
                tactsuitVr.Recoil("Knife", isRight);
            }
        }

        /*
        [HarmonyPatch(typeof(Melee), "Update", new Type[] {  })]
        public class bhaptics_KnifeShowImpact
        {
            [HarmonyPostfix]
            public static void Postfix(Melee __instance)
            {
                if (__instance.canStab && __instance.currentVelocity.magnitude > __instance.stabVelocity && __instance.fowardMovementSpeed < -0.04f)
                {
                    if (__instance.isStabbing) tactsuitVr.LOG("Stabbing");
                    tactsuitVr.LOG("Update");
                    bool isRight = (__instance.hand == VRTK.GrabAttachMechanics.Hand.right);
                    tactsuitVr.Recoil("Knife", isRight);
                }
            }
        }
        */
        #endregion

        #region Explosions

        [HarmonyPatch(typeof(GenerateExplosion), "Explode", new Type[] { })]
        public class bhaptics_GenerateExplosion
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
                tactsuitVr.PlaybackHaptics("ExplosionFeet");
            }
        }

        [HarmonyPatch(typeof(DestructionAnimated), "StartGrenadeExplosion", new Type[] { })]
        public class bhaptics_GrenadeExplosion
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
                tactsuitVr.PlaybackHaptics("ExplosionFeet");
            }
        }

        [HarmonyPatch(typeof(Destruction_FFS), "Explode", new Type[] { })]
        public class bhaptics_FFSExplosion
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
                tactsuitVr.PlaybackHaptics("ExplosionFeet");
            }
        }

        [HarmonyPatch(typeof(FragGrenade), "showExplosionEffects", new Type[] { })]
        public class bhaptics_FragGrenadeExplosion
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
                tactsuitVr.PlaybackHaptics("ExplosionFeet");
            }
        }

        [HarmonyPatch(typeof(MetalBarrel), "OnExplosion", new Type[] { })]
        public class bhaptics_BarrelExplosion
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
                tactsuitVr.PlaybackHaptics("ExplosionFeet");
            }
        }

        #endregion

        #region Damage

        [HarmonyPatch(typeof(PlayerController), "ApplyDamage", new Type[] { typeof(Damage) })]
        public class bhaptics_ApplyDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance, Damage damage)
            {
                if ((damage.killType == KillType.toxicGas) && (__instance.isGasMaskOn)) return;
                if (damage.killType == KillType.bullet) tactsuitVr.PlaybackHaptics("BulletHit");
                if (damage.killType == KillType.grenade) tactsuitVr.PlaybackHaptics("ExplosionFace");
                if (damage.killType == KillType.stationaryExplosion) tactsuitVr.PlaybackHaptics("ExplosionBelly");
                if (damage.killType == KillType.enemy) tactsuitVr.PlaybackHaptics("Slash");
                if (damage.killType == KillType.melee) tactsuitVr.PlaybackHaptics("Impact");
                if (damage.killType == KillType.fall) tactsuitVr.PlaybackHaptics("FallDamage");
                if (damage.killType == KillType.other) return;
                if (damage.killType == KillType.fire) tactsuitVr.PlaybackHaptics("FlameThrower");
                if (damage.killType == KillType.toxicGas)
                {
                    if (!tactsuitVr.IsPlaying("GasDeath")) tactsuitVr.PlaybackHaptics("GasDeath");
                }
                if (damage.killType == KillType.electricity)
                {
                    if (!tactsuitVr.IsPlaying("Electrocution")) tactsuitVr.PlaybackHaptics("Electrocution");
                }
                if (damage.killType == KillType.mutantRat) tactsuitVr.PlaybackHaptics("Impact");
                //tactsuitVr.LOG("ApplyDamage: " + damage.killType.ToString() + " " + damage.damageLocation.ToString() + " " + damage.position.x.ToString() + " " + damage.positionToPlace.x.ToString());
                //tactsuitVr.PlaybackHaptics("Impact");
            }
        }

        [HarmonyPatch(typeof(PlayerController), "showBloodFromHit", new Type[] {  })]
        public class bhaptics_ShowBlood
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance)
            {
                //tactsuitVr.LOG("ApplyDamageFloat");
                tactsuitVr.PlaybackHaptics("Slash");
            }
        }

        #endregion

        #region Backpack

        [HarmonyPatch(typeof(HandedInputSelector), "SetActiveController", new Type[] { typeof(OVRInput.Controller) })]
        public class bhaptics_Handedness
        {
            [HarmonyPostfix]
            public static void Postfix(OVRInput.Controller c)
            {
                isRightHanded = (c == OVRInput.Controller.RHand);
            }
        }

        [HarmonyPatch(typeof(AmmoBelt), "AddAmmoToInventory", new Type[] { typeof(UnityEngine.GameObject) })]
        public class bhaptics_PutInBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBelt __instance, UnityEngine.GameObject obj)
            {
                if (obj == null) return;
                if (timerBackpack.ElapsedMilliseconds <= 300) return;
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackStore_L");
                else tactsuitVr.PlaybackHaptics("BackpackStore_R");
            }
        }

        [HarmonyPatch(typeof(PlayerInventoryFilament), "AddToInventory", new Type[] { typeof(UnityEngine.GameObject) })]
        public class bhaptics_AddFilament
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerInventoryFilament __instance, UnityEngine.GameObject obj)
            {
                if (obj == null) return;
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackStore_L");
                else tactsuitVr.PlaybackHaptics("BackpackStore_R");
            }
        }

        [HarmonyPatch(typeof(AmmoBeltMagazines), "AttachToHand", new Type[] { typeof(UnityEngine.GameObject), typeof(VRTK.VRTK_InteractGrab), typeof(VRTK.VRTK_InteractTouch) })]
        public class bhaptics_GetStuffFromBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBeltMagazines __instance)
            {
                timerBackpack.Restart();
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackRetrieve_L");
                else tactsuitVr.PlaybackHaptics("BackpackRetrieve_R");
            }
        }

        [HarmonyPatch(typeof(MachineGunBack), "PutInBackpackHaptics", new Type[] { })]
        public class bhaptics_StoreShotgun
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (isRightHanded) tactsuitVr.PlaybackHaptics("StoreShotgun_R");
                else tactsuitVr.PlaybackHaptics("StoreShotgun_L");
            }
        }

        [HarmonyPatch(typeof(MachineGunBack), "AttachToHand", new Type[] { typeof(UnityEngine.GameObject), typeof(UnityEngine.Vector3), typeof(SnapDropZoneObject) })]
        public class bhaptics_ReceiveShotgun
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (isRightHanded) tactsuitVr.PlaybackHaptics("ReceiveShotgun_R");
                else tactsuitVr.PlaybackHaptics("ReceiveShotgun_L");
            }
        }

        #endregion

        #region Foot interaction

        [HarmonyPatch(typeof(FootStepSound), "PlayAudioForSurface", new Type[] { typeof(GROUND_SURFACE) })]
        public class bhaptics_Footsteps
        {
            [HarmonyPostfix]
            public static void Postfix(GROUND_SURFACE groundSurface)
            {
                if (rightFootStep) tactsuitVr.PlaybackHaptics("FootStep_R");
                else tactsuitVr.PlaybackHaptics("FootStep_L");
                rightFootStep = !rightFootStep;
            }
        }

        [HarmonyPatch(typeof(FootStepSound), "PlayLandingAudio", new Type[] {  })]
        public class bhaptics_FallLanding
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("FallDamage");
                tactsuitVr.PlaybackHaptics("FallDamageFeet");
            }
        }

        #endregion

    }
}
