package org.daisy.urakawa.media;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithLanguage;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * This is the top-most generic interface for a media object. For example, an
 * {@link VideoMedia} type derives this interface by composition of other
 * interfaces of the data model, like {@link Continuous} and {@link Sized}. The
 * actual type (as per multimedia semantics) of the media object is given by the
 * {@link Media#getMediaType()} method, in order to separate the notion of media
 * type from the object-oriented concepts of interface and class.
 * 
 * @depend - Clone - org.daisy.urakawa.media.Media
 * @depend - Aggregation - org.daisy.urakawa.media.MediaType
 * @depend - Aggregation - org.daisy.urakawa.media.MediaFactory
 * @stereotype XukAble
 */
public interface Media extends WithPresentation, WithMediaFactory, WithLanguage, XukAble,
		ValueEquatable<Media> {
	/**
	 * The "continuous" vs "discrete" media type. The
	 * {@link Media#isContinuous()} method always returns the boolean opposite
	 * of {@link Media#isDiscrete()}
	 * 
	 * @return true if this Media is continuous, false if it is discrete.
	 * @see Media#isDiscrete()
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
	 *      Definitions</a>
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
	 *      Definitions</a>
	 */
	public boolean isContinuous();

	/**
	 * The "continuous" vs "discrete" media type. The
	 * {@link Media#isContinuous()} method always returns the boolean opposite
	 * of {@link Media#isDiscrete()}
	 * 
	 * @return true if this Media is discrete, false if continuous.
	 * @see Media#isContinuous()
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/extended-media-object.html#media-Definitions">SMIL
	 *      Definitions</a>
	 * @see <a
	 *      href="http://www.w3.org/TR/SMIL/smil-timing.html#Timing-DiscreteContinuousMedia">SMIL
	 *      Definitions</a>
	 */
	public boolean isDiscrete();

	/**
	 * Tests whether this media is a sequence of other medias.
	 * 
	 * @return true if this media object is actually a sequence of other medias.
	 * @see SequenceMedia
	 */
	public boolean isSequence();

	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	public Media copy();

	/**
	 * @param destPres
	 * @return can return null in case of failure.
	 * @throws FactoryCannotCreateTypeException
	 * @tagvalue Exceptions "FactoryCannotCreateType-MethodParameterIsNull"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 */
	public Media export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException;
}