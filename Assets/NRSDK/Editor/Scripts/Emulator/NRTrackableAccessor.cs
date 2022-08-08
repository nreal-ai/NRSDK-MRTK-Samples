/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NREditor
{
    /// <summary> A nr trackable accessor. </summary>
    internal abstract class NRTrackableAccessor
    {
        /// <summary> Target for the. </summary>
        protected NRTrackableBehaviour m_Target;

        /// <summary> Applies the data properties. </summary>
        public abstract void ApplyDataProperties();

        /// <summary> Applies the data appearance. </summary>
        public abstract void ApplyDataAppearance();
    }


    /// <summary> A nr accessor factory. </summary>
    internal class NRAccessorFactory
    {
        /// <summary> Creates a new NRTrackableAccessor. </summary>
        /// <param name="target"> Target for the.</param>
        /// <returns> A NRTrackableAccessor. </returns>
        public static NRTrackableAccessor Create(NRTrackableBehaviour target)
        {
            if (target is NRTrackableImageBehaviour)
            {
                return new NRImageTargetAccessor((NRTrackableImageBehaviour)target);
            }
            NRDebugger.Error(target.GetType().ToString() + "is not derived from NRTrackableImageBehaviour");
            return null;
        }
    }

}
