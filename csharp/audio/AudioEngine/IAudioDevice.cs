using System;
using System.Collections.Generic;
using System.Text;

namespace AudioEngine
{
	/// <summary>
	/// Enumeration of the possible states of a <see cref="IAudioDevice"/>
	/// </summary>
	public enum AudioDeviceState
	{
		/// <summary>
		/// The <see cref="IAudioDevice"/> is stopped
		/// </summary>
		Stopped,
		/// <summary>
		/// The <see cref="IAudioDevice"/> is playing
		/// </summary>
		Playing,
		/// <summary>
		/// The <see cref="IAudioDevice"/> is paused during playback
		/// </summary>
		PausedPlay,
		/// <summary>
		/// The <see cref="IAudioDevice"/> is recording, that is capturing audio
		/// </summary>
		Recording,
		/// <summary>
		/// The <see cref="IAudioDevice"/> is paused during recording
		/// </summary>
		PausedRecord

	}

	/// <summary>
	/// Interface for a generic audio device
	/// </summary>
	public interface IAudioDevice
	{
		/// <summary>
		/// Fired at regular intervals during playback and recording 
		/// to indicate the progress in time of the playback/recording
		/// </summary>
		event EventHandler<TimeEventArgs> Time;

		/// <summary>
		/// Fired when the <see cref="AudioDeviceState"/> changes
		/// </summary>
		event EventHandler<StateChangedEventArgs> StateChanged;

		/// <summary>
		/// Fired when an overload has occured
		/// </summary>
		event EventHandler<OverloadEventArgs> OverloadOccured;

		/// <summary>
		/// Gets the name of the <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The name</returns>
		string getName();

		/// <summary>
		/// Gets the number of channels setting for the <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The number of channels</returns>
		ushort getNumberOfChannels();

		/// <summary>
		/// Sets the number of channels setting for the <see cref="IAudioDevice"/>
		/// </summary>
		/// <param name="newNOC">The new number of channels</param>
		void setNumberOfChannels(ushort newNOC);

		/// <summary>
		/// Gets the sample rate setting for the <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The sample rate in Hz</returns>
		uint getSampleRate();

		/// <summary>
		/// Sets the sample rate setting for the <see cref="IAudioDevice"/>
		/// </summary>
		/// <param name="newSR">The new sample rate in Hz</param>
		void setSampleRate(uint newSR);

		/// <summary>
		/// Gets the number of bits per sample setting of the <see cref="IAudioDevice"/>
		/// </summary>
		/// <returns>The number of bits per sample</returns>
		ushort getBitDepth();

		/// <summary>
		/// Sets the number of bits per sample setting of the <see cref="IAudioDevice"/>
		/// </summary>
		/// <param name="newBPS">The new number of bits per sample</param>
		void setBitDepth(ushort newBPS);

		/// <summary>
		/// Gets the block align, that is the number of bytes/sample.
		/// Convenience for <c><see cref="getNumberOfChannels"/>()*<see cref="getBitDepth"/>()/8</c>
		/// </summary>
		/// <returns>The bloick align</returns>
		ushort getBlockAlign();

		/// <summary>
		/// Gets the byte rate of the <see cref="IAudioDevice"/>.
		/// Convenience for <c><see cref="getSampleRate"/>()*<see cref="getBlockAlign"/>()</c>
		/// </summary>
		/// <returns>The byte rate in bytes/second</returns>
		uint getByteRate();

		/// <summary>
		/// Gets the current <see cref="AudioDeviceState"/>
		/// </summary>
		/// <returns></returns>
		AudioDeviceState getState();

		/// <summary>
		/// Gets the current time during playback and recording
		/// </summary>
		/// <returns>The current time</returns>
		TimeSpan getCurrentTime();

		/// <summary>
		/// Gets the time equivalent of a given <see cref="byte"/> position
		/// </summary>
		/// <param name="pos">The given <see cref="byte"/> position</param>
		/// <returns>The time equivalent as a <see cref="TimeSpan"/></returns>
		TimeSpan getTimeEquivalent(long pos);

		/// <summary>
		/// Gets the position equivalent of a given time
		/// </summary>
		/// <param name="time">The given time</param>
		/// <returns>The position equivalent</returns>
		long getPositionEquivalent(TimeSpan time);
	}
}
