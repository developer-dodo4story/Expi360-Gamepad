using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using EnhancedDodoServer;

namespace GamePad
{
    /// <summary>
    /// Custom button implementing OnPress and OnRelease events
    /// </summary>
    public class InputButton : Button
    {
        UnityAction onPress, onRelease;
        public bool Pressed { get; private set; }

        /// <summary>
        /// Button implements IPointerUpHandler - overridden OnPointerDown determines when the button was pressed
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Pressed = true;
            if (onPress != null)
                onPress();
        }
        /// <summary>
        /// Button implements IPointerUpHandler - overridden OnPointerUp determines when the button was released
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Pressed = false;
            if (onRelease != null)
                onRelease();
        }
    }
}