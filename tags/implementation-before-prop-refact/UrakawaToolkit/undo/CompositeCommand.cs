using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace urakawa.undo
{
	/// <summary>
	/// A "mega-command" made of a series of "smaller" commands. Useful for merging small commands into one such as:
	/// user typing text letter by letter (the exception/redo would work on full word or sentence, not for each character.)
	/// </summary>
	public class CompositeCommand : WithPresentation, ICommand
	{
		private List<ICommand> mCommands;
		private string mLongDescription = null;
		private string mShortDescription = null;

		public static string ShortDescriptionFormatString = "{0:0} commands: {1}";
		public static string LongDescriptionFormatString = "{0:0} commands:\n{1}";

		/// <summary>
		/// Create an empty composite command.
		/// </summary>
		public CompositeCommand()
		{
			mCommands = new List<ICommand>();
		}

		/// <summary>
		/// Sets the long humanly-readable description of the composite command
		/// </summary>
		/// <param name="desc">The long description</param>
		public void setLongDescription(string desc)
		{
			mLongDescription = desc;
		}

		/// <summary>
		/// Sets the long humanly-readable description of the composite command
		/// </summary>
		/// <param name="desc">The long description</param>
		public void setShortDescription(string desc)
		{
			mLongDescription = desc;
		}

		/// <summary>
		/// Insert the given command as a child of this node, at the given index. Does NOT replace the existing child,
		/// but increments (+1) the indices of the all children which index >= insertIndex. If insertIndex == children.size
		/// (no following children), then the given node is appended at the end of the existing children list.
		/// </summary>
		/// <param name="command">Cannot be null.</param>
		/// <param name="index">Must be within bounds [0 .. children.size]</param>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">Thrown when the index is out of bounds.</exception>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when a null command is given.</exception>
		public void insert(ICommand command, int index)
		{
			if (command == null) throw new exception.MethodParameterIsNullException("Cannot insert a null command.");
			if (index < 0 || index > mCommands.Count)
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
						String.Format("Cannot insert at index {0}; expected index in range [0 .. {1}]", index, mCommands.Count));
			}
			mCommands.Insert(index, command);
		}

		/// <summary>
		/// Appends the given command as a child of this node, at the given index. Does NOT replace the existing child,
		/// but increments (+1) the indices of the all children which index >= insertIndex. If insertIndex == children.size
		/// (no following children), then the given node is appended at the end of the existing children list.
		/// </summary>
		/// <param name="command">Cannot be null.</param>
		public void append(ICommand command)
		{
			insert(command, getCount());
		}

		/// <summary>
		/// Gets the number of <see cref="ICommand"/>s in <c>this</c>
		/// </summary>
		/// <returns></returns>
		public int getCount()
		{
			return mCommands.Count;
		}


		#region ICommand Members

		/// <summary>
		/// Execute the reverse command by executing the reverse commands for all the contained commands.
		/// The commands are undone in reverse order.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotUndoException">Thrown when the command cannot be reversed; either because
		/// the composite command is empty or one of its contained command cannot be undone. In the latter case, the original
		/// exception is passed as the inner exception of the thrown exception.</exception>
		public void unExecute()
		{
			if (mCommands.Count == 0) throw new exception.CannotUndoException("Composite command is empty.");
			try
			{
				for (int i = mCommands.Count - 1; i >= 0; --i) mCommands[i].unExecute();
			}
			catch (exception.CannotUndoException e)
			{
				throw new exception.CannotUndoException("Contained command could not be undone", e);
			}
		}

		/// <summary>
		/// Return the provided exception string.
		/// </summary>
		public string getLongDescription()
		{
			if (mLongDescription != null) return mLongDescription;
			string cmds = "-";
			if (mCommands.Count > 0)
			{
				cmds = mCommands[0].getLongDescription();
				for (int i = 1; i < mCommands.Count; i++)
				{
					cmds += "\n" + getLongDescription();
				}
			}
			return String.Format(LongDescriptionFormatString, mCommands.Count, cmds);
		}

		/// <summary>
		/// Execute all contained commands in order.
		/// </summary>
		/// <exception cref="urakawa.exception.CannotRedoException">Thrown when the command cannot be executed; either because
		/// the composite command is empty or one of its contained command cannot be executed. In the latter case, the original
		/// exception is passed as the inner exception of the thrown exception.</exception>
		public void execute()
		{
			if (mCommands.Count == 0) throw new exception.CannotRedoException("Composite command is empty.");
			try
			{
				foreach (ICommand command in mCommands) command.execute();
			}
			catch (exception.CannotRedoException e)
			{
				throw new exception.CannotRedoException(String.Format("Contained command could not be executed: {0}", e.Message),e);
			}
		}

		/// <summary>
		/// Return the provided redo string.
		/// </summary>
		public string getShortDescription()
		{
			if (mShortDescription != null) return mShortDescription;
			string cmds = "-";
			if (mCommands.Count > 0)
			{
				cmds = mCommands[0].getShortDescription();
				if (mCommands.Count > 1)
				{
					cmds += "..." + mCommands[mCommands.Count - 1];
				}
			}
			return String.Format(ShortDescriptionFormatString, mCommands.Count, cmds);
		}

		/// <summary>
		/// The composite command is reversible if it contains commmands, and all of its contained command are.
		/// </summary>
		public bool canUnExecute()
		{
			return mCommands.Count > 0 && mCommands.TrueForAll(delegate(ICommand c) { return c.canUnExecute(); });
		}


		public List<urakawa.media.data.MediaData> getListOfUsedMediaData()
		{
			List<media.data.MediaData> res = new List<urakawa.media.data.MediaData>();
			foreach (ICommand cmd in mCommands)
			{
				res.AddRange(cmd.getListOfUsedMediaData());
			}
			return res;
		}

		#endregion

		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="CompositeCommand"/> from a CompositeCommand xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void XukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not XukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read CompositeCommand from a non-element node");
			}
			try
			{
				XukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							XukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukIn of CompositeCommand: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a CompositeCommand xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a CompositeCommand xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void XukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a CompositeCommand element to a XUK file representing the <see cref="CompositeCommand"/> instance
		/// </summary>
		/// <param localName="destination">The destination <see cref="XmlWriter"/></param>
		public void XukOut(XmlWriter destination)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not XukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				XukOutAttributes(destination);
				XukOutChildren(destination);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during XukOut of CompositeCommand: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a CompositeCommand element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutAttributes(XmlWriter destination)
		{

		}

		/// <summary>
		/// Write the child elements of a CompositeCommand element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void XukOutChildren(XmlWriter destination)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="CompositeCommand"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="CompositeCommand"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

	}
}