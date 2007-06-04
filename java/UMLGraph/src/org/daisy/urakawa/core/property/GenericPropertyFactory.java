package org.daisy.urakawa.core.property;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * A convenience interface to isolate the factory methods for generic
 * properties.
 * 
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 * @depend - Create 1 Property
 */
public interface GenericPropertyFactory {
	/**
	 * @param xukLocalName
	 * @param xukNamespaceUri
	 * @return a new Property object corresponding to the given type.
	 */
	public Property createProperty(String xukLocalName, String xukNamespaceUri) throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
