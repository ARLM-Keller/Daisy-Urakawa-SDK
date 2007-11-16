package org.daisy.urakawa.property.channel;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithLanguage;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.xuk.XukAble;

/**
 * The "name" of a Channel is purely informative, and is not to be considered as
 * a way of uniquely identifying a Channel instance.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1..n org.daisy.urakawa.media.MediaType
 * @depend - Aggregation 1 org.daisy.urakawa.property.channel.ChannelsManager
 * @stereotype XukAble
 */
public interface Channel extends WithPresentation, WithChannelsManager, WithName, WithLanguage,
		XukAble, ValueEquatable<Channel> {
	/**
	 * @return convenience method that delegates to ChannelsManager.
	 * @see ChannelsManager#getUidOfChannel(Channel)
	 * @stereotype Convenience
	 */
	public String getUid();

	public boolean isEquivalentTo(Channel otherChannel);

	/**
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @param destPres
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 */
	public Channel export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException;

	/**
	 * Tests whether the given Media obejct is accepted by this Channel
	 * 
	 * @param media
	 * @return
	 * @see org.daisy.urakawa.media.DoesNotAcceptMediaException
	 */
	public boolean canAccept(Media media);
}