package org.daisy.urakawa.property.xml;

import java.net.URI;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XmlDataReader;
import org.daisy.urakawa.xuk.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class XmlAttributeImpl implements XmlAttribute {
	public XmlAttribute copy() {
		return null;
	}

	public XmlProperty getXmlProperty() {
		return null;
	}

	public void setXmlProperty(XmlProperty prop)
			throws MethodParameterIsNullException {
	}

	public String getLocalName() {
		return null;
	}

	public String getNamespace() {
		return null;
	}

	public void setLocalName(String newName)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void setNamespace(String newNS)
			throws MethodParameterIsNullException {
	}

	public String getValue() {
		return null;
	}

	public void setValue(String newValue)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void XukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void XukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public String getXukLocalName() {
		return null;
	}

	public String getXukNamespaceURI() {
		return null;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
	}
}
