﻿using System;
using System.Collections.Generic;
using System.Xml;
using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace urakawa.commands
{
    public class TreeNodeSetManagedAudioMediaCommand : Command
    {
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ManagedAudioMediaInsertDataCommand otherz = other as ManagedAudioMediaInsertDataCommand;
            if (otherz == null)
            {
                return false;
            }

            // TODO: test local equality

            return true;
        }
        public override string GetTypeNameFormatted()
        {
            return XukStrings.TreeNodeSetManagedAudioMediaCommand;
        }

        private TreeNode m_TreeNode;
        public TreeNode TreeNode
        {
            private set { m_TreeNode = value; }
            get { return m_TreeNode; }
        }
        private ManagedAudioMedia m_ManagedAudioMedia;
        public ManagedAudioMedia ManagedAudioMedia
        {
            private set { m_ManagedAudioMedia = value; }
            get { return m_ManagedAudioMedia; }
        }

        public void Init(TreeNode treeNode, ManagedAudioMedia managedMedia)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("TreeNode");
            }
            if (managedMedia == null)
            {
                throw new ArgumentNullException("ManagedAudioMedia");
            }
            if (treeNode.Presentation != managedMedia.Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }
            if (treeNode.Presentation != Presentation)
            {
                throw new NodeInDifferentPresentationException("TreeNode vs ManagedAudioMedia");
            }

            if (!managedMedia.HasActualAudioMediaData)
            {
                throw new ArgumentException("HasActualAudioMediaData");
            }

            if (treeNode.GetManagedAudioMediaOrSequenceMedia() != null)
            {
                throw new ArgumentException("treeNode.GetManagedAudioMediaOrSequenceMedia");
            }

            TreeNode = treeNode;
            ManagedAudioMedia = managedMedia;

            m_UsedMediaData.Add(ManagedAudioMedia.AudioMediaData);

            ShortDescription = "Add new audio";
            LongDescription = "Attach a ManagedAudioMedia to a TreeNode in the AudioChannel via the ChannelsProperty";
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override void Execute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetOrCreateChannelsProperty();
            chProp.SetMedia(audioChannel, m_ManagedAudioMedia);
        }

        public override void UnExecute()
        {
            AudioChannel audioChannel = Presentation.ChannelsManager.GetOrCreateAudioChannel();
            ChannelsProperty chProp = m_TreeNode.GetChannelsProperty();
            chProp.SetMedia(audioChannel, null);
        }

        private List<MediaData> m_UsedMediaData = new List<MediaData>();
        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                return m_UsedMediaData;
            }
        }

        protected override void XukInAttributes(XmlReader source)
        {
            //nothing new here
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, IProgressHandler handler)
        {
            //nothing new here
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            //nothing new here
            base.XukOutAttributes(destination, baseUri);
        }

        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }
    }
}