package org.daisy.urakawa.media;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.media.ExternalAudioMedia
 */
public final class MediaFactory extends
        GenericWithPresentationFactory<AbstractMedia>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public MediaFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }

    /**
     * @return
     */
    public ExternalAudioMedia createExternalAudioMedia()
    {
        try
        {
            return create(ExternalAudioMedia.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
