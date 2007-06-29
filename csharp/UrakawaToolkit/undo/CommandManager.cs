using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.undo
{
	/// <summary>
	/// The command manager.
	/// </summary>
	public class CommandManager
	{
		private Stack<ICommand> mUndoStack;  // stack of commands to exception
		private Stack<ICommand> mRedoStack;  // stack of commands to redo

		/// <summary>
		/// Create an empty command manager.
		/// </summary>
		public CommandManager()
		{
			mUndoStack = new Stack<ICommand>();
			mRedoStack = new Stack<ICommand>();
		}

		/// <summary>
		/// Get the name of the next exception command.
		/// </summary>
		/// <exception cref="CannotUndoException">Thrown when there is no command to exception.</exception>
		public string getUndoShortDescription()
		{
			if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to exception.");
			return mUndoStack.Peek().getUnExecuteShortDescription();
		}

		/// <summary>
		/// Undo the last executed command.
		/// </summary>
		/// <exception cref="CannotUndoException">Thrown when there is no command to exception.</exception>
		public void undo()
		{
			if (mUndoStack.Count == 0) throw new exception.CannotUndoException("There is no command to exception.");
			mUndoStack.Peek().unExecute();
			mRedoStack.Push(mUndoStack.Pop());
		}

		/// <summary>
		/// Get the name of the next redo command.
		/// </summary>
		/// <exception cref="CannotRedoException">Thrown when there is no command to redo.</exception>
		public string getRedoShortDescription()
		{
			if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
			return mRedoStack.Peek().getExecuteShortDescription();
		}

		/// <summary>
		/// Redo the last unexecuted command.
		/// </summary>
		/// <exception cref="CannotRedoException">Thrown when there is no command to redo.</exception>
		public void redo()
		{
			if (mRedoStack.Count == 0) throw new exception.CannotRedoException("There is no command to redo.");
			mRedoStack.Peek().execute();
			mUndoStack.Push(mRedoStack.Pop());
		}

		/// <summary>
		/// Execute and register the given command in the exception history and clear the redo history.
		/// </summary>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
		public void execute(ICommand command)
		{
			if (command == null) throw new exception.MethodParameterIsNullException("Command cannot be null.");
			command.execute();
			mUndoStack.Push(command);
			mRedoStack.Clear();
		}

		/// <summary>
		/// Return true if the exception is history is non-empty.
		/// </summary>
		public bool canUndo()
		{
			return mUndoStack.Count > 0;
		}

		/// <summary>
		/// Return true if the redo is history is non-empty.
		/// </summary>
		public bool canRedo()
		{
			return mRedoStack.Count > 0;
		}
	}
}
