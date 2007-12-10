using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace urakawa.media.data
{
	/// <summary>
	/// Implementation of <see cref="IDataProviderFactory"/> that supports the creation of <see cref="FileDataProvider"/>s
	/// </summary>
	public class FileDataProviderFactory : IDataProviderFactory
	{
		#region IDataProviderFactory Members

		IDataProviderManager IDataProviderFactory.getDataProviderManager()
		{
			return getDataProviderManager();
		}

		private FileDataProviderManager mDataProviderManager;

		/// <summary>
		/// Gets the <see cref="FileDataProviderManager"/> that owns the factory 
		/// and manages the data providers created by the factory
		/// </summary>
		/// <returns>The manager</returns>
		public FileDataProviderManager getDataProviderManager()
		{
			if (mDataProviderManager == null)
			{
				throw new exception.IsNotInitializedException(
					"The FileDataProviderFactory has not been initialized with an owning FileDataProviderManager");
			}
			return mDataProviderManager;
		}

		/// <summary>
		/// Initializer assicoating a owning <see cref="FileDataProviderManager"/> with the factory
		/// </summary>
		/// <param name="mngr">The owning manager</param>
		public void setDataProviderManager(IDataProviderManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException(
					"A FiledataProvider factory can not be owned by a null FileDataProviderManager");
			}
			if (!(mngr is FileDataProviderManager))
			{
				throw new exception.MethodParameterIsWrongTypeException(
					"A FiledataProvider factory must be owned by a FileDataProviderManager");
			}
			if (mDataProviderManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The FileDataProviderFactory has already been initialized with a owning FileDataProviderManager");
			}
			mDataProviderManager = (FileDataProviderManager)mngr;
		}

		/// <summary>
		/// MIME type for MPEG-4 AAC audio
		/// </summary>
		public const string AUDIO_MP4_MIME_TYPE = "audio/mpeg-generic";
		/// <summary>
		/// MIME type for MPEG-1/2 Layer III audio (MP3)
		/// </summary>
		public const string AUDIO_MP3_MIME_TYPE = "audio/mpeg";
		/// <summary>
		/// MIME type for linear PCM RIFF WAVE format audio (wav)
		/// </summary>
		public const string AUDIO_WAV_MIME_TYPE = "audio/x-wav";
		/// <summary>
		/// MIME type for JPEG images
		/// </summary>
		public const string IMAGE_JPG_MIME_TYPE = "image/jpeg";
		/// <summary>
		/// MIME type for PNG images
		/// </summary>
		public const string IMAGE_PNG_MIME_TYPE =  "image/png";
		/// <summary>
		/// MIME type for Scalable Vector Graphics (SVG) images
		/// </summary>
		public const string IMAGE_SVG_MIME_TYPE =  "image/svg+xml";
		/// <summary>
		/// MIME type for Cascading Style Sheets (CSS)
		/// </summary>
		public const string STYLE_CSS_MIME_TYPE =  "text/css";
		/// <summary>
		/// MIME type for plain text
		/// </summary>
		public const string TEXT_PLAIN_MIME_TYPE = "text/plain";


		/// <summary>
		/// Gets the file extension for a given MIME type
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns>The extension</returns>
		public static string getExtensionFromMimeType(string mimeType)
		{
			string extension;
			switch (mimeType)
			{
				case AUDIO_MP4_MIME_TYPE:
					extension = ".mp4";
					break;
				case AUDIO_MP3_MIME_TYPE:
					extension = ".mp3";
					break;
				case AUDIO_WAV_MIME_TYPE:
					extension = ".wav";
					break;
				case IMAGE_JPG_MIME_TYPE:
					extension = ".jpg";
					break;
				case IMAGE_PNG_MIME_TYPE:
					extension = ".png";
					break;
				case IMAGE_SVG_MIME_TYPE:
					extension = ".svg";
					break;
				case STYLE_CSS_MIME_TYPE:
					extension = ".css";
					break;
				case TEXT_PLAIN_MIME_TYPE:
					extension = ".txt";
					break;
				default:
					extension = ".bin";
					break;
			}
			return extension;
		}

		System.Threading.Mutex mCreateFileDataProviderMutex = new System.Threading.Mutex();

		/// <summary>
		/// Creates a <see cref="FileDataProvider"/> for the given MIME type
		/// </summary>
		/// <param name="mimeType">The given MIME type</param>
		/// <returns>The created data provider</returns>
		public IDataProvider createDataProvider(string mimeType)
		{
			if (mimeType == null)
			{
				throw new exception.MethodParameterIsNullException("Can not create a FileDataProvider for a null MIME type");
			}
			mCreateFileDataProviderMutex.WaitOne();
			FileDataProvider newProv;
			try
			{
				newProv = new FileDataProvider(
					getDataProviderManager(),
					getDataProviderManager().getNewDataFileRelPath(getExtensionFromMimeType(mimeType)),
					mimeType);
				getDataProviderManager().addDataProvider(newProv);
			}
			finally
			{
				mCreateFileDataProviderMutex.ReleaseMutex();
			}
			return newProv;
		}

		/// <summary>
		/// Creates a data provider for the given mime type of type mathcing the given xuk QName
		/// </summary>
		/// <param name="mimeType">The given MIME type</param>
		/// <param name="xukLocalName">The local name part of the given xuk QName</param>
		/// <param name="xukNamespaceUri">The namespace uri part of the given xuk QName</param>
		/// <returns>The created data provider</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="xukLocalName"/> or <paramref name="xukNamespaceUri"/> is <c>null</c></exception>
		public IDataProvider createDataProvider(string mimeType, string xukLocalName, string xukNamespaceUri)
		{
			if (xukLocalName == null || xukNamespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException("No part of the xuk QName can be null");
			}
			if (xukNamespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (xukLocalName)
				{
					case "FileDataProvider":
						return createDataProvider(mimeType);
				}
			}
			return null;
		}

		#endregion
	}
}