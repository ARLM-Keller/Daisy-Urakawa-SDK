package org.daisy.urakawa.command;

import java.net.URI;

import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.IXmlDataReader;
import org.daisy.urakawa.nativeapi.IXmlDataWriter;
import org.daisy.urakawa.progress.IProgressHandler;
import org.daisy.urakawa.progress.ProgressCancelledException;
import org.daisy.urakawa.xuk.IXukAble;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 */
public final class CommandFactory extends WithPresentation implements
		ICommandFactory {
	public ICommand createCommand(String xukLocalName, String xukNamespaceURI)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (xukLocalName == null || xukNamespaceURI == null) {
			throw new MethodParameterIsNullException();
		}
		if (xukLocalName.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		if (xukNamespaceURI == IXukAble.XUK_NS) {
			if (xukLocalName == ICompositeCommand.class.getSimpleName()) {
				return createCompositeCommand();
			}
		}
		return null;
	}

	public ICompositeCommand createCompositeCommand() {
		ICompositeCommand newCmd = new CompositeCommand();
		try {
			newCmd.setPresentation(getPresentation());
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return newCmd;
	}

	@Override
	protected void clear() {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(IXmlDataReader source, IProgressHandler ph)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutChildren(IXmlDataWriter destination, URI baseUri,
			IProgressHandler ph) throws XukSerializationFailedException,
			MethodParameterIsNullException, ProgressCancelledException {
	}
}