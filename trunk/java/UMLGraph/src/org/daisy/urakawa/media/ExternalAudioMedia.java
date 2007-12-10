package org.daisy.urakawa.media;

/**
 * A concrete audio media type, with an external "Located" resource which is
 * un-managed.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface ExternalAudioMedia extends AudioMedia, Clipped, Located {
}