using System;

namespace Bespoke.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class XmlAttributeNameAttribute : Attribute
    {
		public string XmlAttributeName
		{
			get
			{
				return mXmlAttributeName;
			}
			set
			{
				mXmlAttributeName = value;
			}
		}

        public XmlAttributeNameAttribute(string xmlAttributeName)
		{
			mXmlAttributeName = xmlAttributeName;
		}

		private string mXmlAttributeName;
    }
}
