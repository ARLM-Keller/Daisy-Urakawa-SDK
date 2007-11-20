using System;
using System.Collections.Generic;
using System.Text;
using urakawa.property.channel;
using urakawa.media;

namespace urakawa.events
{
	public class ChannelMediaMapEvent : ChannelsPropertyEvent
	{
		public ChannelMediaMapEvent(ChannelsProperty src, Channel destCh, IMedia mapdMedia, IMedia prevMedia) : base(src)
		{
			DestinationChannel = destCh;
			MappedMedia = mapdMedia;
			PreviousMedia = prevMedia;
		}
		public readonly Channel DestinationChannel;
		public readonly IMedia MappedMedia;
		public readonly IMedia PreviousMedia;
	}
}
