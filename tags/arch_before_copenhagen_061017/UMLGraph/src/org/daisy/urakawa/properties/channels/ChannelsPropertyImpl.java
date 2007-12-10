package org.daisy.urakawa.properties.channels;

import org.daisy.urakawa.coreTree.CoreNode;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MediaTypeIsIllegalException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.properties.PropertyType;

import java.util.List;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * Generally speaking, an end-user would not need to use this class directly.
 * They would just manipulate the corresponding abstract interface and use the provided
 * default factory implementation to create this class instances transparently.
 * -
 * However, this is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such (it's public after all),
 * or they can sub-class it in order to specialize their application.
 */
public class ChannelsPropertyImpl implements ChannelsProperty, ChannelsPropertyValidator {
    /**
     * @hidden
     */
    public Media getMedia(Channel channel) throws MethodParameterIsNullException, ChannelDoesNotExistException {
        return null;
    }

    /**
     * @hidden
     */
    public void setMedia(Channel channel, Media media) throws MethodParameterIsNullException, ChannelDoesNotExistException, MediaTypeIsIllegalException {
    }

    /**
     * @hidden
     */
    public List getListOfUsedChannels() {
        return null;
    }

    /**
     * @hidden
     */
    public PropertyType getType() {
        return null;
    }

    /**
     * @hidden
     */
    public void setType(PropertyType type) {
    }

    /**
     * @hidden
     */
    public CoreNode getOwner() {
        return null;
    }

    /**
     * @hidden
     */
    public void setOwner(CoreNode newOwner) {
    }

    /**
     * @hidden
     */
    public ChannelsPropertyImpl copy() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean canSetMedia(Channel channel, Media media) throws MethodParameterIsNullException, ChannelDoesNotExistException, MediaTypeIsIllegalException {
        return false;
    }
}