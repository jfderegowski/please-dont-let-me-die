using System;
using NoReleaseDate.Common.Runtime.Properties;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoReleaseDate.Plugins.UIComponents
{
    [ExecuteAlways]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class MinMaxRectSize : UIBehaviour, ILayoutElement, ILayoutIgnorer, ILayoutSelfController
    {
        #region ILayoutIgnorer

        public bool ignoreLayout => _ignoreLayout;

        #endregion
        
        #region ILayoutElement

        public float minWidth => _minWidth.hasValue ? _minWidth.Value : -1;

        public float preferredWidth
        {
            get
            {
                if (!_maxWidth.hasValue)
                    return -1;
                
                var maxWidth = GetMaxWidthFromLayoutElements();
                
                return Mathf.Min(maxWidth, _maxWidth.Value);
            }
        }

        public float minHeight => _minHeight.hasValue ? _minHeight.Value : -1;

        public float preferredHeight
        {
            get
            {
                if (!_maxHeight.hasValue)
                    return -1;
                
                var maxHeight = GetMaxHeightFromLayoutElements();
                
                return Mathf.Min(maxHeight, _maxHeight.Value);
            }
        }

        public float flexibleWidth => -1;
        
        public float flexibleHeight => -1;
        
        public int layoutPriority => _layoutPriority.hasValue ? _layoutPriority.Value : 1;
        
        public void CalculateLayoutInputHorizontal() { }

        public void CalculateLayoutInputVertical() { }

        #endregion

        #region ILayoutSelfController

        public void SetLayoutHorizontal()
        {
            if (_horizontalFit == FitMode.Unconstrained)
                return;
            
            const RectTransform.Axis axis = RectTransform.Axis.Horizontal;
            const int axisIndex = (int)axis;
            
            var size = _horizontalFit switch
            {
                FitMode.Unconstrained => throw new InvalidOperationException(),
                FitMode.MinSize => minWidth,
                FitMode.MaxSize => preferredWidth > minWidth ? preferredWidth : minWidth,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            RectTransform.SetSizeWithCurrentAnchors(axis, size);
        }

        public void SetLayoutVertical()
        {
            if (_verticalFit == FitMode.Unconstrained)
                return;
            
            const RectTransform.Axis axis = RectTransform.Axis.Vertical;
            const int axisIndex = (int)axis;
            
            var size = _verticalFit switch
            {
                FitMode.Unconstrained => throw new InvalidOperationException(),
                FitMode.MinSize => minHeight,
                FitMode.MaxSize => preferredHeight > minHeight ? preferredHeight : minHeight,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            RectTransform.SetSizeWithCurrentAnchors(axis, size);
        }

        #endregion

        private enum FitMode
        {
            Unconstrained,
            MinSize,
            MaxSize
        }
        
        private RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform)
                    _rectTransform = transform as RectTransform;

                return _rectTransform;
            }
        }
        
        [SerializeField] private bool _ignoreLayout;

        [Header("Width")]
        [SerializeField] private HasValue<float> _minWidth = new(-1);
        [SerializeField] private HasValue<float> _maxWidth = new(-1);

        [Header("Height")]
        [SerializeField] private HasValue<float> _minHeight = new(-1);
        [SerializeField] private HasValue<float> _maxHeight = new(-1);
        
        [Header("Fit Mode")]
        [SerializeField] private FitMode _horizontalFit = FitMode.Unconstrained;
        [SerializeField] private FitMode _verticalFit = FitMode.Unconstrained;

        [SerializeField, Space] private HasValue<int> _layoutPriority = new(1);

        private ILayoutElement[] _layoutElements;
        private RectTransform _rectTransform;
        
        protected override void OnValidate()
        {
            base.OnValidate();
            
            // Set dirty to update the layout
            if (gameObject.activeInHierarchy)
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

        protected override void Awake()
        {
            base.Awake();

            _layoutElements = GetComponents<ILayoutElement>();
        }
        
        private float GetMaxWidthFromLayoutElements()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) 
                _layoutElements = GetComponents<ILayoutElement>();
#endif
            
            if (_layoutElements == null || _layoutElements.Length == 0)
                return -1;
            
            float maxWidth = -1;
            foreach (var layoutElement in _layoutElements)
            {
                if (ReferenceEquals(layoutElement, this))
                    continue;
                
                if (layoutElement.preferredWidth > maxWidth)
                    maxWidth = layoutElement.preferredWidth;
            }

            return maxWidth;
        }
        
        private float GetMaxHeightFromLayoutElements()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) 
                _layoutElements = GetComponents<ILayoutElement>();
#endif
            
            if (_layoutElements == null || _layoutElements.Length == 0)
                return -1;
            
            float maxHeight = -1;
            foreach (var layoutElement in _layoutElements)
            {
                if (ReferenceEquals(layoutElement, this))
                    continue;
                
                if (layoutElement.preferredHeight > maxHeight)
                    maxHeight = layoutElement.preferredHeight;
            }

            return maxHeight;
        }
    }
}