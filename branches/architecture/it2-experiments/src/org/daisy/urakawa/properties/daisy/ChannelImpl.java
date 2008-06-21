package org.daisy.urakawa.properties.daisy;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;

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
 * @see org.daisy.urakawa.properties.daisy.Channel
 */
public class ChannelImpl implements Channel {
    /**
     * @hidden
     */
    public String getLang() {
        return null;
    }

    /**
     * @hidden
     */
    public String getName() {
        return null;
    }

    /**
     * @hidden
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public boolean isMediaTypeSupported(MediaType mediaType) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean addSupportedMediaType(MediaType mediaType) {
        return false;
    }
}