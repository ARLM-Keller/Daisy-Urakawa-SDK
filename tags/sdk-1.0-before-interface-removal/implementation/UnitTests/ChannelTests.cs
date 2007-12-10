using System;
using NUnit.Framework;
using urakawa.core;
using urakawa.properties.channel;
using urakawa.examples;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Summary description for ChannelTests.
	/// </summary>
	public class ChannelTests : TestCollectionBase
	{

		/// <summary>
		/// Checks that the removal of a channels disassociates the media in this channel
		/// from the core nodes.
		/// </summary>
		/// <remarks>Assumes that the file loaded into the presentation has a channel 
		/// with id c1 and that at least on piece of media is attached to that channel</remarks>
		public void RemoveChannel()
		{
			IChannel c1Channel = mProject.getPresentation().getChannelsManager().getChannel("c1");
			DetectMediaCoreNodeVisitor detVis = new DetectMediaCoreNodeVisitor(c1Channel);
			mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
			Assert.IsTrue(
				detVis.hasFoundMedia(),
				"The channel with id \"c1\" must contain media or the test will be meaningless");
			mProject.getPresentation().getChannelsManager().detachChannel(c1Channel);
			mProject.getPresentation().getChannelsManager().addChannel(c1Channel);
			detVis.reset();
			mProject.getPresentation().getRootNode().acceptDepthFirst(detVis);
			Assert.IsFalse(
				detVis.hasFoundMedia(), 
				"Found media in channel that was removed and re-added");
		}
	}
}