using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;
using urakawa.unitTests.testbase;
using System.IO;

namespace urakawa.unitTests.fixtures.xukfiles

{
	/// <summary>
	/// Tests for opening and saving XUK files
	/// </summary>
	/// 
	[TestFixture] public class XUKOpen
	{
		protected string mDefaultFile = @"..\XUKWorks\simplesample.xuk";

		/// <summary>
		/// Tests opening of XUK files
		/// </summary>
		[Test] public void OpenXUK()
		{
			Project proj;
			OpenXUK(out proj, mDefaultFile);
		}

		private void OpenXUK(out Project proj, string file)
		{
			proj = new Project();
			proj.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(proj_changed);
			
			string filepath = Directory.GetCurrentDirectory();

			Uri fileUri = new Uri(filepath);
			
			fileUri = new Uri(fileUri, file);
			proj.OpenXuk(fileUri);
		}

		void proj_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(String.Format("Changed event from {0}:\n\t{1}", sender, e.ToString().Replace("\n", "\n\t")));
		}

		[Test] public void DeleteChannel()
		{
			Project proj;
			OpenXUK(out proj, mDefaultFile);
			ChannelsManager chMgr = proj.GetPresentation(0).ChannelsManager;
			Channel ch = (Channel)chMgr.getListOfChannels()[0];
			chMgr.removeChannel(ch);
			chMgr.addChannel(ch);
			urakawa.examples.CollectMediaFromChannelTreeNodeVisitor collVis
				= new urakawa.examples.CollectMediaFromChannelTreeNodeVisitor(ch);
			proj.GetPresentation(0).RootNode.AcceptDepthFirst(collVis);
			Assert.AreEqual(
				0, collVis.CollectedMedia.Length, 
				"The channel unexpectedly contained media after being deleted and re-added");

		}


    		
	}
}