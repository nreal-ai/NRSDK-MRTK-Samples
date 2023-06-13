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
    using System.Text;

    /// <summary>
    /// Structure holding data summarizing the result of an Android permissions request. </summary>
    public struct AndroidPermissionsRequestResult
    {
        /// <summary> Constructs a new AndroidPermissionsRequestResult. </summary>
        /// <param name="permissionNames"> The value for PermissionNames.</param>
        /// <param name="grantResults">    The value for GrantResults.</param>
        public AndroidPermissionsRequestResult(
            string[] permissionNames, bool[] grantResults) : this()
        {
            PermissionNames = permissionNames;
            GrantResults = grantResults;
        }

        /// <summary> Gets or sets a collection of permissions requested. </summary>
        /// <value> A list of names of the permissions. </value>
        public string[] PermissionNames { get; private set; }

        /// <summary>
        /// Gets or sets a collection of results corresponding to <see cref="PermissionNames"/>. </summary>
        /// <value> The grant results. </value>
        public bool[] GrantResults { get; private set; }

        /// <summary> Gets a value indicating whether all permissions are granted. </summary>
        /// <value> True if all is granted, false if not. </value>
        public bool IsAllGranted
        {
            get
            {
                if (PermissionNames == null || GrantResults == null)
                {
                    return false;
                }

                for (int i = 0; i < GrantResults.Length; i++)
                {
                    if (!GrantResults[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < PermissionNames.Length; i++)
            {
                st.AppendLine("Name:" + PermissionNames[i] + " GrantResult:" + GrantResults[i]);
            }
            return st.ToString();
        }
    }
}