﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MelonLoader;
using Bhaptics.SDK2;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        /* A class that contains the basic functions for the bhaptics Tactsuit, like:
         * - A Heartbeat function that can be turned on/off
         * - A function to read in and register all .tact patterns in the bHaptics subfolder
         * - A logging hook to output to the Melonloader log
         * - 
         * */
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        // Event to start and stop the heartbeat thread
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        // dictionary of all feedback patterns found in the bHaptics directory
        public Dictionary<String, FileInfo> FeedbackMap = new Dictionary<String, FileInfo>();

        public void HeartBeatFunc()
        {
            while (true)
            {
                // Check if reset event is active
                HeartBeat_mrse.WaitOne();
                PlaybackHaptics("HeartBeat");
                Thread.Sleep(600);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            var res = BhapticsSDK2.Initialize("aJ3w5rjMyXIuXT1ZjXH1", "5LT1oXLunpY8CXsyDJe6", "");

            if (res > 0)
            {
                LOG("Failed to do bhaptics initialization...");
            }
            LOG("Starting HeartBeat thread...");
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            PlaybackHaptics("HeartBeat");
        }

        public void LOG(string logStr)
        {
#pragma warning disable CS0618 // remove warning that the logger is deprecated
            MelonLogger.Msg(logStr);
#pragma warning restore CS0618
        }



        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f, float xzAngle = 0f, float yShift = 0f)
        {
            int res;
            res = BhapticsSDK2.Play(key.ToLower(), intensity, duration, xzAngle, yShift);
            // LOG("Playing back: " + key);
            //if (res > 0) LOG("Playback failed: " + key + " " + key.ToLower());
        }


        public void Recoil(string weaponName, bool isRightHand, bool twoHanded = false, float intensity = 1.0f)
        {
            // weaponName is a parameter that will go into the vest feedback pattern name
            // isRightHand is just which side the feedback is on
            // intensity should usually be between 0 and 1

            // make postfix according to parameter
            string postfix = "_L";
            string otherPostfix = "_R";
            if (isRightHand) { postfix = "_R"; otherPostfix = "_L"; }

            // stitch together pattern names for Arm and Hand recoil
            string keyHands = "RecoilHands" + postfix;
            string keyArm = "RecoilArms" + postfix;
            string keyOtherHand = "RecoilHands" + otherPostfix;
            string keyOtherArm = "RecoilArms" + otherPostfix;
            // vest pattern name contains the weapon name. This way, you can quickly switch
            // between swords, pistols, shotguns, ... by just changing the shoulder feedback
            // and scaling via the intensity for arms and hands
            string keyVest = "Recoil" + weaponName + "Vest" + postfix;
            PlaybackHaptics(keyHands);
            PlaybackHaptics(keyArm);
            PlaybackHaptics(keyVest);
            if (twoHanded)
            {
                PlaybackHaptics(keyOtherHand);
                PlaybackHaptics(keyOtherArm);
            }
        }


        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
        }

        public bool IsPlaying(String effect)
        {
            return BhapticsSDK2.IsPlaying(effect.ToLower());
        }

        public void StopHapticFeedback(String effect)
        {
            BhapticsSDK2.Stop(effect.ToLower());
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            BhapticsSDK2.StopAll();
        }

        public void StopThreads()
        {
            // Yes, looks silly here, but if you have several threads like this, this is
            // very useful when the player dies or starts a new level
            StopHeartBeat();
        }


    }
}
