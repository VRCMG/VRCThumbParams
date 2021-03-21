﻿using System;
using System.Collections;
using MelonLoader;
using ThumbParams;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

[assembly: MelonInfo(typeof(MainMod), "ThumbParams", "1.1.4", "benaclejames")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace ThumbParams
{
    public class MainMod : MelonMod
    {
        private int rightThumb, leftThumb;

        public override void VRChat_OnUiManagerInit()
        {
            MelonCoroutines.Start(UpdateParamStores());
            MelonLogger.Msg(ConsoleColor.Cyan, "Initialized Sucessfully!");
        }
        

        IEnumerator UpdateParamStores()
        {
            for (;;)
            {
                yield return new WaitForSeconds(2);
                rightThumb = GetParamIndex("RightThumb");
                leftThumb = GetParamIndex("LeftThumb");
            }
        }
        
        private static int GetParamIndex(string paramName)
        {
            VRCExpressionParameters.Parameter[] parameters = new VRCExpressionParameters.Parameter[0];
            
            parameters = VRCPlayer
                    .field_Internal_Static_VRCPlayer_0
                    ?.field_Internal_GameObject_0?.GetComponent<VRCAvatarDescriptor>()?.expressionParameters
                    ?.parameters;


            if (parameters == null) return -1;
            
            var index = -1;
            for (var i = 0; i < parameters.Length; i++)
            {
                VRCExpressionParameters.Parameter param = parameters[i];
                if (param.name == null)
                    return -1;
                if (param.name == paramName)
                {
                    index = i;
                }
            }

            return index;
        }
        
        internal static SteamVR_ControllerManager GetControllerManager()
        {
            foreach (var vrcTracking in VRCTrackingManager.field_Private_Static_VRCTrackingManager_0
                .field_Private_List_1_VRCTracking_0)
            {
                var vrcTrackingSteam = vrcTracking.TryCast<VRCTrackingSteam>();
                if (vrcTrackingSteam == null) continue;

                return vrcTrackingSteam.field_Private_SteamVR_ControllerManager_0;
            }

            throw new ApplicationException("SteamVR tracking not found");
        }
        
        public override void OnUpdate()
        {
            if (VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0[
                    "ThumbSpreadLeft"] == null ||
                VRCInputManager.field_Private_Static_Dictionary_2_String_VRCInput_0[
                    "ThumbSpreadRight"] == null)
                return;

            var leftThumb = ConvertToThumbState(VRCInputManager
                .field_Private_Static_Dictionary_2_String_VRCInput_0[
                    "ThumbSpreadLeft"].field_Public_Single_0);
            
            var rightThumb = ConvertToThumbState(VRCInputManager
                .field_Private_Static_Dictionary_2_String_VRCInput_0[
                    "ThumbSpreadRight"].field_Public_Single_0);
            
            var controller = VRCPlayer.field_Internal_Static_VRCPlayer_0
                    ?.field_Private_VRC_AnimationController_0?.field_Private_AvatarAnimParamController_0;
            
            
            SetParameter(controller, this.leftThumb, (int) leftThumb);
            SetParameter(controller, this.rightThumb, (int) rightThumb);
        }

        private void SetParameter(AvatarAnimParamController controller,
            int paramIndex, int state)
        {
            
            if (controller == null || controller.field_Private_AvatarPlayableController_0 == null ||
                paramIndex == -1)
                return;
            
            controller.field_Private_AvatarPlayableController_0.Method_Public_Boolean_Int32_Single_1(paramIndex, state);
        }

        enum ThumbState
        {
            THUMBUP,
            ABUTTON,
            BBUTTON,
            TRACKPAD,
            THUMBSTICK
        }

        private ThumbState ConvertToThumbState(float floatThumbState)
        {
            switch (floatThumbState)
            {
                case 0.66f:
                    return ThumbState.THUMBUP;
                case 0.7f:
                    return ThumbState.THUMBUP;
                case 0.4f:
                    return ThumbState.ABUTTON;
                case 0.35f:
                    return ThumbState.BBUTTON;
                case 0.6f:
                    return ThumbState.TRACKPAD;
                case 0.9f:
                    return ThumbState.THUMBSTICK; 
            }

            return ThumbState.THUMBUP;
        }
    }
}