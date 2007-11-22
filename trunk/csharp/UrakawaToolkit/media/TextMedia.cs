using System;
using System.Xml;

namespace urakawa.media
{
	/// <summary>
	/// TextMedia represents a text string
	/// </summary>
	public class TextMedia : AbstractMedia, ITextMedia
	{
		
		#region Event related members

		/// <summary>
		/// Event fired after the text of the <see cref="TextMedia"/> has changed
		/// </summary>
		public event EventHandler<urakawa.events.TextChangedEventArgs> textChanged;
		/// <summary>
		/// Fires the <see cref="textChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="TextMedia"/> whoose text was changed</param>
		/// <param name="newText">The new text value</param>
		/// <param name="prevText">Thye text value prior to the change</param>
		protected void notifyTextChanged(ExternalTextMedia source, string newText, string prevText)
		{
			EventHandler<urakawa.events.TextChangedEventArgs> d = textChanged;
			if (d != null) d(this, new urakawa.events.TextChangedEventArgs(source, newText, prevText));
		}

		void this_textChanged(object sender, urakawa.events.TextChangedEventArgs e)
		{
			notifyChanged(e);
		}
		#endregion

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref localName="fact"/> is <c>null</c>
		/// </exception>
		protected internal TextMedia()
		{
			mText = "";
			this.textChanged += new EventHandler<urakawa.events.TextChangedEventArgs>(this_textChanged);
		}


		private string mText;


		/// <summary>
		/// This override is useful while debugging
		/// </summary>
		/// <returns>The textual content of the <see cref="ITextMedia"/></returns>
		public override string ToString()
		{
			return mText;
		}

		#region ITextMedia Members

		/// <summary>
		/// Return the text string
		/// </summary>
		/// <returns></returns>
		public string getText()
		{
			return mText;
		}

		/// <summary>
		/// Set the text string
		/// </summary>
		/// <param name="text"></param>
		public void setText(string text)
		{
			if (text == null)
			{
				throw new exception.MethodParameterIsNullException("The text of a TextMedia cannot be null");
			}
			mText = text;
		}

		#endregion

		#region IMedia Members

		/// <summary>
		/// This always returns false, because
		/// text media is never considered continuous
		/// </summary>
		/// <returns></returns>
		public override bool isContinuous()
		{
			return false;
		}

		/// <summary>
		/// This always returns true, because
		/// text media is always considered discrete
		/// </summary>
		/// <returns></returns>
		public override bool isDiscrete()
		{
			return true;
		}


		/// <summary>
		/// This always returns false, because
		/// a single media object is never considered to be a sequence
		/// </summary>
		/// <returns></returns>
		public override bool isSequence()
		{
			return false;
		}

		/// <summary>
		/// Make a copy of this text object
		/// </summary>
		/// <returns>The copy</returns>
		public new TextMedia copy()
		{
			return copyProtected() as TextMedia;
		}

		/// <summary>
		/// Make a copy of this text object
		/// </summary>
		/// <returns>The copy</returns>
		protected override IMedia copyProtected()
		{
			return export(getMediaFactory().getPresentation());
		}

		/// <summary>
		/// Exports the text media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external text media</returns>
		public new TextMedia export(Presentation destPres)
		{
			return exportProtected(destPres) as TextMedia;
		}

		protected override IMedia exportProtected(Presentation destPres)
		{
			TextMedia exported = destPres.getMediaFactory().createMedia(
				getXukLocalName(), getXukNamespaceUri()) as TextMedia;
			if (exported == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a TextMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.setText(this.getText());
			return exported;
		}


		#endregion

		#region IXukAble members

		protected override void clear()
		{
			mText = "";
			base.clear();
		}

		protected override void xukInChild(XmlReader source)
		{
			if (source.LocalName == "mText" && source.NamespaceURI == ToolkitSettings.XUK_NS)
			{
				if (!source.IsEmptyElement)
				{
					XmlReader subtreeReader = source.ReadSubtree();
					subtreeReader.Read();
					try
					{
						setText(subtreeReader.ReadElementContentAsString());
					}
					finally
					{
						subtreeReader.Close();
					}
				}
				return;
			}
			base.xukInChild(source);
		}

		protected override void xukOutChildren(XmlWriter destination, Uri baseUri)
		{
			destination.WriteStartElement("mText", ToolkitSettings.XUK_NS);
			destination.WriteString(getText());
			destination.WriteEndElement();
			base.xukOutChildren(destination, baseUri);
		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Compares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool valueEquals(IMedia other)
		{
			if (!base.valueEquals(other)) return false;
			if (getText() != ((TextMedia)other).getText()) return false;
			return true;
		}

		#endregion
	}
}