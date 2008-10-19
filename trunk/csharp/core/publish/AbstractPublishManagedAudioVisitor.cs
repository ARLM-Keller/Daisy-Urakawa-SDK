using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;
using urakawa.xuk;

namespace urakawa.publish
{
    /// <summary>
    /// An abstract <see cref="ITreeNodeVisitor"/> that publishes <see cref="ManagedAudioMedia"/> 
    /// from a source <see cref="Channel"/> to a destination <see cref="Channel"/> as <see cref="ExternalAudioMedia"/>.
    /// In concrete implementations of the abstract visitor, 
    /// methods <see cref="TreeNodeTriggersNewAudioFile"/> and <see cref="TreeNodeMustBeSkipped"/> 
    /// must be implemented to control which <see cref="TreeNode"/>s trigger the generation of a new audio file
    /// and which <see cref="TreeNode"/>s are skipped.
    /// After visitation the <see cref="WriteCurrentAudioFile"/> method must be called to ensure that
    /// the current audio file is written to disk.
    /// </summary>
    public abstract class AbstractPublishManagedAudioVisitor : ITreeNodeVisitor
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected AbstractPublishManagedAudioVisitor()
        {
            ResetAudioFileNumbering();
        }

        private Channel mSourceChannel;
        private Channel mDestinationChannel;
        private Uri mDestinationDirectory;
        private string mAudioFileBaseNameFormat = "aud{0:0}.wav";
        private int mCurrentAudioFileNumber;
        private PCMFormatInfo mCurrentAudioFilePCMFormat = null;
        private Stream mCurrentAudioFileStream = null;

        /// <summary>
        /// Gets the source <see cref="Channel"/> from which the <see cref="ManagedAudioMedia"/> to publish is retrieved
        /// </summary>
        /// <returns>The source channel</returns>
        public Channel SourceChannel
        {
            get
            {
                if (mSourceChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a source Channel");
                }
                return mSourceChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The source Channel can not be null");
                mSourceChannel = value;
            }
        }

        /// <summary>
        /// Gets the destination <see cref="Channel"/> to which the published audio is added as <see cref="ExternalAudioMedia"/>
        /// </summary>
        /// <returns>The destination channel</returns>
        public Channel DestinationChannel
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not been inistalized with a destination Channel");
                }
                return mDestinationChannel;
            }
            set
            {
                if (value == null)
                    throw new exception.MethodParameterIsNullException("The destination Channel can not be null");
                mDestinationChannel = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="Uri"/> of the destination directory in which the published audio files are created
        /// </summary>
        /// <returns>The destination directory <see cref="Uri"/></returns>
        public Uri DestinationDirectory
        {
            get
            {
                if (mDestinationChannel == null)
                {
                    throw new exception.IsNotInitializedException(
                        "The AbstractPublishManagedAudioVisitor has not ben initialized with a destination directory Uri");
                }
                return mDestinationDirectory;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        "The Uri of the destination directory can not be null");
                }
                mDestinationDirectory = value;
            }
        }

        /// <summary>
        /// Gets the format of the name of the published audio files - format parameter 0 is the number of the audio file (1, 2, ...)
        /// </summary>
        /// <returns>The audio file name format</returns>
        public string AudioFileNameFormat
        {
            get { return mAudioFileBaseNameFormat; }
        }

        /// <summary>
        /// Gets the number of the current audio file
        /// </summary>
        /// <returns>The current audio file number</returns>
        public int CurrentAudioFileNumber
        {
            get { return mCurrentAudioFileNumber; }
        }

        /// <summary>
        /// Resets the audio file numbering, setting the current audio file number to 0. 
        /// </summary>
        public void ResetAudioFileNumbering()
        {
            mCurrentAudioFileNumber = 0;
        }


        /// <summary>
        /// Controls when new audio files are created. In concrete implementations,
        /// if this method returns <c>true</c> for a given <see cref="TreeNode"/>, 
        /// this <see cref="TreeNode"/> triggers the creation of a new audio file
        /// </summary>
        /// <param name="node">The given node</param>
        /// <returns>A <see cref="bool"/> indicating if the given node triggers a new audio file</returns>
        public abstract bool TreeNodeTriggersNewAudioFile(TreeNode node);

        /// <summary>
        /// Controls what <see cref="TreeNode"/> are skipped during publish visitation
        /// </summary>
        /// <param name="node">A <see cref="TreeNode"/> to test</param>
        /// <returns>A <see cref="bool"/> indicating if the <see cref="TreeNode"/> should be skipped</returns>
        public abstract bool TreeNodeMustBeSkipped(TreeNode node);


        /// <summary>
        /// Writes the curently active audio file to disk.
        /// </summary>
        public void WriteCurrentAudioFile()
        {
            if (mCurrentAudioFileStream != null && mCurrentAudioFilePCMFormat != null)
            {
                Uri file = GetCurrentAudioFileUri();
                FileStream fs = new FileStream(
                    file.LocalPath,
                    FileMode.Create, FileAccess.Write, FileShare.Read);
                try
                {
                    PCMDataInfo pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
                    pcmData.DataLength = (uint) mCurrentAudioFileStream.Length;
                    pcmData.WriteRiffWaveHeader(fs);
                    mCurrentAudioFileStream.Position = 0;
                    BinaryReader rd = new BinaryReader(mCurrentAudioFileStream);
                    byte[] data = rd.ReadBytes((int) mCurrentAudioFileStream.Length);
                    rd.Close();
                    fs.Write(data, 0, data.Length);
                    mCurrentAudioFileStream = null;
                    mCurrentAudioFilePCMFormat = null;
                }
                finally
                {
                    fs.Close();
                }
            }
        }

        private Uri GetCurrentAudioFileUri()
        {
            Uri res = DestinationDirectory;
            res = new Uri(res, String.Format(AudioFileNameFormat, CurrentAudioFileNumber));
            return res;
        }

        private void CreateNextAudioFile()
        {
            WriteCurrentAudioFile();
            mCurrentAudioFileNumber++;
            mCurrentAudioFileStream = new MemoryStream();
        }

        #region ITreeNodeVisitor Members

        /// <summary>
        /// The pre-visit method does the business logic of publishing the managed audio 
        /// from the source to the destination <see cref="Channel"/>
        /// </summary>
        /// <param name="node">The node being visited</param>
        /// <returns>A <see cref="bool"/> indicating if the children of <paramref name="node"/> should be visited as well</returns>
        public virtual bool PreVisit(TreeNode node)
        {
            if (TreeNodeMustBeSkipped(node)) return false;
            if (TreeNodeTriggersNewAudioFile(node)) CreateNextAudioFile();
            if (node.HasProperties(typeof (ChannelsProperty)))
            {
                ChannelsProperty chProp = node.GetProperty<ChannelsProperty>();
                if (chProp.GetMedia(DestinationChannel) != null) chProp.SetMedia(DestinationChannel, null);
                ManagedAudioMedia mam = chProp.GetMedia(SourceChannel) as ManagedAudioMedia;
                if (mam != null)
                {
                    AudioMediaData amd = mam.AudioMediaData;
                    if (mCurrentAudioFilePCMFormat == null)
                    {
                        mCurrentAudioFilePCMFormat = amd.PCMFormat;
                    }
                    if (mCurrentAudioFileStream == null || !mCurrentAudioFilePCMFormat.ValueEquals(amd.PCMFormat))
                    {
                        CreateNextAudioFile();
                        mCurrentAudioFilePCMFormat = amd.PCMFormat;
                    }
                    BinaryReader rd = new BinaryReader(amd.GetAudioData());
                    Time clipBegin =
                        Time.Zero.AddTimeDelta(
                            mCurrentAudioFilePCMFormat.GetDuration((uint) mCurrentAudioFileStream.Position));
                    Time clipEnd = clipBegin.AddTimeDelta(amd.AudioDuration);
                    try
                    {
                        byte[] data = rd.ReadBytes(amd.GetPCMLength());
                        mCurrentAudioFileStream.Write(data, 0, data.Length);
                    }
                    finally
                    {
                        rd.Close();
                    }
                    ExternalAudioMedia eam = node.Presentation.MediaFactory.Create<ExternalAudioMedia>();
                    eam.Language = mam.Language;
                    eam.Src = node.Presentation.RootUri.MakeRelativeUri(GetCurrentAudioFileUri()).ToString();
                    eam.ClipBegin = clipBegin;
                    eam.ClipEnd = clipEnd;
                    chProp.SetMedia(mDestinationChannel, eam);
                }
            }
            return true;
        }

        /// <summary>
        /// Nothing is done in in post-visit
        /// </summary>
        /// <param name="node">The node</param>
        public virtual void PostVisit(TreeNode node)
        {
            //Nothing is done in PostVisit visit
        }

        #endregion
    }
}