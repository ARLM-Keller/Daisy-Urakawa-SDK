using System;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of custom IProperty
	/// </summary>
	public class ExampleCustomProperty : urakawa.core.IProperty
	{
		private const string NS = "http://www.daisy.org/urakawa/example";

		private CoreNode mOwner;

		/// <summary>
		/// The data of the custom property
		/// </summary>
		public string CustomData = "";

		internal ExampleCustomProperty()
		{
			// 
			// TODO: Add constructor logic here
			//
		}
		#region IProperty Members

		/// <summary>
		/// Generates a copy of the instance
		/// </summary>
		/// <returns>The copy</returns>
		public urakawa.core.IProperty copy()
		{
			IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			ExampleCustomProperty theCopy 
				= (ExampleCustomProperty)propFact.createProperty("ExampleCustomProperty");
			theCopy.setOwner(getOwner());
			return theCopy;
		}

		/// <summary>
		/// Gets the owner <see cref="urakawa.core.ICoreNode"/>
		/// </summary>
		/// <returns>The owner</returns>
		public urakawa.core.ICoreNode getOwner()
		{
			// TODO:  Add ExampleCustomProperty.getOwner implementation
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="urakawa.core.ICoreNode"/>
		/// </summary>
		/// <param name="newOwner">The new owner</param>
		public void setOwner(urakawa.core.ICoreNode newOwner)
		{
			if (!mOwner.GetType().IsAssignableFrom(newOwner.GetType()))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"The owner must be a CoreNode of a subclass of CoreNode");
			}
			IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
			if (!propFact.GetType().IsSubclassOf(typeof(ExampleCustomPropertyFactory)))
			{
				throw new exception.OperationNotValidException(
					"The property factory of the presentation of the owner must subclass ExampleCustomPropertyFactory");
			}
			mOwner = (CoreNode)newOwner;
		}

		#endregion

		#region IXUKable Members

		/// <summary>
		/// Reads the instance from a ExampleCustomProperty element in a XUK document
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the instance was succesfully read</returns>
		public bool XUKin(System.Xml.XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Reader is null");
			}

			if (!(source.LocalName == "ExampleCustomProperty" 
				&& source.NamespaceURI == NS
				&& source.NodeType == System.Xml.XmlNodeType.Element))
			{
				return false;
			}

			CustomData = source.GetAttribute("CustomData");
			if (CustomData==null) CustomData = "";
			if (source.IsEmptyElement) return true;
			while (source.Read())
			{
				if (source.NodeType==System.Xml.XmlNodeType.Element)
				{
					break;
				}
				if (source.NodeType==System.Xml.XmlNodeType.EndElement) return true;
				if (source.EOF) break;
			}
			return false;
		}

		/// <summary>
		/// Writes an ExampleCustomProperty element to a XUK file representing the instance.
		/// </summary>
		/// <param name="destination">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the element was succesfully written</returns>
		public bool XUKout(System.Xml.XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("Xml Writer is null");
			}

			destination.WriteStartElement("ExampleCustomProperty", NS);
			destination.WriteAttributeString("CustomData", CustomData);
			destination.WriteEndElement();
			return true;
		}

		#endregion
	}
}