using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.media.data;
using NUnit.Framework;

namespace urakawa.unitTests.mediaDataTests
{
	/// <summary>
	/// Base clas for fixtures testing functionality in the <see cref="urakawa.media.data"/> namespace
	/// </summary>
	public class MediaDataTestBase : testbase.TestCollectionBase
	{
		protected string mCopyDirectory;

		public override void Init()
		{
			base.Init();
			DeleteCopyDirectory();
		}

		protected void DeleteCopyDirectory()
		{
			if (Directory.Exists(mCopyDirectory))
			{
				try
				{
					Directory.Delete(mCopyDirectory, true);
				}
				catch (Exception e)
				{
					Assert.Fail("Could not delete directory {0}: {1}\n{2}", mCopyDirectory, e.Message, e.StackTrace);
				}
			}
		}

		[TestFixtureSetUp]
		public void InitFixture()
		{
			mDefaultFile = "../../XukWorks/MediaDataSample/MediaDataSample.xuk";
			mCopyDirectory = "../../XukWorks/MediaDataSampleCopy";
		}

		[TearDown]
		public void Terminate()
		{
			Init();
			//Delete any files in the data directory not used by the FileDataProviderManager
			FileDataProviderManager dataProvMngr = (FileDataProviderManager)mProject.getPresentation().getDataProviderManager();
			DirectoryInfo dataDI = new DirectoryInfo(dataProvMngr.getDataFileDirectoryFullPath());
			foreach (FileInfo file in dataDI.GetFiles())
			{
				bool found = false;
				foreach (FileDataProvider fileDataProv in dataProvMngr.getListOfDataProviders())
				{
					if (file.FullName.ToLower() == fileDataProv.getDataFileFullPath().ToLower())
					{
						found = true;
						break;
					}
				}
				if (!found) file.Delete();
			}
			DeleteCopyDirectory();
		}
	}
}