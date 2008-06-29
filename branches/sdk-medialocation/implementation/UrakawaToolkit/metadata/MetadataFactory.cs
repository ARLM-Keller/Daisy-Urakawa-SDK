using System;

namespace urakawa.metadata
{  
	/// <summary>
	/// Default <see cref="IMetadata"/> factory - supports creation of <see cref="Metadata"/> instances
	/// </summary>
	public class MetadataFactory : IMetadataFactory
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public MetadataFactory()
		{
    }
    #region IMetadataFactory Members

		/// <summary>
		/// Creates an <see cref="IMetadata"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="IMetadata"/> instance or <c>null</c> if the given QName is not supported</returns>
		public IMetadata createMetadata(string localName, string namespaceUri)
    {
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "Metadata":
						return createMetadata();
				}
			}
			return null;
    }

		IMetadata IMetadataFactory.createMetadata()
		{
			return createMetadata();
		}

    #endregion

		/// <summary>
		/// Creates an <see cref="Metadata"/> instance
		/// </summary>
		/// <returns>The created instance</returns>
		public Metadata createMetadata()
		{
			return new Metadata();
		}
  }
}