package org.daisy.urakawa.undo;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaData;
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
public class UndoRedoManagerImpl implements UndoRedoManager {
	public boolean canRedo() {
		return false;
	}

	public boolean canUndo() {
		return false;
	}

	public void execute(Command command) throws MethodParameterIsNullException {
	}

	public String getRedoShortDescription() throws CannotRedoException {
		return null;
	}

	public String getUndoShortDescription() throws CannotUndoException {
		return null;
	}

	public void redo() throws CannotRedoException {
	}

	public void undo() throws CannotUndoException {
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

	public void flushCommands() {
	}

	public void cancelTransaction()
			throws UndoRedoTransactionIsNotStartedException {
	}

	public void endTransaction()
			throws UndoRedoTransactionIsNotStartedException {
	}

	public boolean isTransactionActive() {
		return false;
	}

	public void startTransaction(String shortDescription, String longDescription) {
	}

	public List<Command> getListOfCommandsInCurrentTransactions() {
		return null;
	}

	public List<Command> getListOfRedoStackCommands() {
		return null;
	}

	public List<Command> getListOfUndoStackCommands() {
		return null;
	}

	public List<MediaData> getListOfUsedMediaData() {
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