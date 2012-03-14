using System;

namespace Bespoke.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class XmlRequiredAttribute : Attribute
    {
		public bool IsRequired
		{
			get
			{
				return mIsRequired;
			}
			set
			{
				mIsRequired = value;
			}
		}

        public bool InvokeConstructor
        {
            get
            {
                return mInvokeConstructor;
            }
            set
            {
                mInvokeConstructor = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                return mDefaultValue;
            }
            set
            {
                mDefaultValue = value;
            }
        }

        public XmlRequiredAttribute()
            : this(true, false, null)
        {
        }

        public XmlRequiredAttribute(bool isRequired, bool invokeConstructor)
            : this(false, true, null)
        {
        }

        public XmlRequiredAttribute(bool isRequired, bool invokeConstructor, object defaultValue)
		{
            mIsRequired = isRequired;
            mInvokeConstructor = invokeConstructor;
            mDefaultValue = defaultValue;
		}

		private bool mIsRequired;
        private bool mInvokeConstructor;
        private object mDefaultValue;
    }
}
