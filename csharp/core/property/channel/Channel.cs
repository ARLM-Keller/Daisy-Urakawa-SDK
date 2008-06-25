using System;
using System.Xml;
using urakawa.media;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.property.channel
{
	/// <summary>
	/// A <see cref="Channel"/> is used to associate <see cref="media.IMedia"/> 
	/// with <see cref="core.TreeNode"/>s via <see cref="ChannelsProperty"/>
	/// </summary>
	public class Channel : XukAble, IValueEquatable<Channel>
	{
		private string mName = "";
		private string mLanguage = null;
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
		/// Determines if the channel is equivalent to a given other channel, 
		/// possibly from another <see cref="Presentation"/>
		/// </summary>
		/// <param name="otherChannel">The given other channel</param>
		/// <returns>A <see cref="bool"/> indicating equivalence</returns>
		public virtual bool isEquivalentTo(Channel otherChannel)
		{
			if (otherChannel == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not test for equivalence with a null Channel");
			}
			if (this.GetType() != otherChannel.GetType()) return false;
			if (this.getName() != otherChannel.getName()) return false;
			if (this.getLanguage() != otherChannel.getLanguage()) return false;
			return true;
		}

		/// <summary>
		/// Exports the channel to a destination <see cref="Presentation"/>.
		/// The exported channels has the same name
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported channel</returns>
		public Channel export(Presentation destPres)
		{
			return exportProtected(destPres);
		}

		/// <summary>
		/// Exports the channel to a destination <see cref="Presentation"/>.
		/// The exported channels has the same name.
		/// (protected virtual method, called by public <see cref="export"/> method)
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported channel</returns>
		/// <remarks>
		/// In derived classes, this method should be overridden. 
		/// If one wants the copy method to return the correct sub-type,
		/// override <see cref="export"/> with the <c>new</c> keyword, making it return <see cref="exportProtected"/>
		/// </remarks>
		protected virtual Channel exportProtected(Presentation destPres)
		{
			Channel exportedCh = destPres.getChannelFactory().createChannel(
				getXukLocalName(), getXukNamespaceUri());
			if (exportedCh == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The ChannelsFacotry of the destination Presentation can not create a Channel matching Xuk QName {0}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exportedCh.setName(getName());
			exportedCh.setLanguage(getLanguage());
			return exportedCh;
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
			if (name==null) 
			{
				throw new exception.MethodParameterIsNullException(
					"Can not set channel name to null");
			}
			mName = name;
		}

		/// <summary>
		/// Gets the language of the channel
		/// </summary>
		/// <param name="lang"></param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"Can not set channel language to an empty string");
			}
			mLanguage = lang;
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
		/// Gets the language of the channel
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
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

        ///// <summary>
        ///// Reads the <see cref="Channel"/> from a Channel xuk element
        ///// </summary>
        ///// <param name="source">The source <see cref="XmlReader"/></param>
        ///// <param name="handler">The handler for progress</param>
        //public void xukIn(XmlReader source, ProgressHandler handler)
        //{
        //    if (source == null)
        //    {
        //        throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
        //    }
        //    if (source.NodeType != XmlNodeType.Element)
        //    {
        //        throw new exception.XukException("Can not read Channel from a non-element node");
        //    }
        //    try
        //    {
        //        xukInAttributes(source);
        //        if (!source.IsEmptyElement)
        //        {
        //            while (source.Read())
        //            {
        //                if (source.NodeType == XmlNodeType.Element)
        //                {
        //                    xukInChild(source);
        //                }
        //                else if (source.NodeType == XmlNodeType.EndElement)
        //                {
        //                    break;
        //                }
        //                if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
        //            }
        //        }

        //    }
        //    catch (exception.XukException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new exception.XukException(
        //            String.Format("An exception occured during xukIn of Channel: {0}", e.Message),
        //            e);
        //    }
        //}

		/// <summary>
		/// Reads the attributes of a Channel xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			string name = source.GetAttribute("name");
			if (name == null) name = "";
			setName(name);
			string lang = source.GetAttribute("language");
			if (lang != null) lang = lang.Trim();
			if (lang == "") lang = null;
			setLanguage(lang);
            base.xukInAttributes(source);
		}

        ///// <summary>
        ///// Write a Channel element to a XUK file representing the <see cref="Channel"/> instance
        ///// </summary>
        ///// <param name="destination">The destination <see cref="XmlWriter"/></param>
        ///// <param name="baseUri">
        ///// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        ///// if <c>null</c> absolute <see cref="Uri"/>s are written
        ///// </param>
        //public void xukOut(XmlWriter destination, Uri baseUri)
        //{
        //    if (destination == null)
        //    {
        //        throw new exception.MethodParameterIsNullException(
        //            "Can not xukOut to a null XmlWriter");
        //    }

        //    try
        //    {
        //        destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
        //        xukOutAttributes(destination, baseUri);
        //        xukOutChildren(destination, baseUri);
        //        destination.WriteEndElement();

        //    }
        //    catch (exception.XukException e)
        //    {
        //        throw e;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new exception.XukException(
        //            String.Format("An exception occured during xukOut of Channel: {0}", e.Message),
        //            e);
        //    }
        //}


		/// <summary>
		/// Writes the attributes of a Channel element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("name", getName());
			destination.WriteAttributeString("language", getLanguage());
            base.xukOutAttributes(destination, baseUri);
		}

		#endregion

		#region IValueEquatable<Channel> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public virtual bool valueEquals(Channel other)
		{
			if (other == null) return false;
			if (GetType() != other.GetType()) return false;
			if (getName() != other.getName()) return false;
			if (getLanguage() != other.getLanguage()) return false;
			return true;
		}

		#endregion
	}
}