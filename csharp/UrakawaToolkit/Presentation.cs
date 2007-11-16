using System;
using System.Xml;
using System.Collections.Generic;
using urakawa.core;
using urakawa.core.events;
using urakawa.property;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.media;
using urakawa.media.data;
using urakawa.metadata;
using urakawa.undo;
using urakawa.xuk;

namespace urakawa
{
	/// <summary>
	/// The primary container for a document tree consisting of <see cref="TreeNode"/>s,
	/// includes factories and managers for:
	/// <list type="bullet">
	/// <item><see cref="Property"/>s</item>
	/// <item><see cref="Channel"/>s</item>
	/// <item><see cref="IMedia"/></item>
	/// <item><see cref="MediaData"/></item>
	/// <item><see cref="IDataProvider"/>s</item>
	/// <item><see cref="Metadata"/></item>
	/// </list>
	/// </summary>
	public class Presentation : ITreePresentation, IMediaDataPresentation, IChannelPresentation, IXmlPresentation, IValueEquatable<Presentation>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		internal protected Presentation()
		{
			mMetadata = new List<Metadata>();
			mRootNodeInitialized = false;
		}

		#region Old Presentation construction
		///// <summary>
		///// Constructor - initializes the presentation with a given base <see cref="Uri"/> and default factories and managers.
		///// The constructed has an empty <see cref="TreeNode"/> as root
		///// </summary>
		///// <param name="rootUri">The given root uri</param>
		//public Presentation(Uri rootUri) 
		//  : this(rootUri, null, null, null, null, null, null, null, null, null, null, null, null)
		//{
		//}

		///// <summary>
		///// Constructor setting given factories and managers.
		///// The constructed has an empty <see cref="TreeNode"/> as root
		///// </summary>
		///// <param name="rootUri">The base uri of the presentation</param>
		///// <param name="treeNodeFact">
		///// The core node factory of the presentation -
		///// if <c>null</c> a newly created <see cref="TreeNodeFactory"/> is used
		///// </param>
		///// <param name="propFact">
		///// The property factory of the presentation -
		///// if <c>null</c> a newly created <see cref="PropertyFactory"/> is used
		///// </param>
		///// <param name="chFact">
		///// The channel factory of the presentation -
		///// if <c>null</c> a newly created <see cref="ChannelFactory"/> is used
		///// </param>
		///// <param name="chMgr">
		///// The channels manager> of the presentation -
		///// if <c>null</c> a newly created <see cref="ChannelsManager"/> is used
		///// </param>
		///// <param name="mediaFact">
		///// The media factory of the presentation -
		///// if <c>null</c> a newly created <see cref="MediaFactory"/> is used
		///// </param>
		///// <param name="mediaDataMngr">
		///// The media data manager of the presentation -
		///// if <c>null</c> a newly created <see cref="MediaDataManager"/> is used
		/////	</param>
		///// <param name="mediaDataFact">
		///// The media data facotry of the presentation - 
		///// if <c>null</c> a newly created <see cref="MediaDataFactory"/> is used
		///// </param>
		/////	<param name="dataProvMngr">
		/////	The data provider manager of the presentation - 
		/////	if <c>null</c> a newly created <see cref="FileDataProviderManager"/> is used
		///// </param>
		///// <param name="dataProvFact">
		///// The data provider factory of the presentation - 
		///// if <c>null</c> a newly created <see cref="FileDataProviderFactory"/> is used</param>
		/////	<param name="undoRedoMngr">
		/////	The undo/redo manager of the presentation - 
		/////	if <c>null</c> a newly created <see cref="UndoRedoManager"/> is used
		///// </param>
		/////	<param name="cmdFact">
		/////	The command factory of the presentation - 
		/////	if <c>null</c> a newly created <see cref="CommandFactory"/> is used
		///// </param>
		///// <param name="metaFact">
		///// The <see cref="Metadata"/> factory of the presentation
		///// </param>
		//public Presentation(
		//  Uri rootUri,
		//  TreeNodeFactory treeNodeFact, PropertyFactory propFact, 
		//  ChannelFactory chFact, ChannelsManager chMgr, IMediaFactory mediaFact,
		//  MediaDataManager mediaDataMngr, MediaDataFactory mediaDataFact, 
		//  IDataProviderManager dataProvMngr, IDataProviderFactory dataProvFact,
		//  UndoRedoManager undoRedoMngr, CommandFactory cmdFact,
		//  metadata.MetadataFactory metaFact
		//  )
		//{
		//  setRootUri(rootUri);
		//  //Replace nulls with defaults
		//  if (treeNodeFact == null) treeNodeFact = new TreeNodeFactory();
		//  if (propFact == null) propFact = new PropertyFactory();
		//  if (chFact == null) chFact = new ChannelFactory();
		//  if (chMgr == null) chMgr = new ChannelsManager();
		//  if (mediaFact == null) mediaFact = new MediaFactory();
		//  if (mediaDataMngr == null) mediaDataMngr = new MediaDataManager();
		//  if (mediaDataFact == null) mediaDataFact = new MediaDataFactory();
		//  if (dataProvMngr == null) dataProvMngr = new FileDataProviderManager("Data");
		//  if (dataProvFact == null) dataProvFact = new FileDataProviderFactory();
		//  if (undoRedoMngr == null) undoRedoMngr = new UndoRedoManager();
		//  if (cmdFact == null) cmdFact = new CommandFactory();
		//  if (metaFact == null) metaFact = new urakawa.metadata.MetadataFactory();


		//  //Setup member vars
		//  mTreeNodeFactory = treeNodeFact;
		//  mPropertyFactory = propFact;
		//  mChannelFactory = chFact;
		//  mChanelsManager = chMgr;
		//  mMediaFactory = mediaFact;
		//  mMediaDataManager = mediaDataMngr;
		//  mMediaDataFactory = mediaDataFact;
		//  mDataProviderManager = dataProvMngr;
		//  mDataProviderFactory = dataProvFact;
		//  mUndoRedoManager = undoRedoMngr;
		//  mCommandFactory = cmdFact;
		//  mMetadataFactory = metaFact;
		//  mMetadata = new List<Metadata>();

		//  //Linkup members to this
		//  treeNodeFact.setPresentation(this);
		//  mChannelFactory.setPresentation(this);
		//  mChanelsManager.setPresentation(this);
		//  propFact.setPresentation(this);
		//  mMediaFactory.setPresentation(this);
		//  mMediaDataManager.setPresentation(this);
		//  mMediaDataFactory.setPresentation(this);
		//  mDataProviderManager.setPresentation(this);
		//  mDataProviderFactory.setPresentation(this);
		//  mUndoRedoManager.setPresentation(this);
		//  mCommandFactory.setPresentation(this);
		//  mMetadataFactory.setPresentation(this);

		//  setRootNode(getTreeNodeFactory().createNode());

		//  mLanguage = null;
		//}

		#endregion

		private Project mProject;
		private TreeNodeFactory mTreeNodeFactory;
		private PropertyFactory mPropertyFactory;
		private ChannelFactory mChannelFactory;
		private ChannelsManager mChannelsManager;
		private IMediaFactory mMediaFactory;
		private MediaDataManager mMediaDataManager;
		private MediaDataFactory mMediaDataFactory;
		private IDataProviderManager mDataProviderManager;
		private IDataProviderFactory mDataProviderFactory;
		private undo.UndoRedoManager mUndoRedoManager;
		private undo.CommandFactory mCommandFactory;
		private TreeNode mRootNode;
		private bool mRootNodeInitialized;
		private Uri mRootUri;
		private string mLanguage;

		
		/// <summary>
		/// Gets the <see cref="Project"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public Project getProject()
		{
			if (mProject == null)
			{
				throw new exception.IsNotInitializedException(
					"The Presentation has not been initialized with an owning Project");
			}
			return mProject;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with an owning <see cref="Project"/>
		/// </summary>
		/// <param name="proj">The new <see cref="Project"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="proj"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="Project"/>
		/// </exception>
		public void setProject(Project proj)
		{
			if (proj == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The Project can not be null");
			}
			if (mProject != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with an owning Project");
			}
			mProject = proj;
		}

		/// <summary>
		/// Gets the <see cref="DataModelFactory"/> associated with the <see cref="Presentation"/>
		/// via. it's owning <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="DataModelFactory"/></returns>
		public DataModelFactory getDataModelFactory()
		{
			return getProject().getDataModelFactory();
		}

		/// <summary>
		/// Sets the language of the presentation
		/// </summary>
		/// <param name="lang">The new language, can be null but not an empty string</param>
		public void setLanguage(string lang)
		{
			if (lang == "")
			{
				throw new exception.MethodParameterIsEmptyStringException(
					"The language can not be an empty string");
			}
			mLanguage = lang;
		}

		/// <summary>
		/// Gets the language of the presentation
		/// </summary>
		/// <returns>The language</returns>
		public string getLanguage()
		{
			return mLanguage;
		}

		/// <summary>
		/// Removes any <see cref="MediaData"/> and <see cref="IDataProvider"/>s that are not used by any <see cref="TreeNode"/> in the document tree
		/// or by any <see cref="ICommand"/> in the <see cref="UndoRedoManager"/> stacks (undo/redo/transaction).
		/// </summary>
		public void cleanup()
		{
			urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor collectorVisitor
				= new urakawa.media.data.utilities.CollectManagedMediaTreeNodeVisitor();
			if (getRootNode() != null)
			{
				getRootNode().acceptDepthFirst(collectorVisitor);
			}
			List<MediaData> usedMediaData = getUndoRedoManager().getListOfUsedMediaData();
			foreach (IManagedMedia mm in collectorVisitor.getListOfCollectedMedia())
			{
				if (!usedMediaData.Contains(mm.getMediaData())) usedMediaData.Add(mm.getMediaData());
			}
			List<IDataProvider> usedDataProviders = new List<IDataProvider>();
			foreach (MediaData md in getMediaDataManager().getListOfMediaData())
			{
				if (usedMediaData.Contains(md))
				{
					if (md is urakawa.media.data.audio.codec.WavAudioMediaData)
					{
						((urakawa.media.data.audio.codec.WavAudioMediaData)md).forceSingleDataProvider();
					}
					foreach (IDataProvider dp in md.getListOfUsedDataProviders())
					{
						if (!usedDataProviders.Contains(dp)) usedDataProviders.Add(dp);
					}
				}
				else
				{
					md.delete();
				}
			}
			foreach (IDataProvider dp in getDataProviderManager().getListOfDataProviders())
			{
				if (!usedDataProviders.Contains(dp)) dp.delete();
			}
		}

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Presentation"/> from a Presentation xuk element
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
				throw new exception.XukException("Can not read Presentation from a non-element node");
			}
			try
			{
				mTreeNodeFactory = null;
				mPropertyFactory = null;
				mChannelFactory = null;
				mChannelsManager = null;
				mMediaFactory = null;
				mMediaDataManager = null;
				mMediaDataFactory = null;
				mDataProviderManager = null;
				mDataProviderFactory = null;
				mUndoRedoManager = null;
				mCommandFactory = null;
				mRootNode = null;
				mRootNodeInitialized = false;
				mRootUri = null;
				mLanguage = null;
				mMetadata.Clear();
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
					String.Format("An exception occured during xukIn of Presentation: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Presentation xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
			string rootUri = source.GetAttribute("rootUri");
			Uri baseUri = new Uri(System.IO.Directory.GetCurrentDirectory());
			if (source.BaseURI != "") baseUri = new Uri(baseUri, source.BaseURI);
			if (rootUri == null)
			{
				setRootUri(baseUri);
			}
			else
			{
				setRootUri(new Uri(baseUri, rootUri));
			}
			string lang = source.GetAttribute("language");
			if (lang != null) lang = lang.Trim();
			if (lang == "") lang = null;
			setLanguage(lang);
		}

		/// <summary>
		/// Reads an <see cref="xuk.IXukAble"/> instance from one of the children of a xuk element,
		/// more specifically the one with matching Xuk QName
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <param name="xukAble">The instance to read</param>
		protected void xukInXukAbleFromChild(XmlReader source, xuk.IXukAble xukAble)
		{
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (source.LocalName == xukAble.getXukLocalName() && source.NamespaceURI == xukAble.getXukNamespaceUri())
						{
							xukAble.xukIn(source);
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

		private void xukInMetadata(XmlReader source)
		{
			if (source.IsEmptyElement) return;
			while (source.Read())
			{
				if (source.NodeType == XmlNodeType.Element)
				{
					Metadata newMeta = getMetadataFactory().createMetadata(source.LocalName, source.NamespaceURI);
					if (newMeta != null)
					{
						newMeta.xukIn(source);
						mMetadata.Add(newMeta);
					}
					else if (!source.IsEmptyElement)
					{
						//Read past unidentified element
						source.ReadSubtree().Close();
					}
				}
				else if (source.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (source.EOF)
				{
					throw new exception.XukException("Unexpectedly reached EOF");
				}
			}
		}


		/// <summary>
		/// Reads the root <see cref="TreeNode"/> of <c>this</c> from a <c>mRootNode</c> xuk xml element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		/// <remarks>The read is considered succesful even if no valid root node is found</remarks>
		protected void xukInRootNode(XmlReader source)
		{
			setRootNode(null);
			if (!source.IsEmptyElement)
			{
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						TreeNode newRoot = getTreeNodeFactory().createNode(source.LocalName, source.NamespaceURI);
						if (newRoot != null)
						{
							newRoot.xukIn(source);
							setRootNode(newRoot);
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

		private delegate T creatorDelegate<T>(string ln, string ns);
		private delegate void setDelegate<T>(T obj);

		private void xukInXukAbleFromChild<T>(XmlReader source, T instanceVar, creatorDelegate<T> creator, setDelegate<T> setter) where T : IXukAble
		{
			if (!source.IsEmptyElement)
			{
				bool foundObj = false;
				while (source.Read())
				{
					if (source.NodeType == XmlNodeType.Element)
					{
						if (foundObj)
						{
							if (!source.IsEmptyElement)
							{
								source.ReadSubtree().Close();
							}
						}
						else
						{
							instanceVar = creator(source.LocalName, source.NamespaceURI);
							if (instanceVar != null)
							{
								setter(instanceVar);
								foundObj = true;
								instanceVar.xukIn(source);
							}
							else if (!source.IsEmptyElement)
							{
								source.ReadSubtree().Close();
							}
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

		/// <summary>
		/// Reads a child of a Presentation xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			if (source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				readItem = true;
				switch (source.LocalName)
				{
					case "mTreeNodeFactory":
						TreeNodeFactory tnFact = null;
						xukInXukAbleFromChild<TreeNodeFactory>(
							source, tnFact, 
							new creatorDelegate<TreeNodeFactory>(getDataModelFactory().createTreeNodeFactory),
							new setDelegate<TreeNodeFactory>(setTreeNodeFactory));
						break;
					case "mPropertyFactory":
						PropertyFactory pFact = null;
						xukInXukAbleFromChild<PropertyFactory>(
							source, pFact, 
							new creatorDelegate<PropertyFactory>(getDataModelFactory().createPropertyFactory),
							new setDelegate<PropertyFactory>(setPropertyFactory));
						break;
					case "mChannelFactory":
						ChannelFactory chFact = null;
						xukInXukAbleFromChild<ChannelFactory>(
							source, chFact, 
							new creatorDelegate<ChannelFactory>(getDataModelFactory().createChannelFactory),
							new setDelegate<ChannelFactory>(setChannelFactory));
						break;
					case "mChannelsManager":
						ChannelsManager chMngr = null;
						xukInXukAbleFromChild<ChannelsManager>(
							source, chMngr, 
							new creatorDelegate<ChannelsManager>(getDataModelFactory().createChannelsManager),
							new setDelegate<ChannelsManager>(setChannelsManager));
						break;
					case "mMediaFactory":
						IMediaFactory mFact = null;
						xukInXukAbleFromChild<IMediaFactory>(
							source, mFact, 
							new creatorDelegate<IMediaFactory>(getDataModelFactory().createMediaFactory),
							new setDelegate<IMediaFactory>(setMediaFactory));
						break;
					case "mMediaDataManager":
						MediaDataManager mdMngr = null;
						xukInXukAbleFromChild<MediaDataManager>(
							source, mdMngr, 
							new creatorDelegate<MediaDataManager>(getDataModelFactory().createMediaDataManager),
							new setDelegate<MediaDataManager>(setMediaDataManager));
						break;
					case "mMediaDataFactory":
						MediaDataFactory mdFact = null;
						xukInXukAbleFromChild<MediaDataFactory>(
							source, mdFact, 
							new creatorDelegate<MediaDataFactory>(getDataModelFactory().createMediaDataFactory),
							new setDelegate<MediaDataFactory>(setMediaDataFactory));
						break;
					case "mDataProviderManager":
						IDataProviderManager dpMngr = null;
						xukInXukAbleFromChild<IDataProviderManager>(
							source, dpMngr, 
							new creatorDelegate<IDataProviderManager>(getDataModelFactory().createDataProviderManager),
							new setDelegate<IDataProviderManager>(setDataProviderManager));
						break;
					case "mDataProviderFactory":
						IDataProviderFactory dpFact = null;
						xukInXukAbleFromChild<IDataProviderFactory>(
							source, dpFact, 
							new creatorDelegate<IDataProviderFactory>(getDataModelFactory().createDataProviderFactory),
							new setDelegate<IDataProviderFactory>(setDataProviderFactory));
						break;
					case "mUndoRedoManager":
						UndoRedoManager urMngr = null;
						xukInXukAbleFromChild<UndoRedoManager>(
							source, urMngr, 
							new creatorDelegate<UndoRedoManager>(getDataModelFactory().createUndoRedoManager),
							new setDelegate<UndoRedoManager>(setUndoRedoManager));
						break;
					case "mCommandFactory":
						CommandFactory cFact = null;
						xukInXukAbleFromChild<CommandFactory>(
							source, cFact, 
							new creatorDelegate<CommandFactory>(getDataModelFactory().createCommandFactory),
							new setDelegate<CommandFactory>(setCommandFactory));
						break;
					case "mMetadataFactory":
						metadata.MetadataFactory metaFact = null;
						xukInXukAbleFromChild<metadata.MetadataFactory>(
							source, metaFact,
							new creatorDelegate<MetadataFactory>(getDataModelFactory().createMetadataFactory),
							new setDelegate<MetadataFactory>(setMetadataFactory));
						break;
					case "mMetadata":
						xukInMetadata(source);
						break;
					case "mRootNode":
						xukInRootNode(source);
						break;
					default:
						readItem = false;
						break;
				}
			}
			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a Presentation element to a XUK file representing the <see cref="Presentation"/> instance
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
					String.Format("An exception occured during xukOut of Presentation: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Presentation element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			if (baseUri == null)
			{
				destination.WriteAttributeString("rootUri", getRootUri().AbsoluteUri);
			}
			else
			{
				destination.WriteAttributeString("rootUri", baseUri.MakeRelativeUri(getRootUri()).ToString());
			}
			if (getLanguage() != null)
			{
				destination.WriteAttributeString("language", getLanguage());
			}
		}

		/// <summary>
		/// Write the child elements of a Presentation element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			destination.WriteStartElement("mTreeNodeFactory", urakawa.ToolkitSettings.XUK_NS);
			getTreeNodeFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mPropertyFactory", urakawa.ToolkitSettings.XUK_NS);
			getTreeNodeFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mChannelFactory", urakawa.ToolkitSettings.XUK_NS);
			getChannelFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mChannelsManager", urakawa.ToolkitSettings.XUK_NS);
			getChannelsManager().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mMediaFactory", urakawa.ToolkitSettings.XUK_NS);
			getMediaFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mDataProviderFactory", urakawa.ToolkitSettings.XUK_NS);
			getDataProviderFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mDataProviderManager", urakawa.ToolkitSettings.XUK_NS);
			getDataProviderManager().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mMediaDataFactory", urakawa.ToolkitSettings.XUK_NS);
			getMediaDataFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mMediaDataManager", urakawa.ToolkitSettings.XUK_NS);
			getMediaDataManager().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mCommandFactory", urakawa.ToolkitSettings.XUK_NS);
			getCommandFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mUndoRedoManager", urakawa.ToolkitSettings.XUK_NS);
			getUndoRedoManager().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mMetadataFactory", urakawa.ToolkitSettings.XUK_NS);
			getMetadataFactory().xukOut(destination, baseUri);
			destination.WriteEndElement();

			destination.WriteStartElement("mMetadata", urakawa.ToolkitSettings.XUK_NS);
			foreach (Metadata md in mMetadata)
			{
				md.xukOut(destination, baseUri);
			}
			destination.WriteEndElement();

			destination.WriteStartElement("mRootNode", ToolkitSettings.XUK_NS);
			getRootNode().xukOut(destination, baseUri);
			destination.WriteEndElement();
		}



		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Presentation"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Presentation"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region ITreePresentation Members

		/// <summary>
		/// Gets the root <see cref="TreeNode"/> of <c>this</c>
		/// </summary>
		/// <returns>The root</returns>
		/// <remarks>
		/// The root <see cref="TreeNode"/> is initialized lazily:
		/// If the root <see cref="TreeNode"/> has not been explicitly set using the <see cref="setRootNode"/> method,
		/// a call to <see cref="getRootNode"/> will initialize the <see cref="Presentation"/> with a default <see cref="TreeNode"/>
		/// as returned by <c>getTreeNodeFactory().createNode()</c>
		/// </remarks>
		public TreeNode getRootNode()
		{
			if (!mRootNodeInitialized) setRootNode(getTreeNodeFactory().createNode());
			return mRootNode;
		}

		/// <summary>
		/// Sets the root <see cref="TreeNode"/> of <c>this</c>
		/// </summary>
		/// <param name="newRoot">The new root - a <c>null</c> value is allowed</param>
		/// <remarks>If the new root <see cref="TreeNode"/> has a parent it is detached</remarks>
		public void setRootNode(TreeNode newRoot)
		{
			mRootNodeInitialized = true;
			if (newRoot != null)
			{
				if (newRoot.getParent() != null) newRoot.detach();
			}
			mRootNode = newRoot;
		}

		
		/// <summary>
		/// Gets the <see cref="TreeNodeFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="TreeNodeFactory"/> of the <see cref="Presentation"/></returns>
		/// <remark>
		/// The <see cref="TreeNodeFactory"/> of a <see cref="Project"/> is initialized lazily:
		/// If the <see cref="Presentation"/> has not been explicitly initialized with a <see cref="TreeNodeFactory"/> using the <see cref="setTreeNodeFactory"/> method, 
		/// a call to <see cref="getTreeNodeFactory"/> will initialize the <see cref="Presentation"/> with a default <see cref="TreeNodeFactory"/>
		/// as returned by <c>getDataModelFactory().createTreeNodeFactory()</c>
		/// </remark>
		public TreeNodeFactory getTreeNodeFactory()
		{
			if (mTreeNodeFactory == null)
			{
				setTreeNodeFactory(getDataModelFactory().createTreeNodeFactory());
			}
			return mTreeNodeFactory;
		}
                
		/// <summary>
		/// Initializes the presentation with a <see cref="TreeNodeFactory"/>
		/// </summary>
		/// <param name="fact">The <see cref="TreeNodeFactory"/></param>
		public void setTreeNodeFactory(TreeNodeFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The TreeNodeFactory can not be null");
			}
			if (mTreeNodeFactory != null)
			{
				throw new exception.IsAlreadyInitializedException("The Presentation has already been initialized with a TreeNodeFactory");
			}
			mTreeNodeFactory = fact;
			mTreeNodeFactory.setPresentation(this);
		}

		IGenericPropertyFactory ITreePresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		
		/// <summary>
		/// Gets the <see cref="PropertyFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="PropertyFactory"/> of the <see cref="Presentation"/></returns>
		/// <remark>
		/// The <see cref="PropertyFactory"/> of a <see cref="Project"/> is initialized lazily:
		/// If the <see cref="Presentation"/> has not been explicitly initialized with a <see cref="PropertyFactory"/> using the <see cref="setPropertyFactory"/> method, 
		/// a call to <see cref="getPropertyFactory"/> will initialize the <see cref="Presentation"/> with a default <see cref="PropertyFactory"/>
		/// as returned by <c>getDataModelFactory().createPropertyFactory()</c>
		/// </remark>
		public PropertyFactory getPropertyFactory()
		{
			if (mPropertyFactory == null)
			{
				setPropertyFactory(getDataModelFactory().createPropertyFactory());
			}
			return mPropertyFactory;
		}
                
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="PropertyFactory"/>
		/// </summary>
		/// <param name="fact"></param>
		public void setPropertyFactory(PropertyFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException("The PropertyFactory can not be null");
			}
			if (mPropertyFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a PropertyFactory");
			}
			mPropertyFactory = fact;
			mPropertyFactory.setPresentation(this);
		}

		
		/// <summary>
		/// Gets the <see cref="UndoRedoManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="UndoRedoManager"/> of the <see cref="Presentation"/></returns>
		/// <remark>
		/// The <see cref="UndoRedoManager"/> of a <see cref="Project"/> is initialized lazily:
		/// If the <see cref="Presentation"/> has not been explicitly initialized with a <see cref="UndoRedoManager"/> using the <see cref="setUndoRedoManager"/> method, 
		/// a call to <see cref="getUndoRedoManager"/> will initialize the <see cref="Presentation"/> with a default <see cref="UndoRedoManager"/>
		/// as returned by <c>getDataModelFactory().createUndoRedoManager()</c>
		/// </remark>
		public UndoRedoManager getUndoRedoManager()
		{
			if (mUndoRedoManager == null)
			{
				setUndoRedoManager(getDataModelFactory().createUndoRedoManager());
			}
			return mUndoRedoManager;
		}
                
		/// <summary>
		/// Initializes the <see cref="Presentation"/> 
		/// </summary>
		/// <param name="mngr"></param>
		public void setUndoRedoManager(UndoRedoManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The UndoRedoManager can not be null");
			}
			if (mUndoRedoManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a UndoRedoManager");
			}
			mUndoRedoManager = mngr;
			mUndoRedoManager.setPresentation(this);
		}

		
		/// <summary>
		/// Gets the <see cref="CommandFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="CommandFactory"/> of the <see cref="Presentation"/></returns>
		/// <remark>
		/// The <see cref="CommandFactory"/> of a <see cref="Project"/> is initialized lazily:
		/// If the <see cref="Presentation"/> has not been explicitly initialized with a <see cref="CommandFactory"/> using the <see cref="setCommandFactory"/> method, 
		/// a call to <see cref="getCommandFactory"/> will initialize the <see cref="Presentation"/> with a default <see cref="CommandFactory"/>
		/// as returned by <c>getDataModelFactory().createCommandFactory()</c>
		/// </remark>
		public CommandFactory getCommandFactory()
		{
			if (mCommandFactory == null)
			{
				setCommandFactory(getDataModelFactory().createCommandFactory());
			}
			return mCommandFactory;
		}
                        
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="CommandFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="CommandFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="CommandFactory"/>
		/// </exception>
		public void setCommandFactory(CommandFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The CommandFactory can not be null");
			}
			if (mCommandFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a CommandFactory");
			}
			mCommandFactory = fact;
			mCommandFactory.setPresentation(this);
		}
		#region IMediaPresentation Members



		
		/// <summary>
		/// Gets the <see cref="IMediaFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IMediaFactory"/> of the <see cref="Presentation"/></returns>
		/// <remark>
		/// The <see cref="IMediaFactory"/> of a <see cref="Project"/> is initialized lazily:
		/// If the <see cref="Presentation"/> has not been explicitly initialized with a <see cref="IMediaFactory"/> using the <see cref="setMediaFactory"/> method, 
		/// a call to <see cref="getMediaFactory"/> will initialize the <see cref="Presentation"/> with a default <see cref="IMediaFactory"/>
		/// as returned by <c>getDataModelFactory().createMediaFactory()</c>
		/// </remark>
		public IMediaFactory getMediaFactory()
		{
			if (mMediaFactory == null)
			{
				setMediaFactory(getDataModelFactory().createMediaFactory());
			}
			return mMediaFactory;
		}
                        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="IMediaFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="IMediaFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="IMediaFactory"/>
		/// </exception>
		public void setMediaFactory(IMediaFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The IMediaFactory can not be null");
			}
			if (mMediaFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a IMediaFactory");
			}
			mMediaFactory = fact;
			mMediaFactory.setPresentation(this);
		}
        

		/// <summary>
		/// Gets the root <see cref="Uri"/> of the <see cref="Presentation"/>
		/// </summary>
		/// <returns>The root <see cref="Uri"/></returns>
		/// <remarks>
		/// The root <see cref="Uri"/> is initialized lazily:
		/// If the root <see cref="Uri"/> has not been set explicitly using the <see cref="setRootUri"/> method,
		/// 
		/// </remarks>
		public Uri getRootUri()
		{
			if (mRootUri == null)
			{
				mRootUri = new Uri(System.IO.Directory.GetCurrentDirectory());
			}
			return mRootUri;
		}

		/// <summary>
		/// Sets the 
		/// </summary>
		/// <param name="newRootUri">The new base uri</param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when the new uri is <c>null</c></exception>
		/// <exception cref="exception.InvalidUriException">Thrown when the given uri is not absolute</exception>
		public void setRootUri(Uri newRootUri)
		{
			if (newRootUri == null)
			{
				throw new exception.MethodParameterIsNullException("The base Uri can not be null");
			}
			if (!newRootUri.IsAbsoluteUri)
			{
				throw new exception.InvalidUriException("The base uri must be absolute");
			}
			Uri prev = mRootUri;
			mRootUri = newRootUri;
			if (prev == null)
			{
				notifyRootUriChanged(null);
			}
			else if (prev.AbsoluteUri != mRootUri.AbsoluteUri)
			{
				notifyRootUriChanged(prev);
			}
		}

		/// <summary>
		/// Fired when the base <see cref="Uri"/> has changed
		/// </summary>
		public event RootUriChangedEventHandler rootUriChanged;

		private void notifyRootUriChanged(Uri prevUri)
		{
			RootUriChangedEventHandler d = rootUriChanged;
			if (d!=null) d(this, new RootUriChangedEventArgs(prevUri));
		}

		/// <summary>
		/// Gets a list of the <see cref="IMedia"/> used by a given <see cref="TreeNode"/>. 
		/// </summary>
		/// <param name="node">The node</param>
		/// <returns>The list</returns>
		/// <remarks>
		/// An <see cref="IMedia"/> is considered to be used by a <see cref="TreeNode"/> if the media
		/// is linked to the node via. a <see cref="ChannelsProperty"/>
		/// </remarks>
		protected virtual List<IMedia> getListOfMediaUsedByTreeNode(TreeNode node)
		{
			List<IMedia> res = new List<IMedia>();
			foreach (Property prop in node.getListOfProperties())
			{
				if (prop is ChannelsProperty)
				{
					ChannelsProperty chProp = (ChannelsProperty)prop;
					foreach (Channel ch in chProp.getListOfUsedChannels())
					{
						res.Add(chProp.getMedia(ch));
					}
				}
			}
			return res;
		}

		/// <summary>
		/// Gets the list of <see cref="IMedia"/> used by the <see cref="TreeNode"/> tree of the presentation. 
		/// Remark that a 
		/// </summary>
		/// <returns>The list</returns>
		public List<IMedia> getListOfUsedMedia()
		{
			List<IMedia> res = new List<IMedia>();
			if (getRootNode() != null)
			{
				collectUsedMedia(getRootNode(), res);
			}
			return res;
		}

		private void collectUsedMedia(TreeNode node, List<IMedia> collectedMedia)
		{
			foreach (IMedia m in getListOfMediaUsedByTreeNode(node))
			{
				if (!collectedMedia.Contains(m)) collectedMedia.Add(m);
			}
			for (int i = 0; i < node.getChildCount(); i++)
			{
				collectUsedMedia(node.getChild(i), collectedMedia);
			}
		}




		#endregion

		#region IChannelPresentation Members

		
		/// <summary>
		/// Gets the <see cref="ChannelFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public ChannelFactory getChannelFactory()
		{
			if (mChannelFactory == null)
			{
				setChannelFactory(getDataModelFactory().createChannelFactory());
			}
			return mChannelFactory;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="ChannelFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="ChannelFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="ChannelFactory"/>
		/// </exception>
		public void setChannelFactory(ChannelFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The ChannelFactory can not be null");
			}
			if (mChannelFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a ChannelFactory");
			}
			mChannelFactory = fact;
			mChannelFactory.setPresentation(this);
		}


		
		/// <summary>
		/// Gets the <see cref="ChannelsManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public ChannelsManager getChannelsManager()
		{
			if (mChannelsManager == null)
			{
				setChannelsManager(getDataModelFactory().createChannelsManager());
			}
			return mChannelsManager;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="ChannelsManager"/>
		/// </summary>
		/// <param name="fact">The new <see cref="ChannelsManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="ChannelsManager"/>
		/// </exception>
		public void setChannelsManager(ChannelsManager fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The ChannelsManager can not be null");
			}
			if (mChannelsManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a ChannelsManager");
			}
			mChannelsManager = fact;
			mChannelsManager.setPresentation(this);
		}
        

		/// <summary>
		/// Gets the <see cref="IChannelsPropertyFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The <see cref="IChannelsPropertyFactory"/></returns>
		IChannelsPropertyFactory IChannelPresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		#region IXmlPresentation Members

		/// <summary>
		/// Gets the factory creating <see cref="urakawa.property.xml.XmlProperty"/>s 
		/// and <see cref="urakawa.property.xml.XmlAttribute"/>s used by theese
		/// </summary>
		/// <returns>The factory</returns>
		IXmlPropertyFactory IXmlPresentation.getPropertyFactory()
		{
			return getPropertyFactory();
		}

		#endregion

		#region IMediaDataPresentation Members

		
		/// <summary>
		/// Gets the <see cref="MediaDataManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public MediaDataManager getMediaDataManager()
		{
			if (mMediaDataManager == null)
			{
				setMediaDataManager(getDataModelFactory().createMediaDataManager());
			}
			return mMediaDataManager;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="MediaDataManager"/>
		/// </summary>
		/// <param name="mngr">The new <see cref="MediaDataManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="MediaDataManager"/>
		/// </exception>
		public void setMediaDataManager(MediaDataManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MediaDataManager can not be null");
			}
			if (mMediaDataManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a MediaDataManager");
			}
			mMediaDataManager = mngr;
			mMediaDataManager.setPresentation(this);
		}


		
		/// <summary>
		/// Gets the <see cref="MediaDataFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public MediaDataFactory getMediaDataFactory()
		{
			if (mMediaDataFactory == null)
			{
				setMediaDataFactory(getDataModelFactory().createMediaDataFactory());
			}
			return mMediaDataFactory;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="MediaDataFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="MediaDataFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="MediaDataFactory"/>
		/// </exception>
		public void setMediaDataFactory(MediaDataFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MediaDataFactory can not be null");
			}
			if (mMediaDataFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a MediaDataFactory");
			}
			mMediaDataFactory = fact;
			mMediaDataFactory.setPresentation(this);
		}


		
		/// <summary>
		/// Gets the <see cref="IDataProviderManager"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public IDataProviderManager getDataProviderManager()
		{
			if (mDataProviderManager == null)
			{
				setDataProviderManager(getDataModelFactory().createDataProviderManager());
			}
			return mDataProviderManager;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="IDataProviderManager"/>
		/// </summary>
		/// <param name="mngr">The new <see cref="IDataProviderManager"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="mngr"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="IDataProviderManager"/>
		/// </exception>
		public void setDataProviderManager(IDataProviderManager mngr)
		{
			if (mngr == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The IDataProviderManager can not be null");
			}
			if (mDataProviderManager != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a IDataProviderManager");
			}
			mDataProviderManager = mngr;
			mDataProviderManager.setPresentation(this);
		}


		
		/// <summary>
		/// Gets the <see cref="IDataProviderFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public IDataProviderFactory getDataProviderFactory()
		{
			if (mDataProviderFactory == null)
			{
				setDataProviderFactory(getDataModelFactory().createDataProviderFactory());
			}
			return mDataProviderFactory;
		}

		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="IDataProviderFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="IDataProviderFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="IDataProviderFactory"/>
		/// </exception>
		public void setDataProviderFactory(IDataProviderFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The IDataProviderFactory can not be null");
			}
			if (mDataProviderFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a IDataProviderFactory");
			}
			mDataProviderFactory = fact;
			mDataProviderFactory.setPresentation(this);
		}
        

		#endregion

		#region IValueEquatable<Presentation> Members


		/// <summary>
		/// Determines of <c>this</c> has the same value as a given other instance
		/// </summary>
		/// <param name="other">The other instance</param>
		/// <returns>A <see cref="bool"/> indicating the result</returns>
		public bool valueEquals(Presentation other)
		{
			if (other == null) return false;
			if (!getChannelsManager().valueEquals(other.getChannelsManager())) return false;
			if (!getDataProviderManager().valueEquals(other.getDataProviderManager())) return false;
			if (!getMediaDataManager().valueEquals(other.getMediaDataManager())) return false;
			if (!getRootNode().valueEquals(other.getRootNode())) return false;
			List<Metadata> thisMetadata = getMetadataList();
			List<Metadata> otherMetadata = other.getMetadataList();
			if (thisMetadata.Count != otherMetadata.Count) return false;
			foreach (Metadata m in thisMetadata)
			{
				bool found = false;
				foreach (Metadata om in other.getMetadataList(m.getName()))
				{
					if (m.ValueEquals(om)) found = true;
				}
				if (!found) return false;
			}
			if (getLanguage() != other.getLanguage()) return false;
			return true;
		}

		#endregion

		#region ITreeNodeChangedEventManager Members

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is changed, i.e. added or removed 
		/// as the child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeChangedEventHandler coreNodeChanged;

		/// <summary>
		/// Fires the <see cref="coreNodeChanged"/> event
		/// </summary>
		/// <param name="changedNode">The node that changed</param>
		public void notifyTreeNodeChanged(TreeNode changedNode)
		{
			TreeNodeChangedEventHandler d = coreNodeChanged;//Copy to local variable to make thread safe
			if (d != null) d(this, new TreeNodeChangedEventArgs(changedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeAddedEventHandler treeNodeAdded;

		/// <summary>
		/// Fires the <see cref="treeNodeAdded"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="addedNode">The node that has been added</param>
		public void notifyTreeNodeAdded(TreeNode addedNode)
		{
			TreeNodeAddedEventHandler d = treeNodeAdded;//Copy to local variable to make thread safe
			if (d != null) d(this, new TreeNodeAddedEventArgs(addedNode));
		}

		/// <summary>
		/// Event fired whenever a <see cref="TreeNode"/> is added as a child of another <see cref="TreeNode"/>
		/// </summary>
		public event TreeNodeRemovedEventHandler treeNodeRemoved;

		/// <summary>
		/// Fires the <see cref="treeNodeRemoved"/> and <see cref="coreNodeChanged"/> events (in that order)
		/// </summary>
		/// <param name="removedNode">The node that has been removed</param>
		/// <param name="formerParent">The parent node from which the node was removed as a child of</param>
		/// <param name="formerPosition">The position the node previously had of the list of children of it's former parent</param>
		public void notifyTreeNodeRemoved(TreeNode removedNode, TreeNode formerParent, int formerPosition)
		{
			TreeNodeRemovedEventHandler d = treeNodeRemoved;
			if (d != null) d(this, new TreeNodeRemovedEventArgs(removedNode, formerParent, formerPosition));
			notifyTreeNodeChanged(removedNode);
		}

		#endregion

		#region Metadata
		private List<Metadata> mMetadata;
		private MetadataFactory mMetadataFactory;


		
		/// <summary>
		/// Gets the <see cref="MetadataFactory"/> of <c>this</c>
		/// </summary>
		/// <returns>The factory</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when the <see cref="Presentation"/> has not been initialized with a <see cref="TreeNodeFactory"/>
		/// </exception>
		public MetadataFactory getMetadataFactory()
		{
			if (mMetadataFactory == null)
			{
				setMetadataFactory(getDataModelFactory().createMetadataFactory());
			}
			return mMetadataFactory;
		}
        
		
		/// <summary>
		/// Initializes the <see cref="Presentation"/> with a <see cref="MetadataFactory"/>
		/// </summary>
		/// <param name="fact">The new <see cref="MetadataFactory"/></param>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="fact"/> is null
		/// </exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when the <see cref="Presentation"/> has already been initialized with a <see cref="MetadataFactory"/>
		/// </exception>
		public void setMetadataFactory(MetadataFactory fact)
		{
			if (fact == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The MetadataFactory can not be null");
			}
			if (mMetadataFactory != null)
			{
				throw new exception.IsAlreadyInitializedException(
					"The Presentation has already been initialized with a MetadataFactory");
			}
			mMetadataFactory = fact;
			mMetadataFactory.setPresentation(this);
		}
        


		/// <summary>
		/// Appends a <see cref="Metadata"/> to the <see cref="Project"/>
		/// </summary>
		/// <param name="metadata">The <see cref="Metadata"/> to add</param>
		public void appendMetadata(Metadata metadata)
		{
			mMetadata.Add(metadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/>
		/// </summary>
		/// <returns>The <see cref="List{Metadata}"/> of metadata <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList()
		{
			return new List<Metadata>(mMetadata);
		}

		/// <summary>
		/// Gets a <see cref="List{Metadata}"/> of all <see cref="Metadata"/>
		/// in the <see cref="Project"/> with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		/// <returns>The <see cref="List{Metadata}"/> of <see cref="Metadata"/></returns>
		public List<Metadata> getMetadataList(string name)
		{
			List<Metadata> list = new List<Metadata>();
			foreach (Metadata md in mMetadata)
			{
				if (md.getName() == name) list.Add(md);
			}
			return list;
		}

		/// <summary>
		/// Deletes all <see cref="Metadata"/>s with a given name
		/// </summary>
		/// <param name="name">The given name</param>
		public void deleteMetadata(string name)
		{
			foreach (Metadata md in getMetadataList(name))
			{
				deleteMetadata(md);
			}
		}

		/// <summary>
		/// Deletes a given <see cref="Metadata"/>
		/// </summary>
		/// <param name="metadata">The given <see cref="Metadata"/></param>
		public void deleteMetadata(Metadata metadata)
		{
			mMetadata.Remove(metadata);
		}

		#endregion

	}
}