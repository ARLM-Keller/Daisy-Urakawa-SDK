using System;
using System.Xml;
using urakawa.media;
using urakawa.xuk;

namespace urakawa.properties.channel
{
	/// <summary>
	/// A <see cref="Channel"/> is used to associate <see cref="media.IMedia"/> 
	/// with <see cref="core.TreeNode"/>s via <see cref="ChannelsProperty"/>
	/// </summary>
	public class Channel : IXukAble, IValueEquatable<Channel>
	{
		private string mName = "";
		private ChannelsManager mChannelsManager;

		internal Channel(ChannelsManager chMgr)
		{
			mChannelsManager = chMgr;
		}

		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> managing the <see cref="Channel"/>
		/// </summary>
		/// <returns>The <see cref="ChannelsManager"/></returns>
		public ChannelsManager getChannelsManager()
		{
			return mChannelsManager;
		}

		/// <summary>
		/// Sets the human-readable name of the <see cref="Channel"/>
		/// </summary>
		/// <param name="name">The new human-readable name</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="name"/> is null
		/// </exception>
		public void setName(string name)
		{
			if (mName==null) 
			{
				throw new exception.MethodParameterIsNullException(
					"Can not set channel localName to null");
			}
			mName = name;
		}

		/// <summary>
		/// Gets the human-readable name of the <see cref="Channel"/>
		/// </summary>
		/// <returns>The human-readable name</returns>
		public string getName()
		{
			return mName;
		}

		/// <summary>
		/// Checks of a given <see cref="IMedia"/> is accepted by the channel
		/// </summary>
		/// <param name="m">The <see cref="IMedia"/></param>
		/// <returns>
		/// A <see cref="bool"/> indicating if the <see cref="IMedia"/> is accpetable
		/// </returns>
		public virtual bool canAccept(IMedia m)
		{
			return true;
		}

		/// <summary>
		/// Gets the uid of the <see cref="Channel"/>
		/// </summary>
		/// <returns>The Xuk Uid as calculated by 
		/// <c>this.getChannelsManager.getUidOfChannel(this)</c></returns>
		public string getUid()
		{
			return getChannelsManager().getUidOfChannel(this);
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Channel"/> from a Channel xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read Channel from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				string name = "";
				if (!source.IsEmptyElement) name = source.ReadString();
				setName(name);

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of Channel: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Channel xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			// No known attributes
		}

		/// <summary>
		/// Write a Channel element to a XUK file representing the <see cref="Channel"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}
			destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
			XukOutAttributes(destination);
			destination.WriteString(getName());
			destination.WriteEndElement();
		}

		/// <summary>
		/// Writes the attributes of a Channel element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Channel"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<Channel> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public virtual bool ValueEquals(Channel other)
		{
			if (other == null) return false;
			if (GetType() != other.GetType()) return false;
			if (getName() != other.getName()) return false;
			return true;
		}

		#endregion
	}
}
