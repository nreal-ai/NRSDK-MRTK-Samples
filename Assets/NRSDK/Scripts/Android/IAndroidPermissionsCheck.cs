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
    /// <summary>
    /// Interface for checking Android permission. The interface is used for MOCK unity test. </summary>
    public interface IAndroidPermissionsCheck
    {
        /// <summary> Requests an Android permission from the user. </summary>
        /// <param name="permissionName"> The permission to be requested (e.g. android.permission.CAMERA).</param>
        /// <returns>
        /// An asynchronous task that completes when the user has accepted or rejected the requested
        /// permission and yields a <see cref="AndroidPermissionsRequestResult"/> that summarizes the
        /// result. If this method is called when another permissions request is pending, <c>null</c>
        /// will be returned instead. </returns>
        AsyncTask<AndroidPermissionsRequestResult> RequestAndroidPermission(
            string permissionName);
    }
}
