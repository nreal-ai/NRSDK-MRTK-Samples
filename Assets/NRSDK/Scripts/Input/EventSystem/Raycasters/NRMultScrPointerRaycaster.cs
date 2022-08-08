/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* NRSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary> A nr multiply screen pointer raycaster. </summary>
    [DisallowMultipleComponent]
    public class NRMultScrPointerRaycaster : NRPointerRaycaster
    {
        /// <summary> The mouse. </summary>
        public GameObject Mouse;

        /// <summary> The camera. </summary>
        private Camera m_UICamera;
        /// <summary> Width of the screen. </summary>
        private float m_ScreenWidth;
        /// <summary> Height of the screen. </summary>
        private float m_ScreenHeight;
        /// <summary> The last touch. </summary>
        private Vector3 m_LastTouch = m_FarAwayPos;

        /// <summary> The far away position. </summary>
        private static Vector3 m_FarAwayPos = Vector3.one * 10000f;

        /// <summary> <para>See MonoBehaviour.Awake.</para> </summary>
        protected override void Awake()
        {
            base.Awake();
            m_UICamera = gameObject.GetComponent<Camera>();
            //var resolution = NRPhoneScreen.Resolution * NRVirtualDisplayer.ScaleFactor;
            var resolution = NRPhoneScreen.Resolution;
            m_ScreenWidth = resolution.x;
            m_ScreenHeight = resolution.y;
        }

        /// <summary> Updates the screen size described by size. </summary>
        /// <param name="size"> The size.</param>
        public void UpdateScreenSize(Vector2 size)
        {
            m_ScreenWidth = size.x;
            m_ScreenHeight = size.y;
        }

        /// <summary> Raycasts this object. </summary>
        public override void Raycast()
        {
            sortedRaycastResults.Clear();
            breakPoints.Clear();

            var zScale = transform.lossyScale.z;
            var amountDistance = (FarDistance - NearDistance) * zScale;
            float distance;
            Ray ray;
            RaycastResult firstHit = default(RaycastResult);
            distance = amountDistance;

            var touch_x = NRVirtualDisplayer.SystemButtonState.originTouch.x;
            var touch_y = NRVirtualDisplayer.SystemButtonState.originTouch.y;
            var realTouchPos = new Vector3((touch_x + 1) * m_ScreenWidth * 0.5f, (touch_y + 1) * m_ScreenHeight * 0.5f, 0f);
            Vector3 touchpos = NRVirtualDisplayer.SystemButtonState.pressing ? realTouchPos : m_LastTouch;
            m_LastTouch = NRVirtualDisplayer.SystemButtonState.pressing ? touchpos : m_FarAwayPos;
            touchpos = m_UICamera.ScreenToWorldPoint(touchpos);

            //NRDebugger.Info("[PhoneDisplay] origin touch:{0} realTouchPos:{1} touchpos:{2} screenWidth:{3} screenHeight:{4}",
            //    MultiScreenController.SystemButtonState.originTouch, realTouchPos, touchpos, m_ScreenWidth, m_ScreenHeight);

            if (Mouse)
                Mouse.transform.position = touchpos + m_UICamera.transform.forward * 0.3f;
            ray = new Ray(touchpos, m_UICamera.transform.forward);

            breakPoints.Add(touchpos);

            eventCamera.farClipPlane = eventCamera.nearClipPlane + distance;
            eventCamera.orthographicSize = m_UICamera.orthographicSize;
            eventCamera.aspect = m_UICamera.aspect;
            eventCamera.transform.position = ray.origin - (ray.direction * eventCamera.nearClipPlane);
            eventCamera.transform.rotation = Quaternion.LookRotation(ray.direction, transform.up);

            Raycast(ray, distance, sortedRaycastResults);

            firstHit = FirstRaycastResult();

            if (firstHit.isValid && firstHit.gameObject)
            {
                var button = firstHit.gameObject.GetComponent<NRButton>();
                if (button != null)
                {
                    button.OnHover(firstHit);
                }
            }

            breakPoints.Add(firstHit.isValid ? firstHit.worldPosition : ray.GetPoint(distance));

#if UNITY_EDITOR
            if (showDebugRay)
                Debug.DrawLine(breakPoints[0], breakPoints[1], firstHit.isValid ? Color.green : Color.red);
#endif
        }

        /// <summary> Graphic raycast. </summary>
        /// <param name="canvas">                 The canvas.</param>
        /// <param name="ignoreReversedGraphics"> True to ignore reversed graphics.</param>
        /// <param name="ray">                    The ray.</param>
        /// <param name="distance">               The distance.</param>
        /// <param name="raycaster">              The raycaster.</param>
        /// <param name="raycastResults">         The raycast results.</param>
        public override void GraphicRaycast(Canvas canvas, bool ignoreReversedGraphics, Ray ray, float distance, NRPointerRaycaster raycaster, List<RaycastResult> raycastResults)
        {
            if (canvas == null) { return; }

            var eventCamera = raycaster.eventCamera;
            var screenCenterPoint = eventCamera.WorldToScreenPoint(eventCamera.transform.position); //new Vector2(m_ScreenWidth * 0.5f, m_ScreenHeight * 0.5f + 1800);
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            for (int i = 0; i < graphics.Count; ++i)
            {
                var graphic = graphics[i];
                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget) { continue; }

                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, screenCenterPoint, eventCamera)) { continue; }

                if (ignoreReversedGraphics && Vector3.Dot(ray.direction, graphic.transform.forward) <= 0f) { continue; }

                if (!graphic.Raycast(screenCenterPoint, eventCamera)) { continue; }

                float dist;
                new Plane(graphic.transform.forward, graphic.transform.position).Raycast(ray, out dist);
                if (dist > distance) { continue; }
                var racastResult = new RaycastResult
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
                };
                raycastResults.Add(racastResult);
            }
        }
    }
    
}
