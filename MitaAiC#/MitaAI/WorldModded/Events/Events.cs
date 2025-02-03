﻿using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MitaAI
{
    public static class EventsModded
    {
        public static void HandleAnimationEvent(UnityEngine.AnimationEvent evt)
        {
            MelonLogger.Msg($"AnimationEvent triggered! Time: {evt.time}, String: {evt.stringParameter}, Int: {evt.intParameter}, Float: {evt.floatParameter}");
        }

        public static void HandleCustomEvent(string eventName)
        {
            MelonLogger.Msg($"HandleCustomEvent called with event: {eventName}");

            switch (eventName)
            {
                case "ConsoleEnd":
                    PlayerAnimationModded.stopAnim();
                    break;
                case "SofaSit":
                    MelonLogger.Msg($"SofaSit");
                    PlayerAnimationModded.playAnimObject(MitaCore.TryfindChild(MitaCore.worldHouse, "House/HouseGameNormal Tamagotchi/HouseGame Tamagotchi/House/Main/SofaChair") );
                    PlayerAnimationModded.currentPlayerMovement = PlayerAnimationModded.PlayerMovement.sit;
                    break;
                default:
                    break;
            }
        }


    }




    [HarmonyLib.HarmonyPatch]
    public static class Console
    {

    }

    [HarmonyLib.HarmonyPatch(typeof(Mob_Maneken), "StartKillPlayer")]
    public static class Maneken
    {
        private static void Postfix()
        {
            MelonLogger.Msg("TRYING TRYING");
            if (MitaCore.Instance != null)
            {
                MelonLogger.Msg("MitaCore.Instance is NOT  null.))");
                MitaCore.Instance.playerKilled(); // Вызов метода playerKilled из экземпляра MitaCore
            }
            else
            {
                MelonLogger.Msg("MitaCore.Instance is null.");
            }
        }
    }
    
    [HarmonyLib.HarmonyPatch]
    public static class Safe
    {
        [HarmonyLib.HarmonyPatch(typeof(Basement_Safe), "ClickButton", new Type[] { typeof(int) })]
        [HarmonyLib.HarmonyPostfix]
        private static void Postfix()
        {

            MitaCore.Instance?.playerClickSafe();

        }

        [HarmonyLib.HarmonyPatch(typeof(Basement_Safe), "RightPassword")]
        [HarmonyLib.HarmonyPostfix]
        private static void Postfix2()
        { 
            
        MitaCore.Instance?.sendSystemMessage("Игрок открыл сейф"); 
   
        }
    }
 

}
