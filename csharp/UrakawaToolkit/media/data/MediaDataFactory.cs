using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.xuk;

namespace urakawa.media.data
{
	/// <summary>
	/// <para>Factory for creating <see cref="MediaData"/>.</para>
	/// <para>Supports creation of the following <see cref="MediaData"/> types:
	/// <list type="ul">
	/// <item><see cref="audio.codec.WavAudioMediaData"/></item>
	/// </list>
	/// </para>
	/// </summary>
	public class MediaDataFactory : WithPresentation, IXukAble
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		internal protected MediaDataFactory()
		{
		}

		/// <summary>
		/// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
		/// (via the <see cref="Presentation"/> associated with <c>this</c>.
		/// Convenience for <c>getPresentation().getMediaDataManager()</c>
		/// </summary>
		/// <returns>The <see cref="MediaDataManager"/></returns>
		public MediaDataManager getMediaDataManager()
		{
			return getPresentation().getMediaDataManager();
		}

		/// <summary>
		/// Creates an instance of a <see cref="MediaData"/> of type matching a given XUK QName
		/// </summary>
		/// <param name="xukLocalName">The local name part of the QName</param>
		/// <param name="xukNamespaceUri">The namespace uri part of the QName</param>
		/// <returns>The created <see cref="MediaData"/> instance or <c>null</c> if the given QName is supported</returns>
		public virtual MediaData createMediaData(string xukLocalName, string xukNamespaceUri)
		{
			if (xukLocalName == null || xukNamespaceUri == null)
			{
				throw new exception.MethodParameterIsNullException(
					"No part of the QName can be null");
			}
			if (xukNamespaceUri == ToolkitSettings.XUK_NS)
			{
				switch (xukLocalName)
				{
					case "WavAudioMediaData":
						return createWavAudioMediaData();
					default:
						break;
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a <see cref="MediaData"/> instance of a given <see cref="Type"/>
		/// </summary>
		/// <param name="mt">The given <see cref="Type"/></param>
		/// <returns>
		/// The created <see cref="MediaData"/> instance 
		/// or <c>null</c> if the given media <see cref="Type"/> is not supported
		/// </returns>
		public virtual MediaData createMediaData(Type mt)
		{
			MediaData res = createMediaData(mt.Name, ToolkitSettings.XUK_NS);
			if (typeof(AudioMediaData).IsAssignableFrom(mt))
			{
				return createWavAudioMediaData();
			}
			if (res.GetType()==mt) return res;
			return null;
		}

		/// <summary>
		/// Creates a <see cref="AudioMediaData"/> of default type (which is <see cref="WavAudioMediaData"/>)
		/// </summary>
		/// <returns>The created <see cref="WavAudioMediaData"/></returns>
		public virtual AudioMediaData createAudioMediaData()
		{
			return createMediaData(typeof(WavAudioMediaData).Name, ToolkitSettings.XUK_NS) as WavAudioMediaData;
		}

		/// <summary>
		/// Creates a <see cref="WavAudioMediaData"/>
		/// </summary>
		/// <returns>The created <see cref="WavAudioMediaData"/></returns>
		public WavAudioMediaData createWavAudioMediaData()
		{
			return new WavAudioMediaData(getMediaDataManager()); 
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="MediaDataFactory"/> from a MediaDataFactory xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void xukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read MediaDataFactory from a non-element node");
			}
			try
			{
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukIn of MediaDataFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a MediaDataFactory xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a MediaDataFactory xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a MediaDataFactory element to a XUK file representing the <see cref="MediaDataFactory"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void xukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not xukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukOut of MediaDataFactory: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a MediaDataFactory element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a MediaDataFactory element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaDataFactory"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaDataFactory"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}
