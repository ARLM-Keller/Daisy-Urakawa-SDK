package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.XmlDataReader;
import org.daisy.urakawa.XmlDataWriter;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 */
public class XmlAttributeImpl implements XmlAttribute {
    /**
     * @hidden
     */
    public XmlProperty getParent() {
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
    public String getNamespace() {
        return null;
    }

    /**
     * @hidden
     */
    public void setName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
    }

    /**
     * @hidden
     */
    public void setNamespace(String newNS) throws MethodParameterIsNullException {
    }

    /**
     * @hidden
     */
    public XmlAttribute copy() {
        return null;
    }

    /**
     * @hidden
     */
    public String getValue() {
        return null;
    }

    /**
     * @hidden
     */
    public void setValue(String newValue) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
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
}
