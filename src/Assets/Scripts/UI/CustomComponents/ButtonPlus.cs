using System;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace BeamNG.RemoteControlUltra.UI.CustomComponents
{
    public class ButtonPlus : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<PointerEventData>? OnDown;
        public event Action<PointerEventData>? OnUp;

        public void OnPointerDown(PointerEventData eventData)
            => OnDown?.Invoke(eventData);

        public void OnPointerUp(PointerEventData eventData)
            => OnUp?.Invoke(eventData);
    }
}
