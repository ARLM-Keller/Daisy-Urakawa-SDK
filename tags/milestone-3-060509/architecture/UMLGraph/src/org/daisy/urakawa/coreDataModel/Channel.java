package org.daisy.urakawa.coreDataModel;

import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.media.MediaType;

/**
 * The "name" of a Channel is purely informative,
 * and is not to be considered as a way of uniquely identifying a Channel instance.
 * @depend - - - MediaType
 */
public interface Channel {
    /**
     * @param name cannot be null, cannot be empty String
     * @tagvalue Exceptions "MethodParameterIsNull, MethodParameterIsEmptyString"
     */
    public void setName(String name) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

    /**
     * @return cannot return null or empty string, by contract.
     */
    public String getName();

    /**
     * @param mediaType
     * @return true if the media type if supported for this channel.
     * @see org.daisy.urakawa.exceptions.MediaTypeIsIllegalException
     */
    public boolean isMediaTypeSupported(MediaType mediaType);
}