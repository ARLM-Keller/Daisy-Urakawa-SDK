using System;
using urakawa.core;
//using urakawa.properties.channel;
//using urakawa.properties.xml;

namespace urakawa.core.property
{
	/// <summary>
	/// Default implementation of <see cref="ICorePropertyFactory"/> can not create any properties.
	/// Use the built-in sub-class of <see cref="urakawa.PropertyFactory"/> that support creation of 
	/// <see cref="urakawa.properties.channel.ChannelsProperty"/>s 
	/// and <see cref="urakawa.properties.xml.XmlProperty"/>s.
	/// Alternatively the user should create their own sub-class of CorePropertyFactory.
	/// </summary>
	public class CorePropertyFactory : ICorePropertyFactory
	{
    #region ICorePropertyFactory Members

    /// <summary>
    /// Creates a <see cref="IProperty"/> matching a given QName
    /// </summary>
		/// <param localName="localName">The local part of the QName</param>
		/// <param localName="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IProperty"/> or <c>null</c> if the given QName is not supported
		/// (which is always hte case)
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// 
		/// </exception>
    public virtual IProperty createProperty(string localName, string namespaceUri)
    {
			if (localName == null || namespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the given QName can be null");
			}
			return null;
    }

    #endregion
  }
}
