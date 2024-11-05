using UnityEngine;

namespace NoReleaseDate.Common.Runtime.PropertyAttributes
{
    public class LabelAttribute : PropertyAttribute
    {
        public readonly string Label;

        public LabelAttribute(string label) => Label = label;
    }
}
