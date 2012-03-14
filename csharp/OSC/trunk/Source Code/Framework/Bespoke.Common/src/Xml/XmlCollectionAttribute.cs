using System;

namespace Bespoke.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class XmlCollectionAttribute : Attribute
    {
        public string CollectionElementName
		{
			get
			{
                return mCollectionElementName;
			}
			set
			{
                mCollectionElementName = value;
			}
		}

        public string ItemElementName
        {
            get
            {
                return mItemElementName;
            }
            set
            {
                mItemElementName = value;
            }
        }

        public Type ItemType
        {
            get
            {
                return mItemType;
            }
            set
            {
                mItemType = value;
            }
        }

        public XmlCollectionAttribute(string collectionElementName, string itemElementName, Type itemType)
		{
            mCollectionElementName = collectionElementName;
            mItemElementName = itemElementName;
            mItemType = itemType;
		}

		private string mCollectionElementName;
        private string mItemElementName;
        private Type mItemType;
    }
}
