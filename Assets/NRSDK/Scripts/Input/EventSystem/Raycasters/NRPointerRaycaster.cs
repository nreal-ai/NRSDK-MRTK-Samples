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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    
    /// <summary> A nr pointer raycaster. </summary>
    [DisallowMultipleComponent]
    public class NRPointerRaycaster : EventCameraRaycaster
    {
        /// <summary> Values that represent mask type enums. </summary>
        public enum MaskTypeEnum
        {
            /// <summary> An enum constant representing the inclusive option. </summary>
            Inclusive,
            /// <summary> An enum constant representing the exclusive option. </summary>
            Exclusive
        }

        /// <summary> The hits. </summary>
        private static readonly RaycastHit[] hits = new RaycastHit[64];

        /// <summary> Type of the mask. </summary>
        public MaskTypeEnum maskType = MaskTypeEnum.Exclusive;
        /// <summary> The mask. </summary>
        public LayerMask mask;
        /// <summary> Gets the raycast mask. </summary>
        /// <value> The raycast mask. </value>
        public int raycastMask { get { return maskType == MaskTypeEnum.Inclusive ? (int)mask : ~mask; } }
        /// <summary> True to show, false to hide the debug ray. </summary>
        public bool showDebugRay = true;
        /// <summary> True to enable, false to disable the physics raycast. </summary>
        public bool enablePhysicsRaycast = true;
        /// <summary> True to enable, false to disable the graphic raycast. </summary>
        public bool enableGraphicRaycast = true;

        /// <summary> List of button event data. </summary>
        protected readonly List<NRPointerEventData> buttonEventDataList = new List<NRPointerEventData>();
        /// <summary> The sorted raycast results. </summary>
        protected readonly List<RaycastResult> sortedRaycastResults = new List<RaycastResult>();
        /// <summary> The break points. </summary>
        protected readonly List<Vector3> breakPoints = new List<Vector3>();

        /// <summary> The related hand. </summary>
        private ControllerHandEnum m_RelatedHand;
        /// <summary> Gets or sets the related hand. </summary>
        /// <value> The related hand. </value>
        public ControllerHandEnum RelatedHand
        {
            get
            {
                return NRInput.RaycastMode == RaycastModeEnum.Gaze ? NRInput.DomainHand : m_RelatedHand;
            }

            internal set
            {
                m_RelatedHand = value;
            }
        }

        /// <summary> Gets the break points. </summary>
        /// <value> The break points. </value>
        public List<Vector3> BreakPoints { get { return breakPoints; } }
        /// <summary> Gets information describing the hover event. </summary>
        /// <value> Information describing the hover event. </value>
        public NRPointerEventData HoverEventData { get { return buttonEventDataList.Count > 0 ? buttonEventDataList[0] : null; } }
        /// <summary> Gets a list of button event data. </summary>
        /// <value> A list of button event data. </value>
        public ReadOnlyCollection<NRPointerEventData> ButtonEventDataList { get { return buttonEventDataList.AsReadOnly(); } }

        /// <summary> <para>See MonoBehaviour.Start.</para> </summary>
        protected override void Start()
        {
            base.Start();
            buttonEventDataList.Add(new NRPointerEventData(this, EventSystem.current));
        }

        /// <summary> called by StandaloneInputModule, not supported. </summary>
        /// <param name="eventData">        Information describing the event.</param>
        /// <param name="resultAppendList"> List of result appends.</param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {

        }

        /// <summary> Raycasts. </summary>
        public virtual void Raycast()
        {
            sortedRaycastResults.Clear();
            breakPoints.Clear();

            var zScale = transform.lossyScale.z;
            var amountDistance = (FarDistance - NearDistance) * zScale;
            if (!gameObject.activeInHierarchy)
            {
                amountDistance = 0.0001f;
            }
            var origin = transform.TransformPoint(0f, 0f, NearDistance);
            breakPoints.Add(origin);

            Vector3 direction;
            float distance;
            Ray ray;
            RaycastResult firstHit = default(RaycastResult);

            direction = transform.forward;
            distance = amountDistance;
            ray = new Ray(origin, direction);

            eventCamera.farClipPlane = eventCamera.nearClipPlane + distance;
            eventCamera.transform.position = ray.origin - (ray.direction * eventCamera.nearClipPlane);
            eventCamera.transform.rotation = Quaternion.LookRotation(ray.direction, transform.up);

            Raycast(ray, distance, sortedRaycastResults);

            firstHit = FirstRaycastResult();
            breakPoints.Add(firstHit.isValid ? firstHit.worldPosition : ray.GetPoint(distance));
#if UNITY_EDITOR
            if (showDebugRay)
            {
                Debug.DrawLine(breakPoints[0], breakPoints[1], firstHit.isValid ? Color.green : Color.red);
            }
#endif
        }

        /// <summary> Gets raycaster result comparer. </summary>
        /// <returns> The raycaster result comparer. </returns>
        protected virtual Comparison<RaycastResult> GetRaycasterResultComparer()
        {
            return NRInputModule.defaultRaycastComparer;
        }

        /// <summary>
        /// override OnEnable & OnDisable on purpose so that this BaseRaycaster won't be registered into
        /// RaycasterManager. </summary>
        protected override void OnEnable()
        {
            //base.OnEnable();
            NRInputModule.AddRaycaster(this);
        }

        /// <summary> <para>See MonoBehaviour.OnDisable.</para> </summary>
        protected override void OnDisable()
        {
            //base.OnDisable();
            NRInputModule.RemoveRaycaster(this);
        }

        /// <summary> Gets scroll delta. </summary>
        /// <returns> The scroll delta. </returns>
        public virtual Vector2 GetScrollDelta()
        {
            return Vector2.zero;
        }

        /// <summary> First raycast result. </summary>
        /// <returns> A RaycastResult. </returns>
        public RaycastResult FirstRaycastResult()
        {
            for (int i = 0, imax = sortedRaycastResults.Count; i < imax; ++i)
            {
                if (!sortedRaycastResults[i].isValid)
                    continue;
                return sortedRaycastResults[i];
            }
            return default(RaycastResult);
        }

        /// <summary> Raycasts. </summary>
        /// <param name="ray">            The ray.</param>
        /// <param name="distance">       The distance.</param>
        /// <param name="raycastResults"> The raycast results.</param>
        public void Raycast(Ray ray, float distance, List<RaycastResult> raycastResults)
        {
            var results = new List<RaycastResult>();
            if (enablePhysicsRaycast)
            {
                PhysicsRaycast(ray, distance, results);
            }
            if (enableGraphicRaycast)
            {
                var tempCanvases = CanvasTargetCollector.GetCanvases();
                for (int i = tempCanvases.Count - 1; i >= 0; --i)
                {
                    var target = tempCanvases[i];
                    if (target == null || !target.enabled)
                        continue;
                    GraphicRaycast(target.canvas, target.ignoreReversedGraphics, ray, distance, this, results);
                }
            }
            var comparer = GetRaycasterResultComparer();
            if (comparer != null)
            {
                results.Sort(comparer);
            }
            for (int i = 0, imax = results.Count; i < imax; ++i)
            {
                raycastResults.Add(results[i]);
            }
        }

        /// <summary> Physics raycast. </summary>
        /// <param name="ray">            The ray.</param>
        /// <param name="distance">       The distance.</param>
        /// <param name="raycastResults"> The raycast results.</param>
        public virtual void PhysicsRaycast(Ray ray, float distance, List<RaycastResult> raycastResults)
        {
            var hitCount = Physics.RaycastNonAlloc(ray, hits, distance, raycastMask);
            for (int i = 0; i < hitCount; ++i)
            {
                raycastResults.Add(new RaycastResult
                {
                    gameObject = hits[i].collider.gameObject,
                    module = this,
                    distance = hits[i].distance,
                    worldPosition = hits[i].point,
                    worldNormal = hits[i].normal,
                    screenPosition = NRInputModule.ScreenCenterPoint,
                    index = raycastResults.Count,
                    sortingLayer = 0,
                    sortingOrder = 0
                });
            }
        }

        /// <summary> Graphic raycast. </summary>
        /// <param name="canvas">                 The canvas.</param>
        /// <param name="ignoreReversedGraphics"> True to ignore reversed graphics.</param>
        /// <param name="ray">                    The ray.</param>
        /// <param name="distance">               The distance.</param>
        /// <param name="raycaster">              The raycaster.</param>
        /// <param name="raycastResults">         The raycast results.</param>
        public virtual void GraphicRaycast(Canvas canvas, bool ignoreReversedGraphics, Ray ray, float distance, NRPointerRaycaster raycaster, List<RaycastResult> raycastResults)
        {
            if (canvas == null)
                return;

            var eventCamera = raycaster.eventCamera;
            var screenCenterPoint = NRInputModule.ScreenCenterPoint;
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            for (int i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget)
                    continue;

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, screenCenterPoint, eventCamera))
                    continue;

                if (ignoreReversedGraphics && Vector3.Dot(ray.direction, graphic.transform.forward) <= 0f)
                    continue;

                if (!graphic.Raycast(screenCenterPoint, eventCamera))
                    continue;

                float dist;
                new Plane(graphic.transform.forward, graphic.transform.position).Raycast(ray, out dist);
                if (float.IsNaN(dist) || dist > distance)
                    continue;

                raycastResults.Add(new RaycastResult
                {
                    gameObject = graphic.gameObject,
                    module = raycaster,
                    distance = dist,
                    worldPosition = ray.GetPoint(dist),
                    worldNormal = -graphic.transform.forward,
                    screenPosition = screenCenterPoint,
                    index = raycastResults.Count,
                    depth = graphic.depth,
                    sortingLayer = canvas.sortingLayerID,
                    sortingOrder = canvas.sortingOrder
                });
            }
        }
    }
    
}
