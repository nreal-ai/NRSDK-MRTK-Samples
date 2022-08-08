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
    public interface IHandStatesService
    {
        bool IsRunning { get; }

        bool RunService();

        bool StopService();

        void UpdateStates(HandState[] handStates);

        void PauseService();

        void ResumeService();

        void DestroyService();
    }
}
