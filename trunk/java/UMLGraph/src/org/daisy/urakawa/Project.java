package org.daisy.urakawa;

import java.net.URI;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.WithMetadataFactory;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Composition 1 MetadataFactory
 * @depend - Composition 1 Presentation
 */
public interface Project extends WithPresentation, WithMetadataFactory,
		XukAble, ValueEquatable<Project> {
	/**
	 * @param uri
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param reader
	 *            cannot be null.
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean openXUK(XmlDataReader reader)
			throws MethodParameterIsNullException;

	/**
	 * @param uri
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(URI uri) throws MethodParameterIsNullException;

	/**
	 * @param writer
	 *            cannot be null
	 * @return true if successful.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public boolean saveXUK(XmlDataWriter writer)
			throws MethodParameterIsNullException;
}
