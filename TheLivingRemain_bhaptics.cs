using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using Il2Cpp;
using System.Runtime;
using UnityEngine;
using Il2CppHurricaneVR.Framework.Core;

[assembly: MelonInfo(typeof(TheLivingRemain_bhaptics.TheLivingRemain_bhaptics), "TheLivingRemain_bhaptics", "2.0.0", "Florian Fahrenberger")]
[assembly: MelonGame("Five Finger Studios", "TheLivingRemain")]


namespace TheLivingRemain_bhaptics
{
    public class TheLivingRemain_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;
        public static bool isRightHanded = true;
        public static Stopwatch timerBackpack = new Stopwatch();
        public static bool rightFootStep = true;



        public override void OnInitializeMelon()
        {
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

        [HarmonyPatch(typeof(PlayerController), "PlayerOutOfBreath", new Type[] { })]
        public class bhaptics_OutOfBreath
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance)
            {
                if (!tactsuitVr.IsPlaying("OutOfBreath")) tactsuitVr.PlaybackHaptics("OutOfBreath");
            }
        }

        [HarmonyPatch(typeof(PlayerController), "playHurtByToxicGasAudioClip", new Type[] { })]
        public class bhaptics_ToxicGas
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance)
            {
                if (!tactsuitVr.IsPlaying("GasDeath")) tactsuitVr.PlaybackHaptics("GasDeath");
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
        
        [HarmonyPatch(typeof(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSPistol), "FireBullet", new Type[] { typeof(Vector3) })]
        public class bhaptics_FirePistol
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSPistol __instance)
            {
                if (__instance.OutOfAmmo) return;
                bool isRight = true;
                bool twoHanded = false;
                if (__instance.holdingHand == Hand.left) { isRight = false; }
                if (__instance.StabilizerGrabbable.IsBeingHeld) twoHanded = true;
                string pattern = "Pistol";
                tactsuitVr.Recoil(pattern, isRight, twoHanded);
            }
        }

        [HarmonyPatch(typeof(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSHandgunRevolver), "FireBullet", new Type[] { typeof(Vector3) })]
        public class bhaptics_FireRevolver
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSHandgunRevolver __instance)
            {
                if (__instance.OutOfAmmo) return;
                bool isRight = true;
                bool twoHanded = false;
                if (__instance.holdingHand == Hand.left) { isRight = false; }
                if (__instance.StabilizerGrabbable.IsBeingHeld) twoHanded = true;
                string pattern = "Pistol";
                tactsuitVr.Recoil(pattern, isRight, twoHanded);
            }
        }

        [HarmonyPatch(typeof(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSShotgun), "FireBullet", new Type[] { typeof(Vector3) })]
        public class bhaptics_FireShotgun
        {
            [HarmonyPostfix]
            public static void Postfix(Il2CppHurricaneVR.Framework.Weapons.Guns.FFSShotgun __instance)
            {
                if (__instance.OutOfAmmo) return;
                bool isRight = true;
                bool twoHanded = false;
                if (__instance.hvrGrabbable.IsLeftHandGrabbed) { isRight = false; }
                if (__instance.StabilizerGrabbable.IsBeingHeld) twoHanded = true;
                string pattern = "Shotgun";
                tactsuitVr.Recoil(pattern, isRight, twoHanded);
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
                bool isRight = (__instance.hand == Hand.right);
                tactsuitVr.Recoil("Knife", isRight);
            }
        }

        
        [HarmonyPatch(typeof(Melee), "PlayHaptics", new Type[] {  })]
        public class bhaptics_KnifePlayHaptics
        {
            [HarmonyPostfix]
            public static void Postfix(Melee __instance)
            {
                if (__instance.canStab && __instance.currentVelocity.magnitude > __instance.stabVelocity && __instance.fowardMovementSpeed < -0.04f)
                {
                    if (__instance.isStabbing) tactsuitVr.LOG("Stabbing");
                    tactsuitVr.LOG("Update");
                    bool isRight = (__instance.hand == Hand.right);
                    tactsuitVr.Recoil("Knife", isRight);
                }
            }
        }
        
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

        private static (float, float) getAngleAndShift(PlayerController player, Vector3 hitDirection)
        {
            // bhaptics pattern starts in the front, then rotates to the left. 0° is front, 90° is left, 270° is right.
            // y is "up", z is "forward" in local coordinates
            Vector3 patternOrigin = new Vector3(0f, 0f, 1f);
            //Vector3 hitPosition = hit.hitPoint - player.position;
            Quaternion PlayerRotation = player.transform.rotation;
            Vector3 playerDir = PlayerRotation.eulerAngles;
            // get rid of the up/down component to analyze xz-rotation
            Vector3 flattenedHit = new Vector3(hitDirection.x, 0f, hitDirection.z);
            // get angle. .Net < 4.0 does not have a "SignedAngle" function...
            float hitAngle = Vector3.Angle(flattenedHit, patternOrigin);
            // check if cross product points up or down, to make signed angle myself
            Vector3 crossProduct = Vector3.Cross(flattenedHit, patternOrigin);
            if (crossProduct.y > 0f) { hitAngle *= -1f; }
            // relative to player direction
            float myRotation = hitAngle - playerDir.y;
            // switch directions (bhaptics angles are in mathematically negative direction)
            myRotation *= -1f;
            // convert signed angle into [0, 360] rotation
            if (myRotation < 0f) { myRotation = 360f + myRotation; }

            // up/down shift is in y-direction
            // in Vertigo 2, the torso Transform has y=0 at the neck,
            // and the torso ends at roughly -0.5 (that's in meters)
            // so cap the shift to [-0.5, 0]...
            float hitShift = hitDirection.y;
            float upperBound = 0.0f;
            float lowerBound = -0.5f;
            if (hitShift > upperBound) { hitShift = 0.5f; }
            else if (hitShift < lowerBound) { hitShift = -0.5f; }
            // ...and then spread/shift it to [-0.5, 0.5], which is how bhaptics expects it
            else { hitShift = (hitShift - lowerBound) / (upperBound - lowerBound) - 0.5f; }


            return (myRotation, hitShift);
        }


        [HarmonyPatch(typeof(PlayerController), "ApplyDamage", new Type[] { typeof(Damage) })]
        public class bhaptics_ApplyDamage
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerController __instance, Damage damage)
            {
                string pattern = "Slash";
                if ((damage.killType == KillType.toxicGas) && (__instance.isGasMaskOn)) return;
                if (damage.killType == KillType.bullet) pattern = "BulletHit";
                if (damage.killType == KillType.grenade) pattern = "ExplosionFace";
                if (damage.killType == KillType.stationaryExplosion) pattern = "ExplosionBelly";
                if (damage.killType == KillType.enemy) pattern = "Slash";
                if (damage.killType == KillType.melee) pattern = "Impact";
                if (damage.killType == KillType.fall) pattern = "FallDamage";
                if (damage.killType == KillType.other) return;
                if (damage.killType == KillType.fire) pattern = "FlameThrower";
                if (damage.killType == KillType.toxicGas)
                {
                    if (!tactsuitVr.IsPlaying("GasDeath")) tactsuitVr.PlaybackHaptics("GasDeath");
                    return;
                }
                if (damage.killType == KillType.electricity)
                {
                    if (!tactsuitVr.IsPlaying("Electrocution")) tactsuitVr.PlaybackHaptics("Electrocution");
                    return;
                }
                if (damage.killType == KillType.mutantRat) pattern = "Impact";
                tactsuitVr.LOG("ApplyDamage: " + damage.killType.ToString() + " " + damage.damageLocation.ToString() + " " + damage.positionToPlace.x.ToString() + damage.position.x.ToString() + " " + damage.position.y.ToString() + " " + damage.position.z.ToString() + " ");
                tactsuitVr.LOG("Player: " + __instance.transform.position.x.ToString() + " " + __instance.transform.position.y.ToString() + " " + __instance.transform.position.z.ToString());
                tactsuitVr.PlaybackHaptics(pattern);
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

        [HarmonyPatch(typeof(AmmoBelt), "AddAmmoToInventory", new Type[] { typeof(HVRGrabbable), typeof(GameObject) })]
        public class bhaptics_PutInBackPack
        {
            [HarmonyPostfix]
            public static void Postfix(AmmoBelt __instance, GameObject obj)
            {
                if (obj == null) return;
                if (timerBackpack.ElapsedMilliseconds <= 300) return;
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackStore_L");
                else tactsuitVr.PlaybackHaptics("BackpackStore_R");
            }
        }

        [HarmonyPatch(typeof(PlayerInventoryFilament), "AddToInventory", new Type[] { typeof(GameObject) })]
        public class bhaptics_AddFilament
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerInventoryFilament __instance, GameObject obj)
            {
                if (obj == null) return;
                if (isRightHanded) tactsuitVr.PlaybackHaptics("BackpackStore_L");
                else tactsuitVr.PlaybackHaptics("BackpackStore_R");
            }
        }

        [HarmonyPatch(typeof(AmmoBeltMagazines), "AttachToHand", new Type[] { typeof(GameObject), typeof(Il2CppHurricaneVR.Framework.Core.Grabbers.HVRHandGrabber) })]
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

        /*
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
        */

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
