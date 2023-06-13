namespace NRKernal
{
    using System.Collections.Generic;
    using UnityEngine;

    public class NRHandMeshJointConfig : MonoBehaviour
    {
        public Vector3 RotationOffset;
        public HandEnum HandEnum;
        public List<Transform> HandJoint;
    }
}
