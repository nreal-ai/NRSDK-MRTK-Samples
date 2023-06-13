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
    using System.Runtime.InteropServices;

    /// <summary> Request flags for the meshing system. </summary>
    public enum NRMeshingFlags
    {
        NR_MESHING_FLAGS_COMPUTE_NORMAL = 1 << 0,
        NR_MESHING_FLAGS_NULL = 0,
    };

    /// <summary> State of a block mesh. </summary>
    public enum NRMeshingBlockState
    {
        /// Block mesh has been created.
        NR_MESHING_BLOCK_STATE_NEW,
        /// Block mesh has been updated.
        NR_MESHING_BLOCK_STATE_UPDATED,
        /// Block mesh has been deleted.
        NR_MESHING_BLOCK_STATE_DELETED,
        /// Block mesh is unchanged.
        NR_MESHING_BLOCK_STATE_UNCHANGED,
    };

    /// <summary> A transform struct in right-hand coordinate system </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NRTransform
    {
        public NativeVector3f position;
        public NativeVector4f rotation;
    };

    /// <summary> Axis aligned bounding box. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NRExtents
    {
        /// The center of the bounding box
        public NRTransform transform;
        /// The size of the bounding box
        public NativeVector3f extents;
    };
}
