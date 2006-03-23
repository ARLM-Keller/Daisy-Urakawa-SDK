package org.daisy.urakawa.exceptions;

/**
 * Some methods forbid passing NULL values.
 * This exception should be raised when NULL values are passed.
 */
class MethodParameterIsNull extends MethodParameterIsInvalid {}