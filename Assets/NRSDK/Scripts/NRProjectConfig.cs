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
	using System.Collections.Generic;
    using UnityEngine;

	[System.Serializable]
	public class NRProjectConfig: ScriptableObject
    {
		public List<NRDeviceType> targetDeviceTypes = new List<NRDeviceType> {
				NRDeviceType.NrealLight,
				NRDeviceType.NrealAir,  
			};

        public string GetTargetDeviceTypesDesc()
        {
            string devices = string.Empty;
            foreach (var device in targetDeviceTypes)
            {
                if (devices != string.Empty)
                    devices += "|";
                devices += device;
            }
            return devices;
        }
	}
}
