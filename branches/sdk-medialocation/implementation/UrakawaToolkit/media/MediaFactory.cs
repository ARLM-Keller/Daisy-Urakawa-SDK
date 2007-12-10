using System;

namespace urakawa.media
{
	/// <summary>
	/// The media factory will create any media object of MediaType.xxx
	/// </summary>
	public class MediaFactory : IMediaFactory
	{
		private IMediaPresentation mPresentation = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MediaFactory()
		{
		}
		#region IMediaFactory Members

		/// <summary>
		/// Create a media object of the given type.
		/// </summary>
		/// <param name="type">The type of media object to create.</param>
		/// <returns>a new MediaObject of a specific type.</returns>
		public virtual IMedia createMedia(urakawa.media.MediaType type)
		{
			if (type == MediaType.AUDIO)
			{
				return createMedia("AudioMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.IMAGE)
			{
				return createMedia("ImageMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.TEXT)
			{
				return createMedia("TextMedia", ToolkitSettings.XUK_NS);
			}

			else if (type == MediaType.VIDEO)
			{
				return createMedia("VideoMedia", ToolkitSettings.XUK_NS);
			}
			else if (type == MediaType.EMPTY_SEQUENCE)
			{
				return createMedia("SequenceMedia", ToolkitSettings.XUK_NS);
			}

			else
			{
				throw new exception.MediaTypeIsIllegalException("MediaFactory.createMedia(" +
					type.ToString() + ") caused MediaTypeIsIllegalException");
			}
		}

		/// <summary>
		/// Creates an <see cref="IMedia"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>The creates <see cref="IMedia"/> or <c>null</c> is the given QName is not supported</returns>
		public IMedia createMedia(string localName, string namespaceUri)
		{
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "AudioMedia":
						return new AudioMedia(this, null);
					case "ExternalAudioMedia":
						return new ExternalAudioMedia(this);
					case "ImageMedia":
						return new ImageMedia(this);
					case "VideoMedia":
						return new VideoMedia(this);
					case "TextMedia":
						return new TextMedia(this);
					case "SequenceMedia":
						return new SequenceMedia(this);

				}
			}
			return null;
		}

		IMediaLocation IMediaFactory.createMediaLocation()
		{
			return createMediaLocation();
		}

		/// <summary>
		/// Creates a <see cref="SrcMediaLocation"/>
		/// </summary>
		/// <returns>The created <see cref="SrcMediaLocation"/></returns>
		public SrcMediaLocation createMediaLocation()
		{
			return new SrcMediaLocation(this);
		}

		/// <summary>
		/// Creates a <see cref="IMediaLocation"/> matching a given QName
		/// </summary>
		/// <param name="localName">The local localName part of the QName</param>
		/// <param name="namespaceUri">The namespace uri part of the QName</param>
		/// <returns>
		/// The created <see cref="IMediaLocation"/> 
		/// or <c>null</c> if the QName is not recognized
		/// </returns>
		/// <remarks>
		/// <see cref="MediaFactory"/> recognizes only 
		/// the QName <c><see cref="urakawa.ToolkitSettings.XUK_NS"/>:<see cref="SrcMediaLocation"/></c> 
		/// </remarks>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when one of the QName parts is <c>null</c>
		/// </exception>
		public IMediaLocation createMediaLocation(string localName, string namespaceUri)
		{
			if (localName == null || namespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the QName can be null");
			}
			if (namespaceUri == urakawa.ToolkitSettings.XUK_NS)
			{
				switch (localName)
				{
					case "SrcMediaLocation":
						return createMediaLocation();
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the <see cref="IMediaPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The associated <see cref="IMediaPresentation"/></returns>
		public IMediaPresentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(
					"No media presentation has yet been associated with the media factory");
			}
			return mPresentation;
		}

		/// <summary>
		/// Sets the <see cref="IMediaPresentation"/> associated with <c>this</c>
		/// </summary>
		/// <param name="pres">The associated <see cref="IMediaPresentation"/></param>
		public void setPresentation(IMediaPresentation pres)
		{
			if (pres==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The media factory can not be associated with a null media presentation");
			}
			if (mPresentation != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The media presentation has already been associated with a meida presentation");
			}
			mPresentation = pres;
		}

		#endregion
	}
}