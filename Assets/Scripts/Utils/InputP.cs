using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class InputP
    {
        public const int MouseId = ushort.MaxValue;

        public static int TouchCount { get; private set; }

        static Dictionary<int, TouchP> touches = new Dictionary<int, TouchP>()
        {
            { MouseId, default }
        };

        public delegate void TouchEvent(TouchP touch);

        public static event TouchEvent? OnTouchDown;
        public static event TouchEvent? OnTouchUp;

        private static float updateTime = float.NaN;

        public static void Update()
        {
            if (Time.time == updateTime) return; // update max once per frame

            updateTime = Time.time;

            List<int> newTouches = new List<int>();
            List<TouchP> newTouchesDown = new List<TouchP>();

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (!touches.ContainsKey(touch.fingerId))
                {
                    touches.Add(touch.fingerId, new TouchP(touch.fingerId, touch));
                    newTouchesDown.Add(touches[touch.fingerId]);
                }
                else
                    touches[touch.fingerId] = new TouchP(touch.fingerId, touch, touches[touch.fingerId].StartTime,
                        MouseButtons.None);

                newTouches.Add(touch.fingerId);
            }

            List<int> touchKeys = new List<int>(touches.Keys);
            foreach (int key in touchKeys)
                if (key != MouseId && !newTouches.Contains(key))
                {
                    OnTouchUp?.Invoke(touches[key]);
                    touches.Remove(key);
                }

            TouchCount = touches.Count;
            if (!Input.mousePresent)
                TouchCount--;// remove mouse from touchCount

            for (int i = 0; i < newTouchesDown.Count; i++)
                OnTouchDown?.Invoke(newTouchesDown[i]);

            // mouse
            if (!Input.mousePresent)
                return;

            TouchP mt = touches[MouseId]; // mouse touch

            if (getCurrentMouseButtons() == MouseButtons.None)
            {
                if (mt.Touching)
                { // Touching == true == not "null"
                    if (mt.Touch.phase == TouchPhase.Ended)
                    {
                        OnTouchUp?.Invoke(mt);
                        touches[MouseId] = default; // set to "null"
                        TouchCount--; // remove mouse from touchCount
                    }
                    else
                    {
                        mt.Touch.phase = TouchPhase.Ended;
                        mt.Touch.position = (Vector2)Input.mousePosition;
                        touches[MouseId] = mt;
                    }
                }
                else
                    TouchCount--; // remove mouse from touchCount
                return;
            }

            bool mouseDown = false;
            if (!mt.Touching)
            {
                mt = new TouchP(MouseId, default) { Touch = new Touch() { fingerId = MouseId } };
                mt.Touch.position = Input.mousePosition;
                mt.MouseButtons = getCurrentMouseButtons();
                mouseDown = true;
            }
            else if (mt.Touch.position != (Vector2)Input.mousePosition)
                mt.Touch.phase = TouchPhase.Moved;
            else
                mt.Touch.phase = TouchPhase.Stationary;

            mt.Touch.position = Input.mousePosition;
            mt.MouseButtons = getCurrentMouseButtons();

            touches[MouseId] = mt;

            if (mouseDown)
                OnTouchDown?.Invoke(mt);
        }

        public static TouchP? GetTouch(int index)
        {
            if (index == 0 && Input.mousePresent && TouchCount == touches.Count) // touchCount only equals touches.Count if mouse btn is down
                return touches[MouseId];
            else if (index < 0 || index >= TouchCount)
                return null;
            else
                return touches.OrderBy(item => item.Key).ToArray()[index].Value;
        }

        public static TouchP GetTouchById(int id)
            => touches[id];

        public static TouchP? GetClosestTouch(Vector2 pos)
        {
            if (TouchCount == 0) return null;

            return touches
                .Where(item => item.Key != MouseId || TouchCount == touches.Count) // remove mouse if it isn't held down
                .MinItem(item => (item.Value.Touch.position - pos).sqrMagnitude)
                .Value;
        }

        public static int TouchDownCount()
            => touches
                .Where(item => item.Key != MouseId || TouchCount == touches.Count) // remove mouse if it isn't held down
                .Where(item => item.Value.Touch.phase == TouchPhase.Began)
                .Count();
        public static int TouchingCount()
            => touches
                .Where(item => item.Key != MouseId || TouchCount == touches.Count) // remove mouse if it isn't held down
                .Where(item => item.Value.Touch.phase != TouchPhase.Began && item.Value.Touch.phase != TouchPhase.Ended)
                .Count();
        public static int TouchUpCount()
            => touches
                .Where(item => item.Key != MouseId || TouchCount == touches.Count) // remove mouse if it isn't held down
                .Where(item => item.Value.Touch.phase == TouchPhase.Ended)
                .Count();

        private static MouseButtons getCurrentMouseButtons()
            => (MouseButtons)((Input.GetMouseButton(0) ? 1 : 0)
                | (Input.GetMouseButton(1) ? 2 : 0)
                | (Input.GetMouseButton(2) ? 4 : 0));
    }

    public struct TouchP
    {
        public readonly int Id;

        public Touch Touch;
        public readonly bool Touching;
        public readonly float StartTime;
        public readonly float Duration => Time.time - StartTime;
        public MouseButtons MouseButtons;

        public TouchP(int _id, Touch _touch)
        {
            Id = _id;

            Touch = _touch;
            Touching = true;
            StartTime = Time.time;
            MouseButtons = MouseButtons.None;
        }

        public TouchP(int _id, Touch _touch, float _startTime, MouseButtons _mouseButtons)
        {
            Id = _id;

            Touch = _touch;
            Touching = true;
            StartTime = _startTime;
            MouseButtons = _mouseButtons;
        }
    }

    [Flags]
    public enum MouseButtons : byte
    {
        None = 0,
        Left = 1,
        Right = 2,
        Middle = 4,
    }
}
