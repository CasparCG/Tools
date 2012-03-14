using System;

namespace Bespoke.Common
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple=false)]
    public class XmlElementNameAttribute : Attribute
    {
		public string XmlElementName
		{
			get
			{
				return mXmlElementName;
			}
			set
			{
				mXmlElementName = value;
			}
		}

		public XmlElementNameAttribute(string xmlElementName)
		{
			mXmlElementName = xmlElementName;
		}

		private string mXmlElementName;
    }
}
