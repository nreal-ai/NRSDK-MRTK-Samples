/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Collections.Generic;


    /// <summary> A controller provider factory. </summary>
    internal static class ControllerProviderFactory
    {
        private static Dictionary<Type, ControllerProviderBase> m_ControllerProviderDict = new Dictionary<Type, ControllerProviderBase>();

        /// <summary> Type of the android controller provider. </summary>
        public static Type controllerProviderType
        {
            get
            {
#if UNITY_EDITOR
                return typeof(EditorControllerProvider);
#else
               return typeof(NRControllerProvider);
#endif
            }
        }

        /// <summary> Creates controller provider. </summary>
        /// <param name="states"> The states.</param>
        /// <returns> The new controller provider. </returns>
        public static ControllerProviderBase CreateControllerProvider(ControllerState[] states)
        {
            ControllerProviderBase provider = GetOrCreateControllerProvider(controllerProviderType, states);
            return provider;
        }

        /// <summary> Creates controller provider. </summary>
        /// <param name="providerType"> Type of the provider.</param>
        /// <param name="states">       The states.</param>
        /// <returns> The new controller provider. </returns>
        internal static ControllerProviderBase GetOrCreateControllerProvider(Type providerType, ControllerState[] states)
        {
            if (providerType != null)
            {
                if (m_ControllerProviderDict.ContainsKey(providerType))
                {
                    return m_ControllerProviderDict[providerType];
                }
                object parserObj = Activator.CreateInstance(providerType, new object[] { states });
                if (parserObj is ControllerProviderBase)
                {
                    var controllerProvider = parserObj as ControllerProviderBase;
                    m_ControllerProviderDict.Add(providerType, controllerProvider);
                    return controllerProvider;
                }
            }
            return null;
        }
    }

}