using System;
using TheraBytes.BetterUi;
using UnityEngine.EventSystems;

namespace EventButtonSystem.Runtime
{
    public class BetterButtonWithEvents : BetterButton, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<PublicSelectableState> onStateChange;
        public event Action<PointerEventData> onEnter;
        public event Action<PointerEventData> onExit;
        public event Action<PointerEventData> onDown;
        public event Action<PointerEventData> onUp;
        public event Action<AxisEventData> onMove;
        public event Action<PointerEventData> onDrag;
        public event Action<PointerEventData> onBeginDrag;
        public event Action<PointerEventData> onEndDrag;

        public PublicSelectableState currentState =>
            currentSelectionState switch
            {
                SelectionState.Normal => PublicSelectableState.Normal,
                SelectionState.Highlighted => PublicSelectableState.Highlighted,
                SelectionState.Pressed => PublicSelectableState.Pressed,
                SelectionState.Selected => PublicSelectableState.Selected,
                SelectionState.Disabled => PublicSelectableState.Disabled,
                _ => throw new ArgumentOutOfRangeException()
            };

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            onStateChange?.Invoke(SelectableStateToPublicSelectableState(state));
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onEnter?.Invoke(eventData);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            onExit?.Invoke(eventData);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onDown?.Invoke(eventData);
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onUp?.Invoke(eventData);
        }
        
        public override void OnMove(AxisEventData axisEventData)
        {
            base.OnMove(axisEventData);
            onMove?.Invoke(axisEventData);
        }

        public void OnBeginDrag(PointerEventData eventData) => onBeginDrag?.Invoke(eventData);

        public void OnDrag(PointerEventData eventData) => onDrag?.Invoke(eventData);

        public void OnEndDrag(PointerEventData eventData) => onEndDrag?.Invoke(eventData);
        
        private static PublicSelectableState SelectableStateToPublicSelectableState(SelectionState state) =>
            state switch
            {
                SelectionState.Normal => PublicSelectableState.Normal,
                SelectionState.Highlighted => PublicSelectableState.Highlighted,
                SelectionState.Pressed => PublicSelectableState.Pressed,
                SelectionState.Selected => PublicSelectableState.Selected,
                SelectionState.Disabled => PublicSelectableState.Disabled,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
    }
}