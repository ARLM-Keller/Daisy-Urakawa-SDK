using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Channel : IChannel
	{
    private string mName;

    internal Channel(string name)
    {
      mName = name;
    }
    #region IChannel Members

    /// <summary>
    /// Sets the name of the <see cref="IChannel"/>
    /// </summary>
    /// <param name="name">The new name</param>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="name"/> is null
    /// </exception>
    /// <exception cref="exception.MethodParameterIsEmptyStringException">
    /// Thrown when <paramref name="name"/> is an empty string
    /// </exception>
    public void setName(string name)
    {
      if (mName==null) 
      {
        throw new exception.MethodParameterIsNullException(
          "Can not set channel name to null");
      }
      if (mName==String.Empty)
      {
        throw new exception.MethodParameterIsEmptyStringException(
          "Can not set channel name to the empty string");
      }
      mName = name;
    }

    /// <summary>
    /// Gets the name of the <see cref="IChannel"/>
    /// </summary>
    /// <returns>The name</returns>
    public string getName()
    {
      return mName;
    }

    /// <summary>
    /// Checks of a given <see cref="media.MediaType"/> is supported by the channel
    /// </summary>
    /// <param name="type">The <see cref="media.MediaType"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="media.MediaType"/>
    /// is supported</returns>
    /// <remarks>
    /// Always returns <c>true</c> - model does not specify what support for a <see cref="media.MediaType"/>
    /// means
    /// </remarks>
    public bool isMediaTypeSupported(urakawa.media.MediaType type)
    {
      return true;
    }

    #endregion
  }
}
