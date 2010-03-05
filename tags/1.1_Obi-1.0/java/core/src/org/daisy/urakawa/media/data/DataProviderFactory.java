package org.daisy.urakawa.media.data;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the factory that creates
 * {@link org.daisy.urakawa.media.data.DataProvider} instances.
 * </p>
 * <p>
 * The returned object is managed by its associated manager.
 * </p>
 * 
 * @depend - Create - org.daisy.urakawa.media.data.DataProvider
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 */
public interface DataProviderFactory extends XukAble, WithPresentation {
	/**
	 * @return Gets the DataProviderManager associated with the
	 *         DataProviderFactory
	 * @throws IsNotInitializedException 
	 */
	DataProviderManager getDataProviderManager() throws IsNotInitializedException;

	/**
	 * <p>
	 * Creates a DataProvider instance of default type for a given MIME type.
	 * </p>
	 * <p>
	 * This factory method takes a single argument to specify the exact type of
	 * object to create.
	 * </p>
	 * 
	 * @param mimeType
	 * @return can return null (in case the given argument does not match any
	 *         supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	DataProvider createDataProvider(String mimeType)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;

	/**
	 * <p>
	 * Creates a DataProvider instance of type matching a given XUK QName for a
	 * given MIME type.
	 * </p>
	 * 
	 * @param mimeType
	 * @param xukLocalName
	 *            cannot be null, cannot be empty string.
	 * @param xukNamespaceURI
	 *            cannot be null, but can be empty string.
	 * @return can return null (in case the given argument and QName
	 *         specification does not match any supported type).
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden:
	 *             <b>xukLocalName, mimeType</b>
	 */
	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException;
}