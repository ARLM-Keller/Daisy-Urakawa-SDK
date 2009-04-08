using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media.data
{
    /// <summary>
    /// Manager for <see cref="DataProvider"/>s
    /// </summary>
    public sealed class DataProviderManager : XukAble
    {

        public override string GetTypeNameFormatted()
        {
            return XukStrings.DataProviderManager;
        }
        private Presentation mPresentation;

        /// <summary>
        /// Gets the <see cref="Presentation"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The owning <see cref="Presentation"/></returns>
        public Presentation Presentation
        {
            get
            {
                return mPresentation;
            }
        }

        public DataProviderManager(Presentation pres)
        {
            mPresentation = pres;
            mDataFileDirectory = null;
            m_CompareByteStreamsDuringValueEqual = true;
        }

        public void AllowCopyDataOnUriChanged(bool enable)
        {
            if (enable)
            {
                Presentation.RootUriChanged += Presentation_rootUriChanged;
            }
            else
            {
                Presentation.RootUriChanged -= Presentation_rootUriChanged;
            }
        }

        private Dictionary<string, DataProvider> mDataProvidersDictionary = new Dictionary<string, DataProvider>();

        private Dictionary<DataProvider, string> mReverseLookupDataProvidersDictionary =
            new Dictionary<DataProvider, string>();

        private List<string> mXukedInFilDataProviderPaths = new List<string>();
        private string mDataFileDirectory;


        /// <summary>
        /// Appends data from a given input <see cref="Stream"/> to a given <see cref="DataProvider"/>
        /// </summary>
        /// <param name="data">The given input stream</param>
        /// <param name="count">The number of bytes to append</param>
        /// <param name="provider">The given data provider</param>
        public static void AppendDataToProvider(Stream data, long count, DataProvider provider)
        {
            if (count <= 0)
            {
                return;
            }

            if (count > (data.Length - data.Position))
            {
                throw new exception.InputStreamIsTooShortException(
                            String.Format("The given data Stream is shorter than the requested {0:0} bytes",
                            count));
            }

            Stream provOutputStream = provider.GetOutputStream();

            try
            {
                provOutputStream.Seek(0, SeekOrigin.End);

                const int BUFFER_SIZE = 1024 * 300; // 300 KB MAX BUFFER  
                if (count <= BUFFER_SIZE)
                {
                    byte[] buffer = new byte[count];
                    int bytesRead = data.Read(buffer, 0, (int)count);
                    if (bytesRead > 0)
                    {
                        provOutputStream.Write(buffer, 0, bytesRead);
                    }
                    else
                    {
                        throw new exception.InputStreamIsTooShortException(
                            String.Format("Can not read {0:0} bytes from the given data Stream",
                            count));
                    }
                }
                else
                {
                    int bytesRead = 0;
                    int totalBytesWritten = 0;
                    byte[] buffer = new byte[BUFFER_SIZE];

                    while ((bytesRead = data.Read(buffer, 0, BUFFER_SIZE)) > 0)
                    {
                        if ((totalBytesWritten + bytesRead) > count)
                        {
                            int bytesToWrite = (int)(count - totalBytesWritten);
                            provOutputStream.Write(buffer, 0, bytesToWrite);
                            totalBytesWritten += bytesToWrite;
                        }
                        else
                        {
                            provOutputStream.Write(buffer, 0, bytesRead);
                            totalBytesWritten += bytesRead;
                        }
                    }
                }
            }
            finally
            {
                provOutputStream.Close();
            }
        }

        /// <summary>
        /// Compares the data content of two data providers to check for value equality
        /// </summary>
        /// <param name="dp1">Data provider 1</param>
        /// <param name="dp2">Data provider 2</param>
        /// <returns>A <see cref="bool"/> indicating if the data content is identical</returns>
        public static bool CompareDataProviderContent(DataProvider dp1, DataProvider dp2)
        {
            Stream s1 = null;
            Stream s2 = null;
            bool allEq;
            try
            {
                s1 = dp1.GetInputStream();
                s2 = dp2.GetInputStream();
                allEq = ((s1.Length - s1.Position) == (s2.Length - s2.Position));
                while (allEq && (s1.Position < s1.Length))
                {
                    if (s1.ReadByte() != s2.ReadByte())
                    {
                        allEq = false;
                        break;
                    }
                }
            }
            finally
            {
                if (s1 != null) s1.Close();
                if (s2 != null) s2.Close();
            }
            return allEq;
        }


        /// <summary>
        /// Gets the path of the data file directory used by <see cref="FileDataProvider"/>s
        /// managed by <c>this</c>, relative to the base uri of the <see cref="Presentation"/>
        /// owning the file data provider manager.
        /// </summary>
        /// <returns>The path</returns>
        /// <remarks>
        /// The DataFileDirectory is initialized lazily:
        /// If the DataFileDirectory has not been explicitly initialized using the <see cref="DataFileDirectory"/> setter,
        /// retrieving <see cref="DataFileDirectory"/> will assing it the default value "Data"</remarks>
        public string DataFileDirectory
        {
            get
            {
                if (mDataFileDirectory == null) mDataFileDirectory = "Data";
                return mDataFileDirectory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The DataFileDirectory can not be null");
                }
                if (mDataFileDirectory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The DataProviderManager has already been initialized with a DataFileDirectory");
                }
                Uri tmp;
                if (!Uri.TryCreate(value, UriKind.Relative, out tmp))
                {
                    throw new exception.InvalidUriException(String.Format(
                                                                "DataFileDirectory must be a relative Uri, '{0}' is not",
                                                                value));
                }

                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                mDataFileDirectory = value;
            }
        }

        /// <summary>
        /// Moves the data file directory of the manager
        /// </summary>
        /// <param name="newDataFileDir">The new data file direcotry</param>
        /// <param name="deleteSource">A <see cref="bool"/> indicating if the source/old data files shlould be deleted</param>
        /// <param name="overwriteDestDir">A <see cref="bool"/> indicating if the new data directory should be overwritten</param>
        public void MoveDataFiles(string newDataFileDir, bool deleteSource, bool overwriteDestDir)
        {
            if (Path.IsPathRooted(newDataFileDir))
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The data file directory path must be relative");
            }
            string oldPath = DataFileDirectoryFullPath;
            mDataFileDirectory = newDataFileDir;
            string newPath = DataFileDirectoryFullPath;
            try
            {
                if (Directory.Exists(newPath))
                {
                    if (overwriteDestDir)
                    {
                        Directory.Delete(newPath);
                    }
                    else
                    {
                        throw new exception.OperationNotValidException(
                            String.Format("Directory {0} already exists", newPath));
                    }
                }
                CopyDataFiles(oldPath, newPath);
                if (deleteSource && Directory.Exists(oldPath))
                {
                    Directory.Delete(oldPath);
                }
            }
            catch (Exception e)
            {
                throw new exception.OperationNotValidException(
                    String.Format("Could not move data files to {0}: {1}", newPath, e.Message),
                    e);
            }
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                string parentDir = Path.GetDirectoryName(path);
                if (!Directory.Exists(parentDir)) CreateDirectory(parentDir);
                Directory.CreateDirectory(path);
            }
        }

        private void CopyDataFiles(string source, string dest)
        {
            CreateDirectory(dest);
            foreach (FileDataProvider fdp in ListOfFileDataProviders)
            {
                if (!File.Exists(Path.Combine(source, fdp.DataFileRelativePath)))
                {
                    throw new exception.DataMissingException(String.Format(
                                                                 "Error while copying data files from {0} to {1}: Data file {2} does not exist in the source",
                                                                 source, dest, fdp.DataFileRelativePath));
                }
                File.Copy(Path.Combine(source, fdp.DataFileRelativePath), Path.Combine(dest, fdp.DataFileRelativePath));
            }
        }

        private string getDataFileDirectoryFullPath(Uri baseUri)
        {
            if (!baseUri.IsFile)
            {
                throw new exception.InvalidUriException(
                    "The base Uri of the presentation to which the DataProviderManager belongs must be a file Uri");
            }
            Uri dataFileDirUri = new Uri(baseUri, DataFileDirectory);
            return dataFileDirUri.LocalPath;
        }


        /// <summary>
        /// Gets the full path of the data file directory. 
        /// Convenience for <c>Path.Combine(getBasePath(), getDataFileDirectory())</c>
        /// </summary>
        /// <returns>The full path</returns>
        public string DataFileDirectoryFullPath
        {
            get { return getDataFileDirectoryFullPath(Presentation.RootUri); }
        }

        /*
        /// <summary>
        /// Initializer that sets the path of the data file directory
        /// used by <see cref="FileDataProvider"/>s managed by <c>this</c>
        /// </summary>
        public string DataFileDirectoryPath
        {
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The path of the data file directory can not be null");
                }
                if (mDataFileDirectory != null)
                {
                    throw new exception.IsAlreadyInitializedException(
                        "The data provider manager already has a data file directory");
                }
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }
                mDataFileDirectory = value;
            }
        }
         */

        /// <summary>
        /// Gets a new data file path relative to the path of the data file directory of the manager
        /// </summary>
        /// <param name="extension">The entension of the new data file path</param>
        /// <returns>The relative path</returns>
        public string GetNewDataFileRelPath(string extension)
        {
            string res;
            while (true)
            {
                res = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                foreach (FileDataProvider prov in ListOfFileDataProviders)
                {
                    if (!prov.IsDataFileInitialized) continue;
                    if (res.ToLower() == prov.DataFileRelativePath.ToLower()) continue;
                }
                break;
            }

            return res;
        }

        /// <summary>
        /// Gets a list of the <see cref="FileDataProvider"/>s managed by the manager
        /// </summary>
        /// <returns>The list of file data providers</returns>
        public List<FileDataProvider> ListOfFileDataProviders
        {
            get
            {
                List<FileDataProvider> res = new List<FileDataProvider>();
                foreach (DataProvider prov in ListOfDataProviders)
                {
                    if (prov is FileDataProvider)
                    {
                        res.Add((FileDataProvider)prov);
                    }
                }
                return res;
            }
        }

        #region IDataProviderManager Members

        private void Presentation_rootUriChanged(Object o, events.presentation.RootUriChangedEventArgs e)
        {
            if (e.PreviousUri != null)
            {
                string prevDataDirFullPath = getDataFileDirectoryFullPath(e.PreviousUri);
                if (Directory.Exists(prevDataDirFullPath))
                {
                    CopyDataFiles(prevDataDirFullPath, DataFileDirectoryFullPath);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="urakawa.media.data.DataProviderFactory"/> of the <see cref="DataProviderManager"/>
        /// </summary>
        /// <returns>The <see cref="DataProviderFactory"/></returns>
        public DataProviderFactory DataProviderFactory
        {
            get
            {
                return Presentation.DataProviderFactory;
            }
        }

        /// <summary>
        /// Detaches one of the <see cref="DataProvider"/>s managed by the manager
        /// </summary>
        /// <param name="provider">The <see cref="DataProvider"/> to delete</param>
        /// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted</param>
        public void RemoveDataProvider(DataProvider provider, bool delete)
        {
            if (provider == null)
            {
                throw new exception.MethodParameterIsNullException("Can not detach a null DataProvider from the manager");
            }
            if (delete)
            {
                provider.Delete();
            }
            else
            {
                string uid = GetUidOfDataProvider(provider);
                RemoveDataProvider(uid, provider);
            }
        }


        /// <summary>
        /// Detaches the <see cref="DataProvider"/> with a given UID from the manager
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <param name="delete">A <see cref="bool"/> indicating if the removed data provider should be deleted</param>
        public void RemoveDataProvider(string uid, bool delete)
        {
            DataProvider provider = GetDataProvider(uid);
            if (delete)
            {
                provider.Delete();
            }
            else
            {
                RemoveDataProvider(uid, provider);
            }
        }

        private void RemoveDataProvider(string uid, DataProvider provider)
        {
            mDataProvidersDictionary.Remove(uid);
            mReverseLookupDataProvidersDictionary.Remove(provider);
        }

        /// <summary>
        /// Gets the UID of a given <see cref="DataProvider"/>
        /// </summary>
        /// <param name="provider">The given data provider</param>
        /// <returns>The UID of <paramref name="provider"/></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="provider"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when data provider <paramref name="provider"/> is not managed by <c>this</c>
        /// </exception>
        public string GetUidOfDataProvider(DataProvider provider)
        {
            if (provider == null)
            {
                throw new exception.MethodParameterIsNullException("Can not get the uid of a null DataProvider");
            }
            if (!mReverseLookupDataProvidersDictionary.ContainsKey(provider))
            {
                throw new exception.IsNotManagerOfException("The given DataProvider is not managed by this");
            }
            return mReverseLookupDataProvidersDictionary[provider];
        }

        /// <summary>
        /// Gets the <see cref="DataProvider"/> with a given UID
        /// </summary>
        /// <param name="uid">The given UID</param>
        /// <returns>The data provider with the given UID</returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="uid"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// When no data providers managed by <c>this</c> has the given UID
        /// </exception>
        public DataProvider GetDataProvider(string uid)
        {
            if (uid == null)
            {
                throw new exception.MethodParameterIsNullException("Can not get the data provider with UID null");
            }
            if (!mDataProvidersDictionary.ContainsKey(uid))
            {
                throw new exception.IsNotManagerOfException(
                    String.Format("The manager does not manage a DataProvider with UID {0}", uid));
            }
            return mDataProvidersDictionary[uid];
        }

        /// <summary>
        /// Adds a <see cref="DataProvider"/> to the manager with a given uid
        /// </summary>
        /// <param name="provider">The data provider to add</param>
        /// <param name="uid">The uid to assign to the added data provider</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="provider"/> or <paramref name="uid"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when the data provider is already added tothe manager 
        /// or if the manager already manages another data provider with the given uid
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">Thrown if the data provides does not have <c>this</c> as manager</exception>
        private void AddDataProvider(DataProvider provider, string uid)
        {
            if (provider == null)
            {
                throw new exception.MethodParameterIsNullException("Can not manage a null DataProvider");
            }
            if (uid == null)
            {
                throw new exception.MethodParameterIsNullException("A managed DataProvider can not have uid null");
            }
            if (mReverseLookupDataProvidersDictionary.ContainsKey(provider))
            {
                throw new exception.IsAlreadyManagerOfException(
                    "The given DataProvider is already managed by the manager");
            }
            if (mDataProvidersDictionary.ContainsKey(uid))
            {
                throw new exception.IsAlreadyManagerOfException(String.Format(
                                                                    "Another DataProvider with uid {0} is already manager by the manager",
                                                                    uid));
            }
            if (provider.DataProviderManager != this)
            {
                throw new exception.IsNotManagerOfException(
                    "The given DataProvider does not return this as DataProviderManager");
            }

            mDataProvidersDictionary.Add(uid, provider);
            mReverseLookupDataProvidersDictionary.Add(provider, uid);
        }

        /// <summary>
        /// Adds a <see cref="DataProvider"/> to be managed by the manager
        /// </summary>
        /// <param name="provider">The data provider</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref name="provider"/> is <c>null</c>
        /// </exception>
        /// <exception cref="exception.IsAlreadyManagerOfException">
        /// Thrown when <paramref name="provider"/> is already managed by <c>this</c>
        /// </exception>
        /// <exception cref="exception.IsNotManagerOfException">
        /// Thrown when <paramref name="provider"/> does not return <c>this</c> as owning manager
        /// </exception>
        /// <seealso cref="DataProvider.DataProviderManager"/>
        public void AddDataProvider(DataProvider provider)
        {
            AddDataProvider(provider, GetNextUid());
        }

        /// <summary>
        /// Determines if the manager manages a <see cref="DataProvider"/> with a given uid
        /// </summary>
        /// <param name="uid">The given uid</param>
        /// <returns>
        /// A <see cref="bool"/> indicating if the manager manages a <see cref="DataProvider"/> with the given uid
        /// </returns>
        public bool IsManagerOf(string uid)
        {
            return mDataProvidersDictionary.ContainsKey(uid);
        }

        /// <summary>
        /// Sets the uid of a given <see cref="DataProvider"/> to a given value
        /// </summary>
        /// <param name="provider">The given data provider</param>
        /// <param name="uid">The given uid</param>
        public void SetDataProviderUid(DataProvider provider, string uid)
        {
            RemoveDataProvider(provider, false);
            AddDataProvider(provider, uid);
        }

        private string GetNextUid()
        {
            ulong i = 0;
            while (i < UInt64.MaxValue)
            {
                string newId = String.Format(
                    "DPID{0:0000}", i);
                if (!mDataProvidersDictionary.ContainsKey(newId)) return newId;
                i++;
            }
            throw new OverflowException("YOU HAVE WAY TOO MANY DATAPROVIDERS!!!");
        }

        /// <summary>
        /// Gets a list of the <see cref="DataProvider"/>s managed by the manager
        /// </summary>
        /// <returns>The list</returns>
        public List<DataProvider> ListOfDataProviders
        {
            get { return new List<DataProvider>(mDataProvidersDictionary.Values); }
        }

        private bool m_CompareByteStreamsDuringValueEqual = true;

        public bool CompareByteStreamsDuringValueEqual
        {
            get { return m_CompareByteStreamsDuringValueEqual; }
            set { m_CompareByteStreamsDuringValueEqual = value; }
        }

        /// <summary>
        /// Remove all <see cref="DataProvider"/> that are managed by the manager, 
        /// but are not used by any <see cref="MediaData"/>
        /// </summary>
        /// <param name="delete">A <see cref="bool"/> indicating if the removed data providers should be deleted</param>
        public void RemoveUnusedDataProviders(bool delete)
        {
            List<DataProvider> usedDataProviders = new List<DataProvider>();
            foreach (MediaData md in Presentation.MediaDataManager.ListOfMediaData)
            {
                foreach (DataProvider prov in md.ListOfUsedDataProviders)
                {
                    if (!usedDataProviders.Contains(prov)) usedDataProviders.Add(prov);
                }
            }
            foreach (DataProvider prov in ListOfDataProviders)
            {
                if (!usedDataProviders.Contains(prov))
                {
                    RemoveDataProvider(prov, delete);
                }
            }
        }

        #endregion

        #region IXukAble Members

        /// <summary>
        /// Clears the <see cref="DataProviderManager"/>, clearing any links to <see cref="DataProvider"/>s
        /// </summary>
        protected override void Clear()
        {
            mDataProvidersDictionary.Clear();
            mDataFileDirectory = null;
            mReverseLookupDataProvidersDictionary.Clear();
            mXukedInFilDataProviderPaths.Clear();
            base.Clear();
        }


        /// <summary>
        /// Reads the attributes of a DataProviderManager xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string dataFileDirectoryPath = source.GetAttribute(XukStrings.DataFileDirectoryPath);
            if (dataFileDirectoryPath == null || dataFileDirectoryPath == "")
            {
                throw new exception.XukException(
                    "dataFileDirectoryPath attribute is missing from DataProviderManager element");
            }
            DataFileDirectory = dataFileDirectoryPath;
        }

        /// <summary>
        /// Reads a child of a DataProviderManager xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukNamespaceUri)
            {
                readItem = true;
                if (source.LocalName == XukStrings.DataProviders)
                {
                    XukInDataProviders(source, handler);
                }
                else if (!Presentation.Project.IsPrettyFormat()
                    //&& source.LocalName == XukStrings.DataProviderItem
                    )
                {
                    //XukInDataProviderItem(source, handler);
                    XukInDataProvider(source, handler);
                }
                else
                {
                    readItem = false;
                }
            }
            if (!(readItem || source.IsEmptyElement))
            {
                source.ReadSubtree().Close(); //Read past invalid MediaDataItem element
            }
        }

        private void XukInDataProviders(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == XukStrings.DataProviderItem && source.NamespaceURI == XukNamespaceUri)
                        {
                            XukInDataProviderItem(source, handler);
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

        private void XukInDataProvider(XmlReader source, ProgressHandler handler)
        {
            if (source.NodeType == XmlNodeType.Element)
            {
                DataProvider prov = DataProviderFactory.Create("", source.LocalName, source.NamespaceURI);
                if (prov != null)
                {
                    string uid = source.GetAttribute(XukStrings.Uid);
                    if (string.IsNullOrEmpty(uid))
                    {
                        throw new exception.XukException("uid attribute of mDataProviderItem element is missing");
                    }
                    prov.XukIn(source, handler);
                    if (prov is FileDataProvider)
                    {
                        FileDataProvider fdProv = (FileDataProvider)prov;
                        if (mXukedInFilDataProviderPaths.Contains(fdProv.DataFileRelativePath.ToLower()))
                        {
                            throw new exception.XukException(String.Format(
                                                                 "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                 fdProv.DataFileRelativePath.ToLower()));
                        }
                        mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath.ToLower());
                    }
                    
                    if (IsManagerOf(uid))
                    {
                        if (GetDataProvider(uid) != prov)
                        {
                            throw new exception.XukException(
                                String.Format("Another DataProvider exists in the manager with uid {0}", uid));
                        }
                    }
                    else
                    {
                        SetDataProviderUid(prov, uid);
                    }
                }
                else if (!source.IsEmptyElement)
                {
                    source.ReadSubtree().Close();
                }
            }
        }

        private void XukInDataProviderItem(XmlReader source, ProgressHandler handler)
        {
            string uid = source.GetAttribute(XukStrings.Uid);
            if (!source.IsEmptyElement)
            {
                bool addedProvider = false;
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        DataProvider prov = DataProviderFactory.Create("", source.LocalName, source.NamespaceURI);
                        if (prov != null)
                        {
                            if (addedProvider)
                            {
                                throw new exception.XukException(
                                    "Multiple DataProviders within the same mDataProviderItem is not supported");
                            }
                            prov.XukIn(source, handler);
                            if (prov is FileDataProvider)
                            {
                                FileDataProvider fdProv = (FileDataProvider)prov;
                                if (mXukedInFilDataProviderPaths.Contains(fdProv.DataFileRelativePath.ToLower()))
                                {
                                    throw new exception.XukException(String.Format(
                                                                         "Another FileDataProvider using data file {0} has already been Xukked in",
                                                                         fdProv.DataFileRelativePath.ToLower()));
                                }
                                mXukedInFilDataProviderPaths.Add(fdProv.DataFileRelativePath.ToLower());
                            }
                            if (uid == null || uid == "")
                            {
                                throw new exception.XukException("uid attribute of mDataProviderItem element is missing");
                            }
                            if (IsManagerOf(uid))
                            {
                                if (GetDataProvider(uid) != prov)
                                {
                                    throw new exception.XukException(
                                        String.Format("Another DataProvider exists in the manager with uid {0}", uid));
                                }
                            }
                            else
                            {
                                SetDataProviderUid(prov, uid);
                            }
                            addedProvider = true;
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

        /// <summary>
        /// Writes the attributes of a DataProviderManager element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            Uri presBaseUri = Presentation.RootUri;
            Uri dfdUri = new Uri(presBaseUri, DataFileDirectory);
            destination.WriteAttributeString(XukStrings.DataFileDirectoryPath, presBaseUri.MakeRelativeUri(dfdUri).ToString());
            base.XukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Write the child elements of a DataProviderManager element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteStartElement(XukStrings.DataProviders, XukNamespaceUri);
            }
            foreach (DataProvider prov in ListOfDataProviders)
            {
                if (Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteStartElement(XukStrings.DataProviderItem, XukNamespaceUri);
                    destination.WriteAttributeString(XukStrings.Uid, prov.Uid);
                }

                prov.XukOut(destination, baseUri, handler);

                if (Presentation.Project.IsPrettyFormat())
                {
                    destination.WriteEndElement();
                }
            }
            if (Presentation.Project.IsPrettyFormat())
            {
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<IDataProviderManager> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        /// <remarks>The base path of the <see cref="DataProviderManager"/>s are not compared</remarks>
        public bool ValueEquals(DataProviderManager other)
        {
            if (other == null) return false;
            DataProviderManager o = other;
            if (o.DataFileDirectory != DataFileDirectory) return false;
            List<DataProvider> oDP = ListOfDataProviders;
            if (o.ListOfDataProviders.Count != oDP.Count) return false;
            foreach (DataProvider dp in oDP)
            {
                string uid = dp.Uid;
                if (!o.IsManagerOf(uid)) return false;
                if (!o.GetDataProvider(uid).ValueEquals(dp)) return false;
            }
            return true;
        }

        #endregion
    }
}