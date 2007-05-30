package org.daisy.urakawa.media;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @checked against C# implementation [29 May 2007]
 * @todo verify / add comments and exceptions
 */
public class MediaFactoryImpl implements MediaFactory {
	public Media createMedia(MediaType type) {
		
		return null;
	}

	public Media createMedia(String xukLocalName, String xukNamespaceUri) {
		
		return null;
	}

	public MediaPresentation getPresentation() {
		
		return null;
	}

	public void setPresentation(MediaPresentation presentation)
			throws MethodParameterIsNullException {
		
	}
}
