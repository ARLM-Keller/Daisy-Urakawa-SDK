package org.daisy.urakawa.undo;

import java.util.List;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.media.data.MediaData;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * Classes realizing this interface must store the state of the object(s)
 * affected by the command execution.
 * </p>
 * @stereotype XukAble
 */
public interface Command extends XukAble, WithPresentation, WithShortLongDescription {
	/**
	 * <p>
	 * Returns a list of MediaData objects that are in use by this command.
	 * </p>
	 * 
	 * @return a non-null, possibly empty, list of Media objects
	 */
	public List<MediaData> getListOfUsedMediaData();

	/**
	 * <p>
	 * executes the Command
	 * </p>
	 */
	public void execute() throws CommandCannotExecuteException;

	/**
	 * <p>
	 * executes the reverse Command
	 * </p>
	 */
	public void unExecute() throws CommandCannotUnExecuteException;

	/**
	 * <p>
	 * Tests whether this command can be un-executed.
	 * </p>
	 * 
	 * @return true if this command can be un-executed.
	 */
	public boolean canUnExecute();

	/**
	 * <p>
	 * Tests whether this command can be executed.
	 * </p>
	 * 
	 * @return true if this command can be executed.
	 */
	public boolean canExecute();
}