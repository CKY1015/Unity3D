﻿using UnityEngine;
using System.Linq;
using Wrld.MapInput;
using Wrld.MapInput.Touch;
using Wrld.MapInput.Mouse;
using System;

namespace Wrld.MapCamera
{
    public class CameraInputHandler
    {
        struct InputFrame
        {
            public Touch[] Touches;

            public Vector3 MousePosition;

            public bool IsLeftDown;
            public bool IsLeftUp;

            public bool IsRightDown;
            public bool IsRightUp;

            public bool IsMiddleDown;
            public bool IsMiddleUp;

            public float MouseXDelta;
            public float MouseYDelta;
            public float MouseWheelDelta;

            public bool HasMouseMoved;

            public bool ShouldHandleEventsThisFrame;
        }

        bool m_isTouchSupported;
        InputFrame m_inputFrame;
        Vector2 m_previousMousePosition;
        IUnityInputProcessor m_touchInputProcessor;


        bool m_isMouseSupported;
        IUnityInputProcessor m_mouseInputProcessor;

        Func<bool> m_shouldConsumeInputDelegate;

        public CameraInputHandler()
        {
            var inputHandler = new UnityInputHandler(NativePluginRunner.API);

            if (UnityEngine.Input.touchSupported && UnityEngine.Input.multiTouchEnabled)
            {
                m_isTouchSupported = true;
                m_touchInputProcessor = new UnityTouchInputProcessor(inputHandler, Screen.width, Screen.height);
            }
            if(UnityEngine.Input.mousePresent)
            {
                m_isMouseSupported = true;
                m_mouseInputProcessor = new UnityMouseInputProcessor(inputHandler, Screen.width, Screen.height);
            }

            m_inputFrame = new InputFrame();
            m_previousMousePosition = Vector2.zero;
        }

        bool HasMouseMoved()
        {
            return (UnityEngine.Input.GetAxis("Mouse X") != 0) || (UnityEngine.Input.GetAxis("Mouse Y") != 0);
        }

        void UpdateInputFrame()
        {
            m_inputFrame.Touches = UnityEngine.Input.touches;

            m_inputFrame.MousePosition  = Input.mousePosition;

            m_inputFrame.IsLeftDown = Input.GetMouseButtonDown(0);
            m_inputFrame.IsLeftUp = Input.GetMouseButtonUp(0);

            m_inputFrame.IsRightDown = Input.GetMouseButtonDown(1);
            m_inputFrame.IsRightUp = Input.GetMouseButtonUp(1);

            m_inputFrame.IsMiddleDown = Input.GetMouseButtonDown(2);
            m_inputFrame.IsMiddleUp = Input.GetMouseButtonUp(2);

            m_inputFrame.MouseXDelta = m_inputFrame.MousePosition.x - m_previousMousePosition.x;
            m_inputFrame.MouseYDelta = m_inputFrame.MousePosition.y - m_previousMousePosition.y;

            m_inputFrame.MouseWheelDelta = Input.mouseScrollDelta.y;

            m_inputFrame.HasMouseMoved = (m_inputFrame.MouseXDelta != 0) || (m_inputFrame.MouseYDelta != 0);

            m_inputFrame.ShouldHandleEventsThisFrame = !(m_shouldConsumeInputDelegate != null && !m_shouldConsumeInputDelegate());

            m_previousMousePosition = m_inputFrame.MousePosition;
        }

        bool HasInputChanged()
        {
            bool inputChanged = false;
            if (m_isTouchSupported)
            {
                if (m_inputFrame.Touches.Any())
                {
                    inputChanged = true;
                }
            }
            if(m_isMouseSupported)
            {
                if (m_inputFrame.IsLeftDown || m_inputFrame.IsLeftUp
                    || m_inputFrame.IsMiddleDown || m_inputFrame.IsMiddleUp
                    || m_inputFrame.IsRightDown || m_inputFrame.IsRightUp
                    || m_inputFrame.HasMouseMoved || m_inputFrame.MouseWheelDelta != 0)
                {
                    inputChanged = true;
                }
            }

            return inputChanged;
        }

        bool HandleTouchInput()
        {
            var touches = m_inputFrame.Touches;

            var anyTouchesDown = touches.Any(_t => _t.phase == TouchPhase.Began) && m_inputFrame.ShouldHandleEventsThisFrame;
            var anyTouchesUp = touches.Any(_t => _t.phase == TouchPhase.Ended);
            int index = 0;

            foreach (var provokingTouch in touches)
            {
                var touchEvent = new TouchInputEvent(anyTouchesUp, anyTouchesDown, index++, provokingTouch.fingerId);
                touchEvent.pointerEvents.AddRange(touches.Select((_t, _i) =>
                        new TouchInputPointerEvent
                        {
                            x = TranslateGlobalXToLocalX(_t.position.x),
                            y = TranslateGlobalYToLocalY(_t.position.y),
                            pointerIdentity = _t.fingerId,
                            pointerIndex = _i
                        }));
                m_touchInputProcessor.HandleInput(touchEvent);
            }

            return touches.Length > 0;

        }
        float TranslateGlobalXToLocalX(float x)
        {
            return x;
        }

        float TranslateGlobalYToLocalY(float y)
        {
            float result = Screen.height - y;
            return result;
        }


        delegate void SendActionDelegate(MouseInputAction a);
        void SendActionToHandler(MouseInputEvent mouseEvent, MouseInputAction action)
        {
            mouseEvent.Action = action;
            m_mouseInputProcessor.HandleInput(mouseEvent);
        }

        void HandleMouseInput()
        {
            MouseInputEvent mouseEvent = new MouseInputEvent();

            mouseEvent.x = TranslateGlobalXToLocalX(m_inputFrame.MousePosition.x);
            mouseEvent.y = TranslateGlobalYToLocalY(m_inputFrame.MousePosition.y);
            mouseEvent.z = m_inputFrame.MouseWheelDelta;

            SendActionDelegate HandleMouseAction = _action => SendActionToHandler(mouseEvent, _action);

            //Left Button
            if (m_inputFrame.IsLeftDown && m_inputFrame.ShouldHandleEventsThisFrame)
            {
                HandleMouseAction(MouseInputAction.MousePrimaryDown);
            }
            if (m_inputFrame.IsLeftUp)
            {
                HandleMouseAction(MouseInputAction.MousePrimaryUp);
            }

            //Right Button
            if (m_inputFrame.IsRightDown && m_inputFrame.ShouldHandleEventsThisFrame)
            {
                HandleMouseAction(MouseInputAction.MouseSecondaryDown);
            }
            if (m_inputFrame.IsRightUp)
            {
                HandleMouseAction(MouseInputAction.MouseSecondaryUp);
            }

            //Middle Button
            if (m_inputFrame.IsMiddleDown && m_inputFrame.ShouldHandleEventsThisFrame)
            {
                HandleMouseAction(MouseInputAction.MouseMiddleDown);
            }
            if (m_inputFrame.IsMiddleUp)
            {
                HandleMouseAction(MouseInputAction.MouseMiddleUp);
            }

            //Mouse Wheel
            if (m_inputFrame.MouseWheelDelta != 0 && m_inputFrame.ShouldHandleEventsThisFrame)
            {
                HandleMouseAction(MouseInputAction.MouseWheel);
            }

            if (m_inputFrame.HasMouseMoved)
            {
                HandleMouseAction(MouseInputAction.MouseMove);
            }
        }

        public void Update()
        {
            UpdateInputFrame();

            if (HasInputChanged())
            {
                bool touchHandled = false;
                if (m_isTouchSupported)
                {
                    touchHandled = HandleTouchInput();
                }
                if(!touchHandled && m_isMouseSupported)
                {
                    HandleMouseInput();
                }
            }

            if (m_isTouchSupported)
            {
                m_touchInputProcessor.Update(Time.deltaTime);
            }
            if (m_isMouseSupported)
            {
                m_mouseInputProcessor.Update(Time.deltaTime);
            }
        }

        public void RegisterShouldConsumeInputDelegate(Func<bool> function)
        {
            m_shouldConsumeInputDelegate += function;
        }

        public void UnregisterShouldConsumeInputDelegate(Func<bool> function)
        {
            m_shouldConsumeInputDelegate -= function;
        }

    }
}
