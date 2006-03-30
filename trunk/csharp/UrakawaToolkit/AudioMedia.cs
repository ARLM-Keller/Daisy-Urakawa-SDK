using System;

namespace urakawa.media
{
	/// <summary>
	/// AudioMedia is the audio object.
	/// It is time-based and comes from an external source.
	/// </summary>
	public class AudioMedia : IAudioMedia
	{
		private Time mClipBegin = new Time();
		private Time mClipEnd = new Time();
		private MediaLocation mMediaLocation = new MediaLocation();
		
		//internal constructor encourages use of MediaFactory to create AudioMedia objects
		internal AudioMedia()
		{
		}

		/// <summary>
		/// this override is useful while debugging
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "AudioMedia";
		}
	
		#region IClippedMedia Members

		public ITimeDelta getDuration()
		{
			return mClipBegin.getDelta(mClipEnd);
		}

		public ITime getClipBegin()
		{
			return mClipBegin;
		}

		public ITime getClipEnd()
		{
			return mClipEnd;
		}

		public void setClipBegin(ITime beginPoint)
		{
			if (beginPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipBegin (null) caused MethodParameterIsNullException");
				return;
			}

			if (beginPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipBegin (" + 
					beginPoint.ToString() + ") caused TimeOffsetIsNegativeException");

				//TODO
				//should the function return null upon receiving this exception?
				//or is there a situation where a negative clipBegin would be meaningful?
			}

			mClipBegin = (Time)beginPoint;
		}

		public void setClipEnd(ITime endPoint)
		{
			if (endPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setClipEnd (null) caused MethodParameterIsNullException");
				return;
			}

			if (endPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.setClipEnd (" + 
					endPoint.ToString() + ") caused TimeOffsetIsNegativeException");

				//TODO
				//should the function return null upon receiving this exception?
				//or is there a situation where a negative clipEnd would be meaningful?
			}
			mClipEnd = (Time)endPoint;
		}

		public IClippedMedia split(ITime splitPoint)
		{
			if (splitPoint == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.split (null) caused MethodParameterIsNullException");
				return null;
			}

			if (splitPoint.isNegativeTimeOffset() == true)
			{
				throw new exception.TimeOffsetIsNegativeException("AudioMedia.split (" + 
					splitPoint.ToString() + ") caused TimeOffsetIsNegativeException");
				
				
				//TODO
				//should the function return null upon receiving this exception?
				//or is there a situation where a negative splitPoint would be meaningful?
			}


			AudioMedia splitMedia = new AudioMedia();
			splitMedia.setClipBegin(splitPoint);
			splitMedia.setClipEnd(mClipEnd);

			this.setClipEnd(splitPoint);

			return splitMedia;
		}

		#endregion

		#region IExternalMedia Members

		public IMediaLocation getLocation()
		{
			return mMediaLocation;
		}

		public void setLocation(IMediaLocation location)
		{
			if (location == null)
			{
				throw new exception.MethodParameterIsNullException("AudioMedia.setLocation(null) caused MethodParameterIsNullException");
				return;
			}

			mMediaLocation = (MediaLocation)location;
		}

		#endregion
		
		#region IMedia Members

		public bool isContinuous()
		{
			return true;
		}

		public bool isDiscrete()
		{
			return false;
		}

		public urakawa.media.MediaType getType()
		{
			return MediaType.AUDIO;
		}

		#endregion

	}
}
