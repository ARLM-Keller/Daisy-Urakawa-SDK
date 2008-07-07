using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using urakawa.events;
using urakawa.events.media.data;

namespace urakawa.media.data
{
    /// <summary>
    /// Abstract implementation of <see cref="MediaData"/> that provides the common functionality 
    /// needed by any implementation of <see cref="MediaData"/>
    /// </summary>
    public abstract class MediaData : WithPresentation, IValueEquatable<MediaData>, IChangeNotifier
    {
        #region Event related members

        /// <summary>
        /// Event fired after the <see cref="MediaData"/> has changed. 
        /// The event fire before any change specific event 
        /// </summary>
        public event EventHandler<urakawa.events.DataModelChangedEventArgs> Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event 
        /// </summary>
        /// <param name="args">The arguments of the event</param>
        protected void NotifyChanged(urakawa.events.DataModelChangedEventArgs args)
        {
            EventHandler<urakawa.events.DataModelChangedEventArgs> d = Changed;
            if (d != null) d(this, args);
        }

        /// <summary>
        /// Event fired after the name of the <see cref="IMedia"/> has changed
        /// </summary>
        public event EventHandler<NameChangedEventArgs> NameChanged;

        /// <summary>
        /// Fires the <see cref="NameChanged"/> event
        /// </summary>
        /// <param name="source">The source, that is the <see cref="MediaData"/> whoose name has changed</param>
        /// <param name="newName">The new name</param>
        /// <param name="prevName">The name prior to the change</param>
        protected void NotifyNameChanged(MediaData source, string newName, string prevName)
        {
            EventHandler<NameChangedEventArgs> d = NameChanged;
            if (d != null) d(this, new NameChangedEventArgs(source, newName, prevName));
        }

        private void this_nameChanged(object sender, NameChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public MediaData()
        {
            this.NameChanged += new EventHandler<NameChangedEventArgs>(this_nameChanged);
        }

        /// <summary>
        /// Gets the <see cref="MediaDataManager"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The assicoated <see cref="MediaDataManager"/></returns>
        public MediaDataManager MediaDataManager
        {
            get { return Presentation.MediaDataManager; }
        }

        /// <summary>
        /// Gets the UID of <c>this</c>.
        /// Convenience for <c><see cref="MediaData.MediaDataManager"/>.<see cref="urakawa.media.data.MediaDataManager.GetUidOfMediaData"/>(this)</c>
        /// </summary>
        /// <returns>The UID</returns>
        public string Uid
        {
            get { return MediaDataManager.GetUidOfMediaData(this); }
        }

        private string mName = "";

        /// <summary>
        /// Gets the name of <c>this</c>
        /// </summary>
        /// <returns>The name</returns>
        public string Name
        {
            get { return mName; }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException("The name of an MediaData can not be null");
                }
                string prevName = mName;
                mName = value;
                NotifyNameChanged(this, value, prevName);
            }
        }

        /// <summary>
        /// Gets a <see cref="List{IDataProvider}"/> of the <see cref="IDataProvider"/>s used by <c>this</c>
        /// </summary>
        /// <returns>The <see cref="List{IDataProvider}"/></returns>
        public abstract List<IDataProvider> ListOfUsedDataProviders { get; }

        /// <summary>
        /// Deletes the <see cref="MediaData"/>, detaching it from it's manager and releasing 
        /// any <see cref="IDataProvider"/>s used
        /// </summary>
        public virtual void Delete()
        {
            MediaDataManager.RemoveMediaData(this);
        }

        /// <summary>
        /// Part of technical solution to make copy method return correct type. 
        /// In implementing classes this method should return a copy of the class instances
        /// </summary>
        /// <returns>The copy</returns>
        protected abstract MediaData ProtectedCopy();

        /// <summary>
        /// Creates a copy of the media data
        /// </summary>
        /// <returns>The copy</returns>
        public MediaData Copy()
        {
            return ProtectedCopy();
        }

        /// <summary>
        /// Part of technical solution to make export method return correct type. 
        /// In implementing classes this method should return a export of the class instances
        /// </summary>
        /// <param name="destPres">The destination presentation of the export</param>
        /// <returns>The export</returns>
        protected abstract MediaData ProtectedExport(Presentation destPres);

        /// <summary>
        /// Exports the media data to a given destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The given destination presentation</param>
        /// <returns>The exported media data</returns>
        public MediaData Export(Presentation destPres)
        {
            return ProtectedExport(destPres);
        }

        #region IValueEquatable<MediaData> Members

        /// <summary>
        /// Determines of <c>this</c> has the same value as a given other instance
        /// </summary>
        /// <param name="other">The other instance</param>
        /// <returns>A <see cref="bool"/> indicating the result</returns>
        public virtual bool ValueEquals(MediaData other)
        {
            if (other == null) return false;
            if (GetType() != other.GetType()) return false;
            if (Name != other.Name) return false;
            return true;
        }

        #endregion
    }
}