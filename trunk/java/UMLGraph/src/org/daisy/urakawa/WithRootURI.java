package org.daisy.urakawa;

import java.net.URISyntaxException;
import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * The getter and setter for the root URI of the Presentation.
 */
public interface WithRootURI {
	/**
	 * Returns the default root URI for the Presentation or the one specified by
	 * setRootURI().
	 * 
	 * @return The root URI of the Presentation.
	 */
	public URI getRootURI();

	/**
	 * Sets the root URI of the Presentation
	 * 
	 * @param newRootUri
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws URISyntaxException
	 *             when the given URI is not absolute
	 * @tagvalue Exceptions "MethodParameterIsNull-URISyntax"
	 */
	public void setRootURI(URI newRootUri)
			throws MethodParameterIsNullException, URISyntaxException;
}