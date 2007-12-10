using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.xuk;

namespace urakawa.media.data
{
	/// <summary>
	/// Default implementation of a <see cref="MediaDataManager"/>
	/// </summary>
	public class MediaDataManager : WithPresentation, IXukAble, IValueEquatable<MediaDataManager>
	{
		private const string DEFAULT_UID_PREFIX = "UID";

		private Dictionary<string, MediaData> mMediaDataDictionary = new Dictionary<string, MediaData>();
		private Dictionary<MediaData, string> mReverseLookupMediaDataDictionary = new Dictionary<MediaData, string>();
		private System.Threading.Mutex mUidMutex = new System.Threading.Mutex();
		private ulong mUidNo = 0;
		private string mUidPrefix = DEFAULT_UID_PREFIX;
		private audio.PCMFormatInfo mDefaultPCMFormat;
		private bool mEnforceSinglePCMFormat;

		/// <summary>
		/// Default constructor
		/// </summary>
		public MediaDataManager()
		{
			mDefaultPCMFormat = new audio.PCMFormatInfo();
			mDefaultPCMFormat.FormatChanged += new EventHandler(DefaultPCMFormat_FormatChanged);
			mEnforceSinglePCMFormat = false;
		}

		void DefaultPCMFormat_FormatChanged(object sender, EventArgs e)
		{
			if (getEnforceSinglePCMFormat())
			{
				if (CheckSinglePCMFormatRule())
				{
					throw new exception.InvalidDataFormatException(
						"Can not change the default PCM format for the MediaDataManager, "
						+ "since the manager is enforcing single PCM format and the change will violate this rule");
				}
			}
		}

		private bool CheckSinglePCMFormatRule()
		{
			foreach (MediaData md in getListOfMediaData())
			{
				if (md is audio.AudioMediaData)
				{
					audio.AudioMediaData amd = (audio.AudioMediaData)md;
					if (!amd.getPCMFormat().ValueEquals(getDefaultPCMFormat())) return false;
				}
			}
			return true;
		}


		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> associated with <c>this</c> 
		/// </summary>
		/// <returns>The <see cref="MediaDataFactory"/></returns>
		public MediaDataFactory getMediaDataFactory()
		{
			return getPresentation().getMediaDataFactory();
		}


		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> associated with <c>this</c> 
		/// (via. the <see cref="IMediaDataPresentation"/> associated with <c>this</c>).
		/// Convenience for <c>getDataProviderManager().getDataProviderFactory()</c>
		/// </summary>
		/// <returns>The <see cref="IDataProviderFactory"/></returns>
		public IDataProviderFactory getDataProviderFactory()
		{
			return getPresentation().getDataProviderManager().getDataProviderFactory();
		}

		/// <summary>
		/// Gets the default <see cref="audio.PCMFormatInfo"/> for <see cref="audio.AudioMediaData"/> managed by the manager
		/// </summary>
		/// <returns>The default PCM format</returns>
		public audio.PCMFormatInfo getDefaultPCMFormat()
		{
			return mDefaultPCMFormat;
		}

		/// <summary>
		/// Gets a <see cref="bool"/> indicating if a single 
		/// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
		/// </summary>
		/// <returns>The <see cref="bool"/></returns>
		public bool getEnforceSinglePCMFormat()
		{
			return mEnforceSinglePCMFormat;
		}

		/// <summary>
		/// Sets a <see cref="bool"/> indicating if a single 
		/// PCMFormat is enforced for all managed <see cref="audio.AudioMediaData"/>
		/// </summary>
		/// <param name="newValue">The new value</param>
		public void setEnforceSinglePCMFormat(bool newValue)
		{
			mEnforceSinglePCMFormat = newValue;
			if (getEnforceSinglePCMFormat())
			{
				if (!CheckSinglePCMFormatRule())
				{
					throw new exception.InvalidDataFormatException(
						"Can not enforce single PCM formats since AudioMediaData with PCM format different "
						+ "from the default for the manager exists in the manager");
				}
			}
		}


		/// <summary>
		/// Gets the <see cref="MediaData"/> with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The <see cref="MediaData"/> with the given UID 
		/// or <c>null</c> if no such <see cref="MediaData"/> exists</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		public MediaData getMediaData(string uid)
		{
			if (uid == null)
			{
				throw new exception.MethodParameterIsNullException("The UID must not be null");
			}
			if (mMediaDataDictionary.ContainsKey(uid))
			{
				return mMediaDataDictionary[uid];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the UID of a given <see cref="MediaData"/>
		/// </summary>
		/// <param name="data">The given <see cref="MediaData"/></param>
		/// <returns>The UID of <see cref="MediaData"/> <paramref name="data"/></returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <c>this</c> is not the manager of <paramref name="data"/>
		/// </exception>
		public string getUidOfMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not get the UID of a null MediaData");
			}
			if (!mReverseLookupMediaDataDictionary.ContainsKey(data))
			{
				throw new exception.IsNotManagerOfException("The given MediaData is not managed by this MediaDataManager");
			}
			return mReverseLookupMediaDataDictionary[data];
		}

		private string getNewUid()
		{
			while (true)
			{
				if (mUidNo < UInt64.MaxValue)
				{
					mUidNo++;
				}
				else
				{
					mUidPrefix += "X";
				}
				string key = String.Format("{0}{1:00000000}", mUidPrefix, mUidNo);
				if (!mMediaDataDictionary.ContainsKey(key))
				{
					return key;
				}
			}
		}

		/// <summary>
		/// Adds a <see cref="MediaData"/> to the <see cref="MediaDataManager"/>
		/// </summary>
		/// <param name="data">The <see cref="MediaData"/> to add</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		public void addMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not add null MediaData to the manager");
			}
			mUidMutex.WaitOne();
			try
			{
				string uid = getNewUid();
				addMediaData(data, uid);
			}
			finally
			{
				mUidMutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Adds a <see cref="MediaData"/> to the <see cref="MediaDataManager"/>, assigning it a given uid
		/// </summary>
		/// <param name="data">The <see cref="MediaData"/> to add</param>
		/// <param name="uid">The uid to assign to the added <see cref="MediaData"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsAlreadyManagerOfException">
		/// Thrown when another <see cref="MediaData"/> has the same uid</exception>
		public void addMediaData(MediaData data, string uid)
		{
			if (mMediaDataDictionary.ContainsKey(uid))
			{
				throw new exception.IsAlreadyManagerOfException(String.Format(
					"There is already another MediaData with uid {0}", uid));
			}
			if (getEnforceSinglePCMFormat())
			{
				if (data is audio.AudioMediaData)
				{
					audio.AudioMediaData amdata = (audio.AudioMediaData)data;
					if (!amdata.getPCMFormat().ValueEquals(getDefaultPCMFormat()))
					{
						throw new exception.InvalidDataFormatException(
							"The AudioMediaData being added has a PCM format that is in-compatible with the manager (breaks enforcing of single PCM format)");
					}
				}
			}
			mMediaDataDictionary.Add(uid, data);
			mReverseLookupMediaDataDictionary.Add(data, uid);
		}

		/// <summary>
		/// Determines if the manager manages a <see cref="MediaData"/> with a given uid
		/// </summary>
		/// <param name="uid">The given uid</param>
		/// <returns>
		/// A <see cref="bool"/> indicating if the manager manages a <see cref="MediaData"/> with the given uid
		/// </returns>
		public bool isManagerOf(string uid)
		{
			return mMediaDataDictionary.ContainsKey(uid);
		}

		/// <summary>
		/// Removes a <see cref="MediaData"/> from <c>this</c>
		/// </summary>
		/// <param name="data">The <see cref="MediaData"/> to remove</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <paramref name="data"/> is not managed by <c>this</c>
		/// </exception>
		public void removeMediaData(MediaData data)
		{
			removeMediaData(getUidOfMediaData(data));
		}

		/// <summary>
		/// Removes a <see cref="MediaData"/> from <c>this</c>
		/// </summary>
		/// <param name="uid">The uid of the <see cref="MediaData"/> to remove</param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="uid"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when no managed <see cref="MediaData"/> has the given uid
		/// </exception>
		public void removeMediaData(string uid)
		{
			MediaData data = getMediaData(uid);
			mUidMutex.WaitOne();
			try
			{
				mMediaDataDictionary.Remove(uid);
				mReverseLookupMediaDataDictionary.Remove(data);
			}
			finally
			{
				mUidMutex.ReleaseMutex();
			}
		}

		/// <summary>
		/// Creates a copy of a given media data
		/// </summary>
		/// <param name="data">The media data to copy</param>
		/// <returns>The copy</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="data"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <paramref name="data"/> is not managed by <c>this</c>
		/// </exception>
		public MediaData copyMediaData(MediaData data)
		{
			if (data == null)
			{
				throw new exception.MethodParameterIsNullException("Can not copy a null MediaData");
			}
			if (data.getMediaDataManager() != this)
			{
				throw new exception.IsNotManagerOfException(
					"Can not copy a MediaData that is not managed by this");
			}
			return data.copy();
		}

		/// <summary>
		/// Creates a copy of the media data with a given UID
		/// </summary>
		/// <param name="uid">The given UID</param>
		/// <returns>The copy</returns>
		/// <exception cref="exception.IsNotManagerOfException">
		/// Thrown when <c>this</c> does not manage a media data with the given UID
		/// </exception>
		public MediaData copyMediaData(string uid)
		{
			MediaData data = getMediaData(uid);
			if (data == null)
			{
				throw new exception.IsNotManagerOfException(String.Format(
					"The media data manager does not manage a media data with UID {0}",
					uid));
			}
			return copyMediaData(data);
		}

		/// <summary>
		/// Gets a list of all <see cref="MediaData"/> managed by <c>this</c>
		/// </summary>
		/// <returns>The list</returns>
		public List<MediaData> getListOfMediaData()
		{
			return new List<MediaData>(mMediaDataDictionary.Values);
		}

		/// <summary>
		/// Gets a list of the uids assigned to <see cref="MediaData"/> by the manager
		/// </summary>
		/// <returns>The list of uids</returns>
		public List<string> getListOfUids()
		{
			return new List<string>(mMediaDataDictionary.Keys);
		}

		/// <summary>
		/// Deletes any <see cref="MediaData"/> not assiciated with a <see cref="TreeNode"/> via. a <see cref="ChannelsProperty"/>
		/// </summary>
		public void deleteUnusedMediaData()
		{
			data.utilities.CollectManagedMediaTreeNodeVisitor visitor = new data.utilities.CollectManagedMediaTreeNodeVisitor();
			urakawa.core.TreeNode root = getPresentation().getRootNode();
			if (root != null)
			{
				root.acceptDepthFirst(visitor);
			}
			List<MediaData> usedMediaData = new List<MediaData>();
			foreach (IManagedMedia mm in getListOfMediaData())
			{
				if (!usedMediaData.Contains(mm.getMediaData())) usedMediaData.Add(mm.getMediaData());
			}
			foreach (MediaData md in getListOfMediaData())
			{
				if (!usedMediaData.Contains(md)) md.delete();
			}
		}

		#region IXukAble Members

		/// <summary>
		/// Reads the <see cref="MediaDataManager"/> from a MediaDataManager xuk element
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
				throw new exception.XukException("Can not read MediaDataManager from a non-element node");
			}
			try
			{
				mUidMutex.WaitOne();
				try
				{
					mMediaDataDictionary.Clear();
					mReverseLookupMediaDataDictionary.Clear();
				}
				finally
				{
					mUidMutex.ReleaseMutex();
				}
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
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
					String.Format("An exception occured during XukIn of MediaDataManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a MediaDataManager xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
			setEnforceSinglePCMFormat(false);
			string attr = source.GetAttribute("EnforceSinglePCMFormat");
			// Modified by JQ 2007-06-25:
			// if the attribute is not present, default to false
			if (attr != null)
			{
				bool es;
				if (!Boolean.TryParse(attr, out es))
				{
					throw new exception.XukException(String.Format(
						"Attribute EnforceSinglePCMFormat value {0} is not a boolean",
						attr));
				}
				setEnforceSinglePCMFormat(es);
			}
		}

		/// <summary>
		/// Reads a child of a MediaDataManager xuk element. 
		/// More specifically the <see cref="MediaData"/> managed by <c>this</c>
		/// is read from the mMediaData child.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mMediaData":
						XukInMediaData(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();
			}
		}

		private void XukInMediaData(XmlReader source)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == "mMediaDataItem" && source.NamespaceURI == ToolkitSettings.XUK_NS)
						{
							XukInMediaDataItem(source);
						}
						else if (!source.IsEmptyElement)
						{
							source.ReadSubtree().Close();
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}

		private void XukInMediaDataItem(XmlReader source)
		{
			string uid = source.GetAttribute("uid");
			MediaData data = null;
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						data = getMediaDataFactory().createMediaData(source.LocalName, source.NamespaceURI);
						if (data != null)
						{
							data.XukIn(source);
						}
					}
					else if (source.NodeType == XmlNodeType.EndElement)
					{
						break;
					}
					if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
			if (data != null)
			{
				removeMediaData(data);
				if (uid == null && uid == "")
				{
					throw new exception.XukException(
						"uid attribute is missing from mMediaDataItem attribute");
				}
				if (isManagerOf(uid))
				{
					throw new exception.XukException(
						String.Format("Another MediaData exists in the manager with uid {0}", uid));
				}
				addMediaData(data, uid);
			}
		}

		/// <summary>
		/// Write a MediaDataManager element to a XUK file representing the <see cref="MediaDataManager"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="destination"/> is <c>null</c></exception>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException("The destination XmlWriter is null");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of MediaDataManager: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a MediaDataManager element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{
			// Added by JQ 2007-06-25:
			// not writing out the EnforceSinglePCMFormat attribute causes opening the XUK file to fail
			destination.WriteAttributeString("EnforceSinglePCMFormat", getEnforceSinglePCMFormat().ToString());
		}

		/// <summary>
		/// Write the child elements of a MediaDataManager element.
		/// Mode specifically the <see cref="MediaData"/> of <c>this</c> is written to a mMediaData element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{
			destination.WriteStartElement("mMediaData", ToolkitSettings.XUK_NS);
			foreach (string uid in mMediaDataDictionary.Keys)
			{
				destination.WriteStartElement("mMediaDataItem", ToolkitSettings.XUK_NS);
				destination.WriteAttributeString("uid", uid);
				mMediaDataDictionary[uid].XukOut(destination);
				destination.WriteEndElement();
			}
			destination.WriteEndElement();
		}


		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="MediaDataManager"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="MediaDataManager"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<MediaDataManager> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool ValueEquals(MediaDataManager other)
		{
			if (other == null) return false;
			List<MediaData> otherMediaData = other.getListOfMediaData();
			if (mMediaDataDictionary.Count != otherMediaData.Count) return false;
			foreach (MediaData oMD in otherMediaData)
			{
				if (!oMD.ValueEquals(getMediaData(other.getUidOfMediaData(oMD)))) return false;
			}
			return true;
		}

		#endregion

	}
}