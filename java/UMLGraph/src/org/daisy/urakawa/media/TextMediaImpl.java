package org.daisy.urakawa.media;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.exceptions.FactoryIsMissingException;

/**
 * No external storage, uses internal memory.
 * Which means XukAble must be implemented to include the actual text data in the XML result.
 */
public class TextMediaImpl implements TextMedia {
    /**
     * @hidden
     */
    public String getText() {
        return null;
    }

    /**
     * @hidden
     */
    public void setText(String text) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public MediaFactory getMediaFactory() {
        return null;
    }

    /**
     * @hidden
     */
    public void setMediaFactory(MediaFactory fact) {
    }

    /**
     * @hidden
     */
    public boolean isContinuous() {
        return false;
    }

    /**
     * @hidden
     */
    public boolean isDiscrete() {
        return false;
    }

    /**
     * @hidden
     */
    public MediaType getMediaType() {
        return null;
    }

    /**
     * @hidden
     */
    public Media copy() throws FactoryIsMissingException {
        return null;
    }

    /**
     * @hidden
     */
    public boolean XukIn(XmlDataReader source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XukOut(XmlDataWriter destination) {
        return false;
    }

    /**
     * @hidden
     */
    public String getXukLocalName() {
        return null;
    }

    /**
     * @hidden
     */
    public String getXukNamespaceURI() {
        return null;
    }

    /**
     * @hidden
     */

	public boolean isSequence() {

		return false;
	}

    /**
     * @hidden
     */

	public boolean ValueEquals(Media other)
			throws MethodParameterIsNullException {

		return false;
	}
}
