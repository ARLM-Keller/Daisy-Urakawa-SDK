using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace urakawa.media.data
{
    [TestFixture]
    public class FileDataProviderManagerTests : IDataProviderManagerTests
    {
        protected FileDataProviderManager mFileDataProviderManager
        {
            get { return mManager as FileDataProviderManager; }
        }

        [SetUp]
        public void SetUp()
        {
            Project proj = new Project();
            proj.AddNewPresentation();
            mManager = proj.GetPresentation(0).DataProviderManager;
        }

        [Test]
        public void temp()
        {
            FileDataProvider fdp = mFileDataProviderManager.getDataProviderFactory().createFileDataProvider(
                FileDataProviderFactory.AUDIO_WAV_MIME_TYPE);
            Assert.IsNotNull(fdp, "Could not create FileDataProvider");
        }
    }
}
