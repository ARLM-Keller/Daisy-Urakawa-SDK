package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.CheckedException;

/**
 * <p>
 * This exception is raised when a {@link org.daisy.urakawa.core.TreeNode} is
 * required to be in the same {@link org.daisy.urakawa.Presentation} as another
 * node, but isn't.
 * </p>
 */
public class TreeNodeIsInDifferentPresentationException extends
		CheckedException {
	/**
	 * 
	 */
	private static final long serialVersionUID = 7503001642396792101L;
}