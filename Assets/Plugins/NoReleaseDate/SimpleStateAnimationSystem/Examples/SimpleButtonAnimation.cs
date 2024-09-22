using DG.Tweening;
using EventButtonSystem.Runtime;
using UISystem.UIAnimationSystem.New;
using UnityEngine;

namespace SimpleStateAnimationSystem.Examples
{
    [RequireComponent(typeof(ButtonWithEvents))]
    public class SimpleButtonAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField, Range(0, 10)] private float _normalScale = 1f;
        [SerializeField, Range(0, 10)] private float _highlightedScale = 1.1f;
        [SerializeField, Range(0, 10)] private float _pressedScale = 0.9f;
        [SerializeField, Range(0, 10)] private float _selectedScale = 1.1f;
        [SerializeField, Range(0, 10)] private float _disabledScale = 1f;
        [SerializeField] private float _animDuration = 0.2f;
        [SerializeField] private Ease _animEase = Ease.OutSine;
        
        private ButtonWithEvents _button;
        private StateAnimations<PublicSelectableState> _buttonAnims;
        private Tweener _normalTween;
        private Tweener _highlightedTween;
        private Tweener _pressedTween;
        private Tweener _selectedTween;
        private Tweener _disabledTween;

        private void Awake()
        {
            _button = GetComponent<ButtonWithEvents>();
            
            var toNormalAnimation = new StateAnimation(ToNormal, KillToNormal);
            var toHighlightedAnimation = new StateAnimation(ToHighlighted, KillToHighlighted);
            var toPressedAnimation = new StateAnimation(ToPressed, KillToPressed);
            var toSelectedAnimation = new StateAnimation(ToSelected, KillToSelected);
            var toDisabledAnimation = new StateAnimation(ToDisabled, KillToDisabled);
            
            _buttonAnims = new StateAnimations<PublicSelectableState>.Builder()
                .SetAnimation(PublicSelectableState.Normal, toNormalAnimation)
                .SetAnimation(PublicSelectableState.Highlighted, toHighlightedAnimation)
                .SetAnimation(PublicSelectableState.Pressed, toPressedAnimation)
                .SetAnimation(PublicSelectableState.Selected, toSelectedAnimation)
                .SetAnimation(PublicSelectableState.Disabled, toDisabledAnimation)
                .Build(GetCurrentState);
        }
        
        private void OnEnable() => _button.onStateChange += OnButtonStateChange;

        private void OnDisable() => _button.onStateChange -= OnButtonStateChange;

        private void OnDestroy() => _buttonAnims.Dispose();

        private void OnButtonStateChange(PublicSelectableState state) => _buttonAnims.UpdateState();

        #region Animations
        
        private PublicSelectableState GetCurrentState() => _button.currentState;

        private void ToNormal() => 
            _normalTween = _button.transform.DOScale(_normalScale, _animDuration).SetEase(_animEase).SetUpdate(true);

        private void ToHighlighted() => 
            _highlightedTween = _button.transform.DOScale(_highlightedScale, _animDuration).SetEase(_animEase).SetUpdate(true);
        
        private void ToPressed() =>
            _pressedTween = _button.transform.DOScale(_pressedScale, _animDuration).SetEase(_animEase).SetUpdate(true);
        
        private void ToSelected() =>
            _selectedTween = _button.transform.DOScale(_selectedScale, _animDuration).SetEase(_animEase).SetUpdate(true);
        
        private void ToDisabled() =>
            _disabledTween = _button.transform.DOScale(_disabledScale, _animDuration).SetEase(_animEase).SetUpdate(true);

        private void KillToNormal() => _normalTween?.Kill();
        
        private void KillToHighlighted() => _highlightedTween?.Kill();
        
        private void KillToPressed() => _pressedTween?.Kill();
        
        private void KillToSelected() => _selectedTween?.Kill();
        
        private void KillToDisabled() => _disabledTween?.Kill();

        #endregion

    }
}