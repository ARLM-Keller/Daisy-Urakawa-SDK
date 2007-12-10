package org.daisy.urakawa.property.xml;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is an XML attribute owned by an XmlProperty.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Clone - org.daisy.urakawa.property.xml.XmlAttribute
 * @depend - Aggregation 1 org.daisy.urakawa.property.xml.XmlProperty
 * @stereotype XukAble
 */
public interface XmlAttribute extends WithXmlProperty, WithQualifiedName,
		WithValue, WithPresentation, XukAble {
	/**
	 * <p>
	 * Cloning method
	 * </p>
	 * 
	 * @return a copy.
	 */
	XmlAttribute copy();
}