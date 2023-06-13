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
    /// <summary> A controller provider base. </summary>
    public abstract class ControllerProviderBase
    {
        /// <summary> The states. </summary>
        protected ControllerState[] states;
        /// <summary> Gets or sets a value indicating whether the inited. </summary>
        /// <value> True if inited, false if not. </value>
        public bool Inited { get; protected set; }

        /// <summary> Constructor. </summary>
        /// <param name="states"> The states.</param>
        public ControllerProviderBase(ControllerState[] states)
        {
            this.states = states;
        }

        /// <summary> Gets the number of controllers. </summary>
        /// <value> The number of controllers. </value>
        public abstract int ControllerCount { get; }

        /// <summary> Executes the 'pause' action. </summary>
        public abstract void OnPause();

        /// <summary> Executes the 'resume' action. </summary>
        public abstract void OnResume();

        /// <summary> Updates this object. </summary>
        public abstract void Update();

        /// <summary> Executes the 'destroy' action. </summary>
        public abstract void OnDestroy();

        /// <summary> Trigger haptic vibration. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <param name="durationSeconds"> (Optional) The duration in seconds.</param>
        /// <param name="frequency">       (Optional) The frequency.</param>
        /// <param name="amplitude">       (Optional) The amplitude.</param>
        public virtual void TriggerHapticVibration(int controllerIndex, float durationSeconds = 0.1f, float frequency = 200f, float amplitude = 0.8f) { }

        /// <summary> Recenters this object. </summary>
        public virtual void Recenter() { }
    }
}