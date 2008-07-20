using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.progress;
using urakawa.xuk;

namespace urakawa.media
{
    /// <summary>
    /// SequenceMedia is a collection of same-type media objects
    /// The first object in the collection determines the collection's type.
    /// </summary>
    public class SequenceMedia : AbstractMedia
    {
        #region Event related members

        private void Item_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            NotifyChanged(e);
        }

        #endregion

        private List<IMedia> mSequence;
        private bool mAllowMultipleTypes;

        /// <summary>
        /// Constructor setting the associated <see cref="IMediaFactory"/>
        /// </summary>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when <paramref localName="fact"/> is <c>null</c>
        /// </exception>
        protected internal SequenceMedia() : base()
        {
            mSequence = new List<IMedia>();
            mAllowMultipleTypes = false;
        }

        /// <summary>
        /// Get the item at the given index
        /// </summary>
        /// <param name="index">Index of the item to return</param>
        /// <returns>The <see cref="IMedia"/> item at the given index</returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given index is out of bounds
        /// </exception>
        public IMedia GetItem(int index)
        {
            if (0 <= index && index < Count)
            {
                return (IMedia) mSequence[index];
            }
            else
            {
                throw new exception.MethodParameterIsOutOfBoundsException("SequenceMedia.GetItem(" +
                                                                          index.ToString() +
                                                                          ") caused MethodParameterIsOutOfBoundsException");
            }
        }


        /// <summary>
        /// Inserts a given <see cref="IMedia"/> item at a given index
        /// </summary>
        /// <param name="index">The given index</param>
        /// <param name="newItem">The given <see cref="IMedia"/> item</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given index is out of bounds
        /// </exception>
        /// <exception cref="exception.MediaNotAcceptable">
        ///	Thrown if the <see cref="SequenceMedia"/> can not accept the media
        /// </exception>
        public void InsertItem(int index, IMedia newItem)
        {
            if (newItem == null)
            {
                throw new exception.MethodParameterIsNullException("The new item can not be null");
            }
            if (index < 0 || Count < index)
            {
                throw new exception.MethodParameterIsOutOfBoundsException(
                    "The index at which to insert media is out of bounds");
            }
            if (!CanAcceptMedia(newItem))
            {
                throw new exception.MediaNotAcceptable(
                    "The new media to insert is of a type that is incompatible with the sequence media");
            }
            mSequence.Insert(index, newItem);
            newItem.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Item_changed);
        }

        /// <summary>
        /// Appends a new <see cref="IMedia"/> item to the end of the sequence
        /// </summary>
        /// <param name="newItem">The new item</param>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the given <see cref="IMedia"/> to insert is <c>null</c>
        /// </exception>
        /// <exception cref="exception.MediaNotAcceptable">
        ///	Thrown if the <see cref="SequenceMedia"/> can not accept the media
        /// </exception>
        public void AppendItem(IMedia newItem)
        {
            InsertItem(Count, newItem);
        }

        /// <summary>
        /// Remove an item from the sequence.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The removed <see cref="IMedia"/> item</returns>
        /// <exception cref="exception.MethodParameterIsOutOfBoundsException">
        /// Thrown when the given index is out of bounds
        /// </exception>
        public IMedia RemoveItem(int index)
        {
            IMedia removedMedia = GetItem(index);
            RemoveItem(removedMedia);
            return removedMedia;
        }

        /// <summary>
        /// Removes a given <see cref="IMedia"/> item from the sequence
        /// </summary>
        /// <param name="item">The item</param>
        /// <exception cref="exception.MediaNotInSequenceException">
        /// Thrown when the given item is not part of the sequence
        /// </exception>
        public void RemoveItem(IMedia item)
        {
            if (!mSequence.Contains(item))
            {
                throw new exception.MediaNotInSequenceException(
                    "Cannot remove a IMedia item that is not part of the sequence");
            }
            mSequence.Remove(item);
            item.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Item_changed);
        }

        /// <summary>
        /// Return the number of items in the sequence.
        /// </summary>
        /// <returns>The number of items</returns>
        public int Count
        {
            get { return mSequence.Count; }
        }

        /// <summary>
        /// Gets a list of the <see cref="IMedia"/> items in the sequence
        /// </summary>
        /// <returns>The list</returns>
        public List<IMedia> ListOfItems
        {
            get { return new List<IMedia>(mSequence); }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating if multiple <see cref="IMedia"/> types are allowed in the sequence
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool AllowMultipleTypes
        {
            get { return mAllowMultipleTypes; }
            set
            {
                if (!value)
                {
                    int count = Count;
                    if (count > 0)
                    {
                        Type firstItemType = GetItem(0).GetType();
                        int i = 1;
                        while (i < count)
                        {
                            if (GetItem(i).GetType() != firstItemType)
                            {
                                throw new exception.OperationNotValidException(
                                    "Can not prohibit multiple IMedia types in the sequence, since the Type of the sequence items differ");
                            }
                        }
                    }
                }
                mAllowMultipleTypes = value;
            }
        }

        #region IMedia Members

        /// <summary>
        /// Use the first item in the collection to determine if this sequence is continuous or not.
        /// </summary>
        /// <returns></returns>
        public override bool IsContinuous
        {
            get
            {
                if (Count > 0)
                {
                    return GetItem(0).IsContinuous;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Use the first item in the collection to determine if this 
        /// sequence is discrete or not.
        /// </summary>
        /// <returns></returns>
        public override bool IsDiscrete
        {
            get
            {
                //use the first item in the collection to determine the value
                if (Count > 0)
                {
                    return GetItem(0).IsDiscrete;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This function always returns true, because this 
        /// object is always considered to be a sequence (even if it contains only one item).
        /// </summary>
        /// <returns><c>true</c></returns>
        public override bool IsSequence
        {
            get { return true; }
        }

        /// <summary>
        /// Make a copy of this media sequence
        /// </summary>
        /// <returns>The copy</returns>
        public new SequenceMedia Copy()
        {
            return CopyProtected() as SequenceMedia;
        }

        /// <summary>
        /// Make a copy of this media sequence
        /// </summary>
        /// <returns>The copy</returns>
        protected override IMedia CopyProtected()
        {
            IMedia newMedia = MediaFactory.CreateMedia(
                XukLocalName, XukNamespaceUri);
            if (!(newMedia is SequenceMedia))
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The media factory can not create an SequenceMedia matching QName {0}:{1}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            SequenceMedia newSeqMedia = (SequenceMedia) newMedia;
            foreach (IMedia item in ListOfItems)
            {
                newSeqMedia.AppendItem(item.Copy());
            }
            return newSeqMedia;
        }

        /// <summary>
        /// Exports the sequence media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported sequence media</returns>
        public new SequenceMedia Export(Presentation destPres)
        {
            return ExportProtected(destPres) as SequenceMedia;
        }

        /// <summary>
        /// Exports the sequence media to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported sequence media</returns>
        protected override IMedia ExportProtected(Presentation destPres)
        {
            SequenceMedia exported = destPres.MediaFactory.CreateMedia(
                                         XukLocalName, XukNamespaceUri) as SequenceMedia;
            if (exported == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The MediaFacotry cannot create a SequenceMedia matching QName {1}:{0}",
                                                                         XukLocalName, XukNamespaceUri));
            }
            foreach (IMedia m in ListOfItems)
            {
                exported.AppendItem(m.Export(destPres));
            }
            return exported;
        }

        #endregion

        /// <summary>
        /// Test a new media object to see if it can belong to this collection 
        /// (optionally a sequence can allow only a single <see cref="Type"/> of <see cref="IMedia"/>)
        /// </summary>
        /// <param name="proposedAddition">The media to test</param>
        /// <returns></returns>
        /// <exception cref="exception.MethodParameterIsNullException">
        /// Thrown when the proposed addition is null
        /// </exception>
        public virtual bool CanAcceptMedia(IMedia proposedAddition)
        {
            if (proposedAddition == null)
            {
                throw new exception.MethodParameterIsNullException(
                    "The proposed addition is null");
            }
            if (Count > 0 && !AllowMultipleTypes)
            {
                if (GetItem(0).GetType() != proposedAddition.GetType()) return false;
            }
            return true;
        }

        #region IXUKAble members

        /// <summary>
        /// Clears/resets the <see cref="SequenceMedia"/> 
        /// </summary>
        protected override void Clear()
        {
            mAllowMultipleTypes = false;
            foreach (IMedia item in ListOfItems)
            {
                RemoveItem(item);
            }
            base.Clear();
        }

        /// <summary>
        /// Reads the attributes of a SequenceMedia xuk element.
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        protected override void XukInAttributes(XmlReader source)
        {
            string val = source.GetAttribute("allowMultipleMediaTypes");
            if (val == "true" || val == "1")
            {
                AllowMultipleTypes = true;
            }
            else
            {
                AllowMultipleTypes = false;
            }
            base.XukInAttributes(source);
        }

        /// <summary>
        /// Reads a child of a SequenceMedia xuk element. 
        /// </summary>
        /// <param name="source">The source <see cref="XmlReader"/></param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            bool readItem = false;
            if (source.NamespaceURI == XukAble.XUK_NS)
            {
                readItem = true;
                switch (source.LocalName)
                {
                    case "mSequence":
                        XukInSequence(source, handler);
                        break;
                    default:
                        readItem = false;
                        break;
                }
            }
            if (!readItem) base.XukIn(source, handler);
        }

        private void XukInSequence(XmlReader source, ProgressHandler handler)
        {
            if (!source.IsEmptyElement)
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        IMedia newMedia = MediaFactory.CreateMedia(source.LocalName, source.NamespaceURI);
                        if (newMedia != null)
                        {
                            newMedia.XukIn(source, handler);
                            if (!CanAcceptMedia(newMedia))
                            {
                                throw new exception.XukException(
                                    String.Format("Media type {0} is not supported by the sequence",
                                                  newMedia.GetType().FullName));
                            }
                            InsertItem(Count, newMedia);
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
        /// Writes the attributes of a SequenceMedia element
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString("allowMultipleMediaTypes", AllowMultipleTypes ? "true" : "false");
            base.XukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Write the child elements of a SequenceMedia element.
        /// </summary>
        /// <param name="destination">The destination <see cref="XmlWriter"/></param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        /// <param name="handler">The handler for progress</param>
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            if (Count > 0)
            {
                destination.WriteStartElement("mSequence", XukAble.XUK_NS);
                for (int i = 0; i < Count; i++)
                {
                    GetItem(i).XukOut(destination, baseUri, handler);
                }
                destination.WriteEndElement();
            }
            base.XukOutChildren(destination, baseUri, handler);
        }

        #endregion

        #region IValueEquatable<IMedia> Members

        /// <summary>
        /// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="IMedia"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(IMedia other)
        {
            if (!base.ValueEquals(other)) return false;
            SequenceMedia otherSeq = (SequenceMedia) other;
            if (Count != otherSeq.Count) return false;
            for (int i = 0; i < Count; i++)
            {
                if (!GetItem(i).ValueEquals(otherSeq.GetItem(i))) return false;
            }
            return true;
        }

        #endregion
    }
}