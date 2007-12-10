package org.daisy.urakawa.properties.xml;

import org.daisy.urakawa.coreTree.CoreNode;
import org.daisy.urakawa.exceptions.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.project.XUKAble;
import org.daisy.urakawa.properties.PropertyType;

import java.net.URI;
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
public class XMLPropertyImpl implements XMLProperty, XUKAble, XMLPropertyValidator {
    /**
     * @hidden
     */
    public XMLType getXMLType() {
        return null;
    }

    /**
     * @hidden
     */
    public void setXMLType(XMLType newType) {
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
    public List getListOfAttributes() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean setAttribute(XMLAttribute attr) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public XMLAttribute getAttribute(String namespace, String name) throws MethodParameterIsNullException {
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
    public XMLPropertyImpl copy() {
        return null;
    }

    /**
     * @hidden
     */
    public boolean XUKIn(URI source) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean XUKOut(URI destination) {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canSetAttribute(XMLAttribute attr) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canSetNamespace(String newNS) throws MethodParameterIsNullException {
        return false;
    }

    /**
     * @hidden
     */
    public boolean canSetName(String newName) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException {
        return false;
    }
}