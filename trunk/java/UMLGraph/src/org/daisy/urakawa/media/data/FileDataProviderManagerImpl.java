package org.daisy.urakawa.media.data;

import java.net.URI;
import java.util.List;

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
public class FileDataProviderManagerImpl implements FileDataProviderManager {
	public String getDataFileDirectory() {
		return null;
	}

	public String getDataFileDirectoryFullPath() {
		return null;
	}

	public List<FileDataProvider> getListOfFileDataProviders() {
		return null;
	}

	public String getNewDataFileRelPath(String extension)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public void moveDataFiles(String newDataFileDir, boolean deleteSource,
			boolean overwriteDestDir) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void setDataFileDirectoryPath(String newPath)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void addDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public void deleteUnusedDataProviders() {
	}

	public void detachDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public void detachDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public DataProvider getDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		return null;
	}

	public List<DataProvider> getListOfDataProviders() {
		return null;
	}

	public String getUidOfDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
		return null;
	}

	public Presentation getPresentation() {
		return null;
	}

	public void setPresentation(Presentation presentation)
			throws MethodParameterIsNullException {
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

	public boolean ValueEquals(DataProviderManager other)
			throws MethodParameterIsNullException {
		return false;
	}

	public void removeDataProvider(String uid)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void removeDataProvider(DataProvider provider)
			throws MethodParameterIsNullException {
	}

	public void removeUnusedDataProviders() {
	}

	public boolean isManagerOf(String uid) {
		return false;
	}

	public void xukIn(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
	}

	public void xukOut(XmlDataWriter destination, URI baseURI)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
	}
}