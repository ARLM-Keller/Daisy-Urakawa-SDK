package org.daisy.urakawa.media.timing;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Time duration (could be in milliseconds, SMPTE, etc.). This really is an
 * interface "lollypop" that should be extended. Typically, methods like
 * getTimeMilliseconds(), getTimeSMPTE(), etc. should be available to the
 * end-user of the API. Can be a 0/positive value in the current local timebase.
 * (cannot be negative)
 * 
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface TimeDelta {
	long getTimeDeltaAsMilliseconds();

	double getTimeDeltaAsMillisecondFloat();

	void setTimeDelta(long timeDeltaAsMS);

	void setTimeDelta(double timeDeltaAsMSF);

	/**
	 * @param other
	 * @return
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	TimeDelta addTimeDelta(TimeDelta other)
			throws MethodParameterIsNullException;
}