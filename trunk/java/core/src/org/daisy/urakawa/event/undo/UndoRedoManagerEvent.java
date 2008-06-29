package org.daisy.urakawa.event.undo;

import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.undo.IUndoRedoManager;

/**
 *
 *
 */
public class UndoRedoManagerEvent extends DataModelChangedEvent {
	/**
	 * @param source
	 */
	public UndoRedoManagerEvent(IUndoRedoManager source) {
		super(source);
		mSourceUndoRedoManager = source;
	}

	private IUndoRedoManager mSourceUndoRedoManager;

	/**
	 * @return data
	 */
	public IUndoRedoManager getSourceUndoRedoManager() {
		return mSourceUndoRedoManager;
	}
}