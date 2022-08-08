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
    public class NRDisplaySubsystemDescriptor : IntegratedSubsystemDescriptor<NRDisplaySubsystem>
    {
        public const string Name = "Subsystem.Display";
        public override string id => Name;
    }

    public class NRDisplaySubsystem : IntegratedSubsystem<NRDisplaySubsystemDescriptor>
    {
        internal static NativeMultiDisplay NativeMultiDisplay { get; private set; }

        public NRDisplaySubsystem(NRDisplaySubsystemDescriptor descriptor) : base(descriptor)
        {
            NativeMultiDisplay = new NativeMultiDisplay();
#if !UNITY_EDITOR
            NativeMultiDisplay.Create();
#endif
        }

        public override void Start()
        {
            if (running)
            {
                return;
            }

            base.Start();

#if !UNITY_EDITOR
            NativeMultiDisplay.Start();
#endif
        }

        public override void Pause()
        {
            if (!running)
            {
                return;

            }

            base.Pause();

#if !UNITY_EDITOR
            NativeMultiDisplay.Pause();
#endif
        }

        public override void Resume()
        {
            if (running)
            {
                return;
            }

            base.Resume();

#if !UNITY_EDITOR
            NativeMultiDisplay.Resume();
#endif
        }

        public override void Stop()
        {
            if (!running)
            {
                return;
            }

            base.Stop();

#if !UNITY_EDITOR
            NativeMultiDisplay.Stop();
            NativeMultiDisplay.Destroy();
#endif
        }

        internal void ListenMainScrResolutionChanged(NRDisplayResolutionCallback callback)
        {
#if !UNITY_EDITOR
            NativeMultiDisplay.ListenMainScrResolutionChanged(callback);
#endif
        }
    }
}
