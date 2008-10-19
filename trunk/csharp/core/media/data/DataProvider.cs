using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa.media.data

{
    /// <summary>
    /// Interface for a generic <see cref="DataProvider"/> providing access to data storage 
    /// via input and output <see cref="Stream"/>s
    /// </summary>
    public abstract class DataProvider : WithPresentation, IValueEquatable<DataProvider>
    {

        private string mMimeType;

        private void Reset()
        {
            mMimeType = null;
           
        }

        /// <summary>
        /// Default constructor - for system use only, 
        /// <see cref="DataProvider"/>s should only be created via. the <see cref="DataProviderFactory"/>
        /// </summary>
        protected DataProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the <see cref="DataProviderManager"/> associated with <c>this</c>
        /// - convenience for <c>Presentation.DataProviderManager</c>
        /// </summary>
        public DataProviderManager DataProviderManager { get { return Presentation.DataProviderManager;}}

        /// <summary>
        /// Gets the UID of the data provider in the context of the manager. 
        /// Convenience for <c>DataProviderManager.GetUidOfDataProvider(this)</c>
        /// </summary>
        public string Uid
        {
            get { return DataProviderManager.GetUidOfDataProvider(this); }
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> providing read access to the data
        /// </summary>
        /// <returns>The input <see cref="Stream"/></returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <remarks>
        /// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
        /// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
        /// <see cref="GetOutputStream"/> and <see cref="Delete"/> will cause <see cref="exception.InputStreamsOpenException"/>s
        /// </remarks>
        public abstract Stream GetInputStream();

        /// <summary>
        /// Gets a <see cref="Stream"/> providing write access to the data
        /// </summary>
        /// <returns>The output <see cref="Stream"/></returns>
        /// <exception cref="exception.DataMissingException">
        /// Thrown if the data stored in the <see cref="DataProvider"/> is missing from the underlying storage mechanism
        /// </exception>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if another output <see cref="Stream"/> from the data provider is already/still open
        /// </exception>
        /// <remarks>
        /// Make sure to close any <see cref="Stream"/> returned by this method when it is no longer needed. 
        /// If there are any open input <see cref="Stream"/>s, subsequent calls to methods
        /// <see cref="GetOutputStream"/> and <see cref="GetInputStream"/> and <see cref="Delete"/> 
        /// will cause <see cref="exception.OutputStreamOpenException"/>s
        /// </remarks>
        public abstract Stream GetOutputStream();

        /// <summary>
        /// Deletes any resources associated with <c>this</c> permanently. Additionally removes the <see cref="DataProvider"/>
        /// from it's <see cref="DataProviderManager"/>
        /// </summary>
        /// <exception cref="exception.OutputStreamOpenException">
        /// Thrown if a output <see cref="Stream"/> from the <see cref="DataProvider"/> is currently open
        /// </exception>
        /// <exception cref="exception.InputStreamsOpenException">
        /// Thrown if one or more input <see cref="Stream"/>s from the <see cref="DataProvider"/> are currently open
        /// </exception>
        public abstract void Delete();

        /// <summary>
        /// Creates a copy of <c>this</c> including a copy of the data
        /// </summary>
        /// <returns>The copy</returns>
        public abstract DataProvider Copy();

        /// <summary>
        /// Exports <c>this</c> to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination <see cref="Presentation"/></param>
        /// <returns>The exported <see cref="DataProvider"/></returns>
        public abstract DataProvider Export(Presentation destPres);

        /// <summary>
        /// Gets or sets the MIME type of the media stored in the data provider
        /// </summary>
        /// <exception cref="IsNotInitializedException">
        /// Thrown when trying to get the <see cref="MimeType"/> before it has been initialized
        /// </exception>
        /// <exception cref="MethodParameterIsNullException">
        /// Thrown when trying to set the <see cref="MimeType"/> to <c>null</c>
        /// </exception>
        public string MimeType
        {
            get
            {
                if (mMimeType == null) throw new IsNotInitializedException("The DataProvider has not been initialized with a MimeType");
                return mMimeType;
            }

            set
            {
                if (value == null)
                {
                    throw new MethodParameterIsNullException("The MimeType cannot be null");
                }
                mMimeType = value;
            }
        }

        #region IXukAble members

        /// <summary>
        /// Clears the <see cref="XukAble"/> of any data - called at the beginning of <see cref="XukAble.XukIn"/>
        /// </summary>
        protected override void Clear()
        {
            Reset();
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a XukAble xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            MimeType = source.GetAttribute("mimeType") ?? "";
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Writes the attributes of a XukAble element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("mimeType", MimeType);
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion


        #region IValueEquatable<DataProvider> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public abstract bool ValueEquals(DataProvider other);

        #endregion
    }
}