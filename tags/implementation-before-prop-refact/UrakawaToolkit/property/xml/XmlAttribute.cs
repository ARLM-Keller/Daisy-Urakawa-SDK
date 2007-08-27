using System;
using System.Xml;
using urakawa.property;
using urakawa.xuk;

namespace urakawa.property.xml
{
	/// <summary>
	/// Default implementation of <see cref="XmlAttribute"/>
	/// </summary>
	public class XmlAttribute : IXukAble
	{
		XmlProperty mParent;
		string mName = "dummy";
		string mNamespace = "";
		string mValue = "";

		/// <summary>
		/// Constructor setting the parent <see cref="XmlProperty"/>
		/// </summary>
		/// <param name="parent">The parent</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the parent is <c>null</c>
		/// </exception>
		protected internal XmlAttribute(XmlProperty parent)
		{
			if (parent == null)
			{
				throw new exception.MethodParameterIsNullException("The parent of an xml attribute can not be null");
			}
			mParent = parent;
		}

		#region XmlAttribute Members
		/// <summary>
    /// Creates a copy of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The copy</returns>
		/// <exception cref="exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="IGenericPropertyFactory"/> of the <see cref="urakawa.core.ITreePresentation"/> 
		/// to which <c>this</c> belongs is not a subclass of <see cref="IXmlPropertyFactory"/>
		/// </exception>
    public XmlAttribute copy()
		{
			string xukLN = getXukLocalName();
			string xukNS = getXukNamespaceUri();
			XmlAttribute copyAttr = getParent().getXmlPropertyFactory().createXmlAttribute(
				getParent(), xukLN, xukNS);
			if (copyAttr == null)
			{
				throw new exception.FactoryCanNotCreateTypeException(String.Format(
					"The xml property factory does not support creating xml attributes matching QName {0}:{1}",
					getXukNamespaceUri(), getXukLocalName()));
			}
			copyAttr.setQName(getLocalName(), getNamespaceUri());
			copyAttr.setValue(getValue());
			return copyAttr;
		}

    /// <summary>
    /// Gets the value of gthe <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The value</returns>
    public string getValue()
		{
			return mValue;
		}

    /// <summary>
    /// Sets the value of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <param name="newValue">The new value</param>
    public void setValue(string newValue)
		{
			mValue = newValue;
		}

    /// <summary>
    /// Gets the namespace of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The namespace</returns>
    public string getNamespaceUri()
		{
				return mNamespace;
		}

    /// <summary>
    /// Gets the local localName of the <see cref="XmlAttribute"/>
    /// </summary>
    /// <returns>The local localName</returns>
    public string getLocalName()
		{
			return mName;
		}

    /// <summary>
    /// Sets the QName of the <see cref="XmlAttribute"/> 
    /// </summary>
    /// <param name="newNamespace">The namespace part of the new QName</param>
    /// <param name="newName">The localName part of the new QName</param>
    public void setQName(string newName, string newNamespace)
		{
      mName = newName;
      mNamespace = newNamespace;
		}

    /// <summary>
    /// Gets the parent <see cref="XmlProperty"/> of <c>this</c>
    /// </summary>
    /// <returns></returns>
    public XmlProperty getParent()
		{
			return mParent;
		}

		/// <summary>
		/// Sets the parent <see cref="XmlProperty"/> of <c>this</c>. 
		/// Is intended for internal use by the owning <see cref="XmlProperty"/>,
		/// calling this method may lead to corruption of the data model
		/// </summary>
		/// <param name="newParent">The new parent</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when the new parent is <c>null</c>
		/// </exception>
		public void setParent(XmlProperty newParent)
		{
			if (newParent == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The parent of an xml attribute can not be null");
			}
			mParent = newParent;
		}
		#endregion

		#region IXUKAble members 

    /// <summary>
    /// Reads the <see cref="XmlAttribute"/> instance from an XmlAttribute element in a XUK file
    /// </summary>
    /// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
      if (source == null)
      {
        throw new exception.MethodParameterIsNullException("Xml Reader is null");
      }
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read XmlAttribute from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				string v = "";
				if (!source.IsEmptyElement)
				{
					v = source.ReadString();
				}
				mValue = v;

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of XmlAttribute: {0}", e.Message),
					e);
			}
    }

		/// <summary>
		/// Reads the attributes of a XmlAttribute xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			string name = source.GetAttribute("localName");
			if (name == null || name == "")
			{
				throw new exception.XukException("name attribute of XmlAttribute element is missing");
			}
			string ns = source.GetAttribute("namespaceUri");
			if (ns == null) ns = "";
			setQName(name, ns);
		}

    /// <summary>
    /// Writes a XmlAttribute element representing the <see cref="XmlAttribute"/> instance
    /// to a XUK file
    /// </summary>
    /// <param name="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(System.Xml.XmlWriter destination)
		{
			destination.WriteStartElement("XmlAttribute", urakawa.ToolkitSettings.XUK_NS);
			XukOutAttributes(destination);
			destination.WriteString(this.mValue);
			destination.WriteEndElement();
		}

		/// <summary>
		/// Writes the attributes of a XmlAttribute element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the write was succesful</returns>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			//localName is required
			if (mName == "")
			{
				throw new exception.XukException("The XmlAttribute has no name");
			}
			destination.WriteAttributeString("localName", mName);
			if (mNamespace != "") destination.WriteAttributeString("namespaceUri", mNamespace);
		}

		
		/// <summary>
		/// Gets the local localName part of the QName representing a <see cref="XmlAttribute"/> in Xuk
		/// </summary>
		/// <returns>The local localName part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="XmlAttribute"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		/// <summary>
		/// Gets a <see cref="string"/> representation of the attribute
		/// </summary>
		/// <returns>The <see cref="string"/> representation</returns>
		public override string ToString()
		{
			string displayName = getLocalName();
			if (getNamespaceUri() != "") displayName = getNamespaceUri() + ":" + displayName;
			return String.Format("{0}='{1}'", getValue().Replace("'", "''"));
		}

	}
}
