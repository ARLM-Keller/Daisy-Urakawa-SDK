using System;
using urakawa.core;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;

namespace urakawa.examples
{
	/// <summary>
	/// Custom <see cref="IGenericPropertyFactory"/> that constructs <see cref="ExampleCustomProperty"/>s
	/// in addition to the standard <see cref="Property"/>s <see cref="XmlProperty"/> and <see cref="ChannelsProperty"/>
	/// </summary>
	public class ExampleCustomPropertyFactory : PropertyFactory
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ExampleCustomPropertyFactory() : base()
		{
		}

		/// <summary>
		/// Creates a <see cref="Property"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="Property"/> or <c>null</c> if the given QName is not supported</returns>
		public override Property createProperty(string localName, string namespaceUri)
		{
			if (localName == "ExampleCustomProperty" && namespaceUri == ExampleCustomDataModelFactory.EX_CUST_NS)
			{
				ExampleCustomProperty newProp = new ExampleCustomProperty();
				newProp.Presentation = Presentation;
				return newProp;
			}
			return base.createProperty(localName, namespaceUri);
		}
	}
}