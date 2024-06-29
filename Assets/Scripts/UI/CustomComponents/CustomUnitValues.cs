using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    [ExecuteAlways]
    public class CustomUnitValues : MonoBehaviour
    {
        [SerializeField] private bool updateCache = false;

        public bool RunOnlyOnce = false;

        public List<CustomValue> Values = null!;

        private void Start()
        {
            refreshCache();

            update();
        }

        private void Update()
        {
            if (RunOnlyOnce) return;

            update();
        }

        private void OnValidate()
        {
            if (!updateCache) return;

            updateCache = false;
            refreshCache();
        }

        private void refreshCache()
        {
            for (int i = 0; i < Values.Count; i++)
                Values[i].UpdateCache();
        }

        private void update()
        {
            for (int i = 0; i < Values.Count; i++)
            {
                CustomValue cv = Values[i];
                if (!cv.IsValid) continue;
                var prop = cv.PropInfo;
                if (prop is null) continue;

                switch (cv.Type)
                {
                    case FieldType.Float:
                        prop.SetValue(cv.Component, getValue(float.Parse(cv.Value, CultureInfo.InvariantCulture), cv.Unit));
                        break;
                    case FieldType.Vec2:
                        {
                            string val = cv.Value;
                            int index = val.IndexOf(':');

                            Vector2 currentVal = (Vector2)prop.GetValue(cv.Component);
                            currentVal[int.Parse(val.Substring(0, index))] = getValue(float.Parse(val.Substring(index + 1), CultureInfo.InvariantCulture), cv.Unit);

                            prop.SetValue(cv.Component, currentVal);
                        }
                        break;
                    case FieldType.Vec2Combined:
                        {
                            string val = cv.Value.Substring(2);
                            int index = val.IndexOf(':');

                            prop.SetValue(cv.Component, new Vector2(
                                getValue(float.Parse(val.Substring(0, index), CultureInfo.InvariantCulture), cv.Unit),
                                getValue(float.Parse(val.Substring(index + 1), CultureInfo.InvariantCulture), cv.Unit)
                            ));
                        }
                        break;
                }
            }
        }

        private float getValue(float value, Unit unit)
        {
            switch (unit)
            {
                case Unit.Pixel:
                    return value;

                case Unit.ViewWidth:
                    return value * Screen.width;
                case Unit.ViewHeight:
                    return value * Screen.height;
                case Unit.ViewMin:
                    return value * MathF.Min(Screen.width, Screen.height);
                case Unit.ViewMax:
                    return value * MathF.Max(Screen.width, Screen.height);

                case Unit.ThisWidth:
                    return value * ((RectTransform)transform).sizeDelta.x;
                case Unit.ThisHeight:
                    return value * ((RectTransform)transform).sizeDelta.y;
                case Unit.ThisMin: 
                    return value * MathF.Min(((RectTransform)transform).sizeDelta.x, ((RectTransform)transform).sizeDelta.y);
                case Unit.ThisMax:
                    return value * MathF.Max(((RectTransform)transform).sizeDelta.x, ((RectTransform)transform).sizeDelta.y);

                case Unit.ParentWidth:
                    return value * ((RectTransform)transform.parent).sizeDelta.x;
                case Unit.ParentHeight: 
                    return value * ((RectTransform)transform.parent).sizeDelta.y;
                case Unit.ParentMin: 
                    return value * MathF.Min(((RectTransform)transform.parent).sizeDelta.x, ((RectTransform)transform.parent).sizeDelta.y);
                case Unit.ParentMax:
                    return value * MathF.Max(((RectTransform)transform.parent).sizeDelta.x, ((RectTransform)transform.parent).sizeDelta.y);

                default:
                    return float.NaN;
            }
        }

        [Serializable]
        public class CustomValue
        {
            [SerializeField] public Component Component = null!;
            [SerializeField] public string Property = string.Empty;
            [SerializeField] public string Value = string.Empty;
            [SerializeField] public Unit Unit = Unit.Pixel;

            public bool IsValid { get; private set; }
            public PropertyInfo? PropInfo { get; private set; }
            public FieldType Type { get; private set; }

            public void UpdateCache()
            {
                Type t = Component.GetType();
                PropInfo = t.GetProperty(Property, BindingFlags.Public | BindingFlags.Instance);

                if (PropInfo.PropertyType == typeof(float))
                    Type = FieldType.Float;
                else if (PropInfo.PropertyType == typeof(Vector2))
                {
                    Type = FieldType.Vec2;

                    if (Value.StartsWith("c:"))
                        Type = FieldType.Vec2Combined;
                }
                else
                    Type = FieldType.Invalid;

                IsValid =
                    Type != FieldType.Invalid &&
                    PropInfo != null &&
                    !string.IsNullOrEmpty(Value) &&
                    (Type != FieldType.Vec2 || Value.Contains(':'));
            }
        }

        public enum FieldType
        {
            Invalid = 0,
            Float,
            Vec2,
            Vec2Combined,
        }

        public enum Unit
        {
            Pixel = 0,

            ViewWidth,
            ViewHeight,
            ViewMin,
            ViewMax,

            ThisWidth,
            ThisHeight,
            ThisMin,
            ThisMax,

            ParentWidth,
            ParentHeight,
            ParentMin,
            ParentMax,
        }
    }
}
