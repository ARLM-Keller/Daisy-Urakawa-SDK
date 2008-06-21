package org.daisy.urakawa.properties;

import org.daisy.urakawa.properties.core.XMLProperty;

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
 * @depend - Create 1 XMLProperty
 * @see org.daisy.urakawa.properties.PropertyFactory
 */
public class PropertyFactoryImpl implements PropertyFactory {
    /**
     * @hidden
     */
    public Property createProperty(String type) {
        return null;
    }

    public XMLProperty createXMLProperty() {
        return null;
    }
}