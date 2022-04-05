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
        
    }
}
