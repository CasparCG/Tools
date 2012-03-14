using System;

namespace Bespoke.Common
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple=false)]
    public class XmlHasChildElementsAttribute : Attribute
    {
		public bool HasChildElements
		{
			get
			{
				return mHasChildElements;
			}
			set
			{
				mHasChildElements = value;
			}
		}

        public XmlHasChildElementsAttribute()
        {
            mHasChildElements = true;
        }

        public XmlHasChildElementsAttribute(bool hasChildElements)
		{
            mHasChildElements = hasChildElements;
		}

		private bool mHasChildElements;
    }
}
