﻿using UnityEngine;
using System.Collections;
using Kilt.Extensions;
using Kilt.Reflection;

namespace Kilt
{
    public abstract class BaseSingleton : MonoBehaviour
    {
        private static bool s_enableSingletonCreation = false;
        public static bool EnableSingletonCreation
        {
            get
            {
                if (Application.isEditor && !Application.isPlaying)
                    s_enableSingletonCreation = true;
                return s_enableSingletonCreation;
            }
            private set
            {
                if (s_enableSingletonCreation == value)
                    return;
                s_enableSingletonCreation = value;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EnableInstanceCreation()
        {
            EnableSingletonCreation = true;
        }
    }
}
