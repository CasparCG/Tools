using System;
using System.Drawing;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Text;

namespace Bespoke.Common
{    
	/// <summary>
	/// Xml helper methods
	/// </summary>
	public static class XmlHelper
	{
        /// <summary>
        /// Parse an Xml element into a specifed type.
        /// </summary>
        /// <param name="elementNode">The xml node containing the element to parse.</param>
        /// <param name="elementType">The type of object to deserialize into.</param>
        /// <returns>The instance of the newly deserialized object.</returns>
        public static object ParseElement(XmlNode elementNode, Type elementType)
        {
            ConstructorInfo constructorInfo = elementType.GetConstructor(new Type[] { });
            object elementObject = constructorInfo.Invoke(null);

            XmlCollectionAttribute[] xmlCollectionClassAttributes = (XmlCollectionAttribute[])elementType.GetCustomAttributes(typeof(XmlCollectionAttribute), true);
            if (xmlCollectionClassAttributes.Length > 0)
            {
                if ((elementObject is IList == false))
                {
                    throw new Exception("Class marked as XmlCollection but type does not implement IList.");
                }

                XmlCollectionAttribute xmlCollectionClassAttribute = xmlCollectionClassAttributes[0];
                if ((elementNode.Name == xmlCollectionClassAttribute.CollectionElementName) && (elementNode.HasChildNodes))
                {
                    IList listObject = (IList)elementObject;
                    foreach (XmlNode childNode in elementNode.ChildNodes)
                    {
                        if (childNode.Name == xmlCollectionClassAttribute.ItemElementName)
                        {
                            object childValue = ParseElement(childNode, xmlCollectionClassAttribute.ItemType);
                            listObject.Add(childValue);
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] properties = elementType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    Type propertyType = property.PropertyType;

                    XmlIgnoreAttribute[] ignoreAttributes = (XmlIgnoreAttribute[])property.GetCustomAttributes(typeof(XmlIgnoreAttribute), true);
                    if (ignoreAttributes.Length > 0)
                    {
                        continue;
                    }

                    bool isRequired;
                    bool invokeConstructor;
                    object defaultValue;
                    XmlRequiredAttribute[] xmlRequiredAttributes = (XmlRequiredAttribute[])property.GetCustomAttributes(typeof(XmlRequiredAttribute), true);
                    if (xmlRequiredAttributes.Length > 0)
                    {
                        XmlRequiredAttribute xmlRequiredAttribute = xmlRequiredAttributes[0];

                        isRequired = xmlRequiredAttribute.IsRequired;
                        invokeConstructor = xmlRequiredAttribute.InvokeConstructor;
                        defaultValue = xmlRequiredAttribute.DefaultValue;
                    }
                    else
                    {
                        isRequired = true;
                        invokeConstructor = false;
                        defaultValue = null;
                    }

                    object value;
                    XmlHasChildElementsAttribute[] xmlHasChildElementsAttributes = (XmlHasChildElementsAttribute[])propertyType.GetCustomAttributes(typeof(XmlHasChildElementsAttribute), true);
                    if ((xmlHasChildElementsAttributes.Length > 0) && (xmlHasChildElementsAttributes[0].HasChildElements))
                    {
                        XmlElementNameAttribute[] xmlElementNameAttributes = (XmlElementNameAttribute[])propertyType.GetCustomAttributes(typeof(XmlElementNameAttribute), true);
                        string childElementName = (xmlElementNameAttributes.Length > 0 ? xmlElementNameAttributes[0].XmlElementName : property.Name);
                        XmlNode childElementNode = XmlHelper.GetChildNode(elementNode, childElementName);

                        if (childElementNode != null)
                        {
                            value = ParseElement(childElementNode, propertyType);
                        }
                        else
                        {
                            if (isRequired)
                            {
                                throw new Exception("Child element [" + childElementName + "] not found.");
                            }

                            if (invokeConstructor)
                            {
                                ConstructorInfo childElementConstructorInfo = propertyType.GetConstructor(new Type[] { });
                                value = childElementConstructorInfo.Invoke(null);
                            }
                            else
                            {
                                value = defaultValue;
                            }
                        }
                    }
                    else
                    {
                        XmlAttributeNameAttribute[] xmlAttributeNameAttributes = (XmlAttributeNameAttribute[])property.GetCustomAttributes(typeof(XmlAttributeNameAttribute), true);
                        string attributeName = (xmlAttributeNameAttributes.Length > 0 ? xmlAttributeNameAttributes[0].XmlAttributeName : property.Name);
                        value = XmlHelper.GetAttribute(elementNode, attributeName, propertyType, isRequired, defaultValue);

                        if ((value == defaultValue) && (isRequired == false) && (invokeConstructor))
                        {
                            if (propertyType.IsValueType)
                            {
                                value = Activator.CreateInstance(propertyType);
                            }
                            else
                            {
                                ConstructorInfo propertyConstructorInfo = propertyType.GetConstructor(new Type[] { });
                                value = propertyConstructorInfo.Invoke(null);
                            }
                        }
                    }

                    XmlCollectionAttribute[] xmlCollectionAttributes = (XmlCollectionAttribute[])property.GetCustomAttributes(typeof(XmlCollectionAttribute), true);
                    if (xmlCollectionAttributes.Length > 0)
                    {
                        if ((value is IList == false))
                        {
                            throw new Exception("Property marked as XmlCollection but type does not implement IList.");
                        }

                        XmlCollectionAttribute xmlCollectionAttribute = xmlCollectionAttributes[0];
                        XmlNode xmlCollectionNode = XmlHelper.GetChildNode(elementNode, xmlCollectionAttribute.CollectionElementName);
                        if ((xmlCollectionNode != null) && (xmlCollectionNode.HasChildNodes))
                        {
                            IList listValue = (IList)value;
                            foreach (XmlNode childNode in xmlCollectionNode.ChildNodes)
                            {
                                if (childNode.Name == xmlCollectionAttribute.ItemElementName)
                                {
                                    object childValue = ParseElement(childNode, xmlCollectionAttribute.ItemType);
                                    listValue.Add(childValue);
                                }
                            }
                        }
                    }

                    property.SetValue(elementObject, value, null);
                }
            }

            return elementObject;
        }

		/// <summary>
		/// Save an object to Xml.
		/// </summary>
		/// <param name="value">The object to serialize.</param>
		/// <param name="fileName">The file to save to.</param>
		public static void SaveToXml(object value, string fileName)
		{
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = true;
			writerSettings.NewLineOnAttributes = true;
			writerSettings.Encoding = Encoding.UTF8;

			XmlWriter writer = XmlWriter.Create(fileName, writerSettings);

			SaveToXml(value, writer);

			writer.Close();
		}

		/// <summary>
		/// Save an object to Xml.
		/// </summary>
		/// <param name="value">The object to serialize.</param>
		/// <param name="writer">The XmlWriter object to save to.</param>
		public static void SaveToXml(object value, XmlWriter writer)
		{
			Type type = value.GetType();

			XmlElementNameAttribute[] xmlElementNameAttributes = (XmlElementNameAttribute[])type.GetCustomAttributes(typeof(XmlElementNameAttribute), true);
			string elementName = (xmlElementNameAttributes.Length > 0 ? xmlElementNameAttributes[0].XmlElementName : type.Name);

			writer.WriteStartElement(elementName);

			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				Type propertyType = property.PropertyType;

				XmlIgnoreAttribute[] ignoreAttributes = (XmlIgnoreAttribute[])property.GetCustomAttributes(typeof(XmlIgnoreAttribute), true);
				if (ignoreAttributes.Length == 0)
				{
					XmlAttributeNameAttribute[] xmlAttributeNameAttributes = (XmlAttributeNameAttribute[])property.GetCustomAttributes(typeof(XmlAttributeNameAttribute), true);
					string attributeName = (xmlAttributeNameAttributes.Length > 0 ? xmlAttributeNameAttributes[0].XmlAttributeName : property.Name);
					object propertyValue = property.GetValue(value, null);
					if (propertyValue != null)
					{
						XmlHasChildElementsAttribute[] xmlHasChildElementsAttributes = (XmlHasChildElementsAttribute[])propertyType.GetCustomAttributes(typeof(XmlHasChildElementsAttribute), true);
						if ((xmlHasChildElementsAttributes.Length > 0) && (xmlHasChildElementsAttributes[0].HasChildElements))
						{
							SaveToXml(propertyValue, writer);
						}
						else
						{
							XmlCollectionAttribute[] xmlCollectionAttributes = (XmlCollectionAttribute[])property.GetCustomAttributes(typeof(XmlCollectionAttribute), true);
							if (xmlCollectionAttributes.Length > 0)
							{
								if ((propertyValue is IList == false))
								{
									throw new Exception("Property marked as XmlCollection but does not implement IList.");
								}

								XmlCollectionAttribute xmlCollectionAttribute = xmlCollectionAttributes[0];
								writer.WriteStartElement(xmlCollectionAttribute.CollectionElementName);

								IList listValue = (IList)propertyValue;
								foreach (object childValue in listValue)
								{
									SaveToXml(childValue, writer);
								}

								writer.WriteEndElement();

							}
							else
							{
								writer.WriteAttributeString(attributeName, (propertyValue != null ? XmlHelper.ToString(propertyValue) : String.Empty));
							}
						}
					}
				}
			}

			writer.WriteEndElement();
		}

		public static XmlNode GetChildNode(XmlNode parentNode, string childNodeName)
		{
			XmlNode foundChildNode = null;

			if (parentNode.HasChildNodes)
			{
				foreach (XmlNode childNode in parentNode.ChildNodes)
				{
					if (childNode.Name == childNodeName)
					{
						foundChildNode = childNode;
						break;
					}
				}
			}

			return foundChildNode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="attributeName"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetAttribute(XmlNode xmlNode, string attributeName, Type type)
		{
			return GetAttribute(xmlNode, attributeName, type, true, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		/// <param name="attributeName"></param>
		/// <param name="type"></param>
		/// <param name="required"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static object GetAttribute(XmlNode xmlNode, string attributeName, Type type, bool required, object defaultValue)
		{
			object value;

			XmlNode foundNode = xmlNode.Attributes.GetNamedItem(attributeName);
			if (foundNode == null)
			{
				if (required)
				{
					throw new Exception("Required attribute [" + attributeName + "] not found");
				}

                if ((defaultValue != null) && (defaultValue.GetType() != type))
				{
					throw new Exception("Default value specified with incompatible type.");
				}

				value = defaultValue;
			}
			else
			{
				value = GetValueFromString(type, foundNode.Value.Trim());
			}

			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xmlNode"></param>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(XmlNode xmlNode, string attributeName)
		{
			return GetAttribute<T>(xmlNode, attributeName, true, default(T));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xmlNode"></param>
		/// <param name="attributeName"></param>
		/// <param name="required"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetAttribute<T>(XmlNode xmlNode, string attributeName, bool required, T defaultValue)
		{
			T value;

			XmlNode foundNode = xmlNode.Attributes.GetNamedItem(attributeName);
			if (foundNode == null)
			{
				if (required)
				{
					throw new Exception("Required attribute [" + attributeName + "] not found");
				}

				value = defaultValue;
			}
			else
			{
				value = (T)GetValueFromString(typeof(T), foundNode.Value.Trim());
			}

			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToString<T>(T value)
		{
			object valueObject = value;

			Type type = value.GetType();
			string stringValue;

			if (type.IsEnum)
			{
				stringValue = valueObject.ToString();
			}
			else
			{
				switch (type.Name)
				{
					case "Point":
						{
							Point point = (Point)valueObject;
							stringValue = string.Format("{0}, {1}", point.X, point.Y);
							break;
						}

					case "Color":
						{
							Color color = (Color)valueObject;
							stringValue = string.Format("{0}, {1}, {2}, {3}", color.R, color.G, color.B, color.A);
							break;
						}

					case "Rectangle":
						{
							Rectangle rectangle = (Rectangle)valueObject;
							stringValue = string.Format("{0}, {1}, {2}, {3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
							break;
						}

					case "DateTime":
						{
							DateTime dateTime = (DateTime)valueObject;
#if WINDOWS
							stringValue = dateTime.ToBinary().ToString();
#else
							stringValue = dateTime.Ticks.ToString();
#endif
							break;
						}

					case "TimeSpan":
						{
							TimeSpan timeSpan = (TimeSpan)valueObject;
							stringValue = timeSpan.Ticks.ToString();
							break;
						}

					default:
						stringValue = valueObject.ToString();
						break;
				}
			}

			return stringValue;
		}

		private static Color ParseColor(string value)
		{
			try
			{
				string[] values = value.Split(DELIMITERS);
				if (values.Length == 3)
				{
					byte r = byte.Parse(values[0].Trim());
					byte g = byte.Parse(values[1].Trim());
					byte b = byte.Parse(values[2].Trim());

					return Color.FromArgb(r, g, b);
				}
#if WINDOWS
				else if (values.Length == 4)
				{
					byte r = byte.Parse(values[0].Trim());
					byte g = byte.Parse(values[1].Trim());
					byte b = byte.Parse(values[2].Trim());
					byte a = byte.Parse(values[3].Trim());

					return Color.FromArgb(r, g, b, a);
				}
#endif
				else
				{
					MemberInfo[] colorMemberInfoList = typeof(Color).GetMember(value);
					if (colorMemberInfoList.Length != 1)
					{
						throw new Exception("Could not find color value: " + value);
					}

					return (Color)typeof(Color).InvokeMember(colorMemberInfoList[0].Name, BindingFlags.GetProperty, null, null, null);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidCastException("Could not parse color", ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		private static Point ParsePoint(string value)
		{
			string[] values = value.Split(DELIMITERS);
			if (values.Length != 2)
			{
				throw new InvalidCastException("Could not parse point");
			}

			try
			{
				int x = int.Parse(values[0].Trim());
				int y = int.Parse(values[1].Trim());

				return new Point(x, y);
			}
			catch (Exception ex)
			{
				throw new InvalidCastException("Could not parse point", ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="stringValue"></param>
		/// <returns></returns>
		private static object GetValueFromString(Type type, string stringValue)
		{
			object valueObject;

			if (type.IsEnum)
			{
				valueObject = Enum.Parse(type, stringValue, true);
            }
			else
			{
				switch (type.Name)
				{
					case "Byte":
						valueObject = byte.Parse(stringValue);
						break;

					case "Int32":
						valueObject = int.Parse(stringValue);
						break;

					case "Single":
						valueObject = float.Parse(stringValue);
						break;

					case "Double":
						valueObject = double.Parse(stringValue);
						break;

					case "Boolean":
						valueObject = bool.Parse(stringValue);
						break;

					case "Color":
						valueObject = ParseColor(stringValue);
						break;

					case "IPAddress":
						valueObject = IPAddress.Parse(stringValue);
						break;

					case "Point":
						valueObject = ParsePoint(stringValue);
						break;

					case "DateTime":
#if WINDOWS
						valueObject = DateTime.FromBinary(long.Parse(stringValue));
#else
						valueObject = new DateTime(long.Parse(stringValue));
#endif
						break;

					case "TimeSpan":
						valueObject = TimeSpan.FromTicks(long.Parse(stringValue));
						break;

					default:
						valueObject = stringValue;
						break;
				}
			}

			return valueObject;
		}

		private static readonly char[] DELIMITERS = ",".ToCharArray();
	}
}
