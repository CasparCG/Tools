using System;
using System.Xml;

namespace Bespoke.Common
{
	/// <summary>
	/// Xml element base class.
	/// </summary>
	public abstract class XmlElementBase
	{
		/// <summary>
		/// Load a XmlElement object from a file.
		/// </summary>
		/// <param name="fileName">The name of the file storing the SceneElement.</param>
		/// <returns>The newly deserialzied XmlElement object.</returns>
		public static T Load<T>(string fileName) where T : XmlElementBase
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(fileName);

			return Load<T>(xmlDocument.DocumentElement);
		}

		/// <summary>
		/// Load a XmlElement object from an XmlNode.
		/// </summary>
		/// <param name="elementNode">The name of the Xml node storing the SceneElement.</param>
		/// <returns>The newly deserialzied XmlElement object.</returns>
		public static T Load<T>(XmlNode elementNode) where T : XmlElementBase
		{
			return (T)XmlHelper.ParseElement(elementNode, typeof(T));
		}

		/// <summary>
		/// Save an XmlElement object to a file.
		/// </summary>
		/// <param name="fileName">The name of the file to save to.</param>
		public virtual void Save(string fileName)
		{
			XmlHelper.SaveToXml(this, fileName);
		}		

		/// <summary>
		/// Save an XmlElement object to an XmlWriter object.
        /// </summary>
        /// <param name="writer">The XmlWriter object to save to.</param>
        public virtual void Save(XmlWriter writer)
        {
            XmlHelper.SaveToXml(this, writer); 
        }
	}
}
