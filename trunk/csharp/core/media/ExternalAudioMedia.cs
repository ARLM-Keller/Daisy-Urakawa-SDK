using System;
using System.Xml;
using urakawa.media.timing;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class ExternalAudioMedia : ExternalMedia, IAudioMedia, IClipped
	{
		#region Event related members
		/// <summary>
		/// Event fired after the clip (clip begin or clip end) of the <see cref="ExternalAudioMedia"/> has changed
		/// </summary>
		public event EventHandler<events.media.ClipChangedEventArgs> ClipChanged;
		/// <summary>
		/// Fires the <see cref="ClipChanged"/> event
		/// </summary>
		/// <param name="source">The source, that is the <see cref="ExternalAudioMedia"/> whoose clip has changed</param>
		/// <param name="newCB">The new clip begin value</param>
		/// <param name="newCE">The new clip begin value</param>
		/// <param name="prevCB">The clip begin value prior to the change</param>
		/// <param name="prevCE">The clip end value prior to the change</param>
		protected void notifyClipChanged(ExternalAudioMedia source, Time newCB, Time newCE, Time prevCB, Time prevCE)
		{
			EventHandler<events.media.ClipChangedEventArgs> d = ClipChanged;
			if (d != null) d(this, new urakawa.events.media.ClipChangedEventArgs(source, newCB, newCE, prevCB, prevCE));
		}

		void this_clipChanged(object sender, urakawa.events.media.ClipChangedEventArgs e)
		{
			notifyChanged(e);
		}

		#endregion

		private Time mClipBegin;
		private Time mClipEnd;

		private void resetClipTimes()
		{
			mClipBegin = Time.Zero;
			mClipEnd = Time.MaxValue;
			this.ClipChanged += new EventHandler<urakawa.events.media.ClipChangedEventArgs>(this_clipChanged);
		}

		/// <summary>
		/// Constructor setting the associated <see cref="IMediaFactory"/>
		/// </summary>
		protected internal ExternalAudioMedia() : base()
		{
			resetClipTimes();
		}
		
		#region IMedia members

	    /// <summary>
	    /// This always returns true, because
	    /// audio media is always considered continuous
	    /// </summary>
	    /// <returns></returns>
	    public override bool IsContinuous
	    {
	        get { return true; }
	    }

	    /// <summary>
	    /// This always returns false, because
	    /// audio media is never considered discrete
	    /// </summary>
	    /// <returns></returns>
	    public override bool IsDiscrete
	    {
	        get { return false; }
	    }

	    /// <summary>
	    /// This always returns false, because
	    /// a single media object is never considered to be a sequence
	    /// </summary>
	    /// <returns></returns>
	    public override bool IsSequence
	    {
	        get { return false; }
	    }

	    /// <summary>
		/// Copy function which returns an <see cref="IAudioMedia"/> object
		/// </summary>
		/// <returns>A copy of this</returns>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown when the <see cref="IMediaFactory"/> associated with this 
		/// can not create an <see cref="ExternalAudioMedia"/> matching the QName of <see cref="ExternalAudioMedia"/>
		/// </exception>
		public new ExternalAudioMedia Copy()
		{
			return copyProtected() as ExternalAudioMedia;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		public new ExternalAudioMedia Export(Presentation destPres)
		{
			return exportProtected(destPres) as ExternalAudioMedia;
		}

		/// <summary>
		/// Exports the external audio media to a destination <see cref="Presentation"/>
		/// - part of technical construct to have <see cref="Export"/> return <see cref="ExternalAudioMedia"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported external audio media</returns>
		protected override IMedia exportProtected(Presentation destPres)
		{
			ExternalAudioMedia exported = base.exportProtected(destPres) as ExternalAudioMedia;
			if (exported==null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The MediaFactory cannot create a ExternalAudioMedia matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			exported.ClipBegin = ClipBegin.copy();
			exported.ClipEnd = ClipEnd.copy();
			return exported;
		}

		#endregion



		#region IXUKAble members

		/// <summary>
		/// Reads the attributes of a ExternalAudioMedia xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected override void xukInAttributes(XmlReader source)
		{
			base.xukInAttributes(source);
			resetClipTimes();
			Time cbTime, ceTime;
			try
			{
				cbTime = new Time(source.GetAttribute("clipBegin"));
			}
			catch (exception.CheckedException e)
			{
				throw new exception.XukException(
					String.Format("clipBegin attribute is missing or has invalid value: {0}", e.Message),
					e);
			}
			try
			{
				ceTime = new Time(source.GetAttribute("clipEnd"));
			}
			catch (exception.CheckedException e)
			{
				throw new exception.XukException(
					String.Format("clipEnd attribute is missing or has invalid value: {0}", e.Message),
					e);
			}
			if (cbTime.isNegativeTimeOffset())
			{
				ClipBegin = cbTime;
				ClipEnd = ceTime;
			}
			else
			{
				ClipEnd = ceTime;
				ClipBegin = cbTime;
			}
		}

		/// <summary>
		/// Writes the attributes of a ExternalAudioMedia element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{
			destination.WriteAttributeString("clipBegin", this.ClipBegin.ToString());
			destination.WriteAttributeString("clipEnd", this.ClipEnd.ToString());
			base.xukOutAttributes(destination, baseUri);
		}

		/// <summary>
		/// Write the child elements of a ExternalAudioMedia element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		protected virtual void xukOutChildren(XmlWriter destination)
		{

		}

		#endregion

		#region IContinuous Members

	    /// <summary>
	    /// Gets the duration of <c>this</c>
	    /// </summary>
	    /// <returns>A <see cref="TimeDelta"/> representing the duration</returns>
	    public TimeDelta Duration
	    {
	        get { return ClipEnd.getTimeDelta(ClipBegin); }
	    }

	    #endregion

		#region IClipped Members

	    /// <summary>
	    /// Gets the clip begin <see cref="Time"/> of <c>this</c>
	    /// </summary>
	    /// <returns>Clip begin</returns>
	    public Time ClipBegin
	    {
	        get { return mClipBegin; }
	        set
	        {
	            if (value == null)
	            {
	                throw new exception.MethodParameterIsNullException("ClipBegin can not be null");
	            }
	            if (value.isLessThan(Time.Zero))
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "ClipBegin is a negative time offset");
	            }
	            if (value.isGreaterThan(ClipEnd))
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "ClipBegin can not be after ClipEnd");
	            }
	            if (!mClipBegin.isEqualTo(value))
	            {
	                Time prevCB = ClipBegin;
	                mClipBegin = value.copy();
	                notifyClipChanged(this, ClipBegin, ClipEnd, prevCB, ClipEnd);
	            }
	        }
	    }

	    /// <summary>
	    /// Gets the clip end <see cref="Time"/> of <c>this</c>
	    /// </summary>
	    /// <returns>Clip end</returns>
	    public Time ClipEnd
	    {
	        get { return mClipEnd; }
	        set
	        {
	            if (value == null)
	            {
	                throw new exception.MethodParameterIsNullException("ClipEnd can not be null");
	            }
	            if (value.isLessThan(ClipBegin))
	            {
	                throw new exception.MethodParameterIsOutOfBoundsException(
	                    "ClipEnd can not be before ClipBegin");
	            }
	            if (!mClipEnd.isEqualTo(value))
	            {
	                Time prevCE = ClipEnd;
	                mClipEnd = value.copy();
	                notifyClipChanged(this, ClipBegin, ClipEnd, ClipBegin, prevCE);
	            }
	        }
	    }

	    IContinuous IContinuous.Split(Time splitPoint)
		{
			return Split(splitPoint);
		}

		/// <summary>
		/// Splits <c>this</c> at a given <see cref="Time"/>
		/// </summary>
		/// <param name="splitPoint">The <see cref="Time"/> at which to split - 
		/// must be between clip begin and clip end <see cref="Time"/>s</param>
		/// <returns>
		/// A newly created <see cref="IAudioMedia"/> containing the audio after,
		/// <c>this</c> retains the audio before <paramref localName="splitPoint"/>.
		/// </returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="splitPoint"/> is <c>null</c>
		/// </exception>
		/// <exception cref="exception.MethodParameterIsOutOfBoundsException">
		/// Thrown when <paramref name="splitPoint"/> is not between clip begin and clip end
		/// </exception>
		public ExternalAudioMedia Split(Time splitPoint)
		{
			if (splitPoint==null)
			{
				throw new exception.MethodParameterIsNullException(
					"The time at which to split can not be null");
			}
			if (splitPoint.isLessThan(ClipBegin))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be before ClipBegin");
			}
			if (splitPoint.isGreaterThan(ClipEnd))
			{
				throw new exception.MethodParameterIsOutOfBoundsException(
					"The split time can not be after ClipEnd");
			}
			ExternalAudioMedia splitAM = (ExternalAudioMedia)Copy();
			ClipEnd = splitPoint;
			splitAM.ClipBegin = splitPoint;
			return splitAM;

		}

		#endregion

		#region IValueEquatable<IMedia> Members

		/// <summary>
		/// Conpares <c>this</c> with a given other <see cref="IMedia"/> for equality
		/// </summary>
		/// <param name="other">The other <see cref="IMedia"/></param>
		/// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
		public override bool ValueEquals(IMedia other)
		{
			if (!base.ValueEquals(other)) return false;
			ExternalAudioMedia otherAudio = (ExternalAudioMedia)other;
			if (!ClipBegin.isEqualTo(otherAudio.ClipBegin)) return false;
			if (!ClipEnd.isEqualTo(otherAudio.ClipEnd)) return false;
			return true;
		}

		#endregion

	}
}