package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.ChannelAlreadyExistsException;
import org.daisy.urakawa.exceptions.ChannelDoesNotExistException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.properties.daisy.Channel;
import org.daisy.urakawa.properties.daisy.ChannelManager;

import java.util.List;

/**
 * The actual implementation to be implemented by the implementation team ;)
 * All method bodies must be completed for realizing the required business logic.
 * -
 * This is the DEFAULT implementation for the API/Toolkit:
 * end-users should feel free to use this class as such,
 * or they can sub-class it in order to specialize the instance creation process.
 * -
 * In addition, an end-user has the possibility to implement the
 * singleton factory pattern, so that only one instance of the factory
 * is used throughout the application life
 * (by adding a method like "static Factory getFactory()").
 *
 * @see org.daisy.urakawa.properties.daisy.ChannelManager
 */
public class ChannelManagerImpl implements ChannelManager {
    /**
     * @hidden
     */
    public void addChannel(Channel channel) throws MethodParameterIsNullException, ChannelAlreadyExistsException {
    }

    /**
     * @hidden
     */
    public void removeChannel(Channel channel) throws MethodParameterIsNullException, ChannelDoesNotExistException {
    }

    /**
     * @hidden
     */
    public List<Channel> getChannels() {
        return null;
    }
}