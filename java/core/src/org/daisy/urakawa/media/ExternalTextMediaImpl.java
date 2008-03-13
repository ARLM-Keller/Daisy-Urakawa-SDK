package org.daisy.urakawa.media;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URI;
import java.net.URISyntaxException;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.event.ChangeListener;
import org.daisy.urakawa.event.ChangeNotifier;
import org.daisy.urakawa.event.ChangeNotifierImpl;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.event.media.TextChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ExternalTextMediaImpl extends ExternalMediaAbstractImpl implements
		TextMedia {
	/**
	 * 
	 */
	public ExternalTextMediaImpl() {
	}

	@Override
	public <K extends DataModelChangedEvent> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (TextChangedEvent.class.isAssignableFrom(event.getClass())) {
			mTextChangedEventNotifier.notifyListeners(event);
		}
		super.notifyListeners(event);
	}

	@Override
	public <K extends DataModelChangedEvent> void registerListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (TextChangedEvent.class.isAssignableFrom(klass)) {
			mTextChangedEventNotifier.registerListener(listener, klass);
		} else {
			super.registerListener(listener, klass);
		}
	}

	@Override
	public <K extends DataModelChangedEvent> void unregisterListener(
			ChangeListener<K> listener, Class<K> klass)
			throws MethodParameterIsNullException {
		if (TextChangedEvent.class.isAssignableFrom(klass)) {
			mTextChangedEventNotifier.unregisterListener(listener, klass);
		} else {
			super.unregisterListener(listener, klass);
		}
	}

	protected ChangeNotifier<DataModelChangedEvent> mTextChangedEventNotifier = new ChangeNotifierImpl();

	@Override
	public boolean isContinuous() {
		return false;
	}

	@Override
	public boolean isDiscrete() {
		return true;
	}

	@Override
	public boolean isSequence() {
		return false;
	}

	@Override
	public ExternalTextMediaImpl copy() {
		return (ExternalTextMediaImpl) copyProtected();
	}

	@Override
	protected Media exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		ExternalTextMediaImpl exported = (ExternalTextMediaImpl) super
				.exportProtected(destPres);
		if (exported == null) {
			throw new FactoryCannotCreateTypeException();
		}
		return exported;
	}

	@Override
	public ExternalTextMediaImpl export(Presentation destPres)
			throws FactoryCannotCreateTypeException,
			MethodParameterIsNullException {
		if (destPres == null) {
			throw new MethodParameterIsNullException();
		}
		return (ExternalTextMediaImpl) exportProtected(destPres);
	}

	public String getText() {
		BufferedReader reader = null;
		try {
			reader = new BufferedReader(new InputStreamReader(getURI().toURL()
					.openStream()));
		} catch (MalformedURLException e) {
			e.printStackTrace();
			return "";
		} catch (IOException e) {
			e.printStackTrace();
			return "";
		} catch (URISyntaxException e) {
			e.printStackTrace();
			return "";
		}
		StringBuffer strText = new StringBuffer();
		String str = null;
		do {
			try {
				str = reader.readLine();
			} catch (IOException e) {
				e.printStackTrace();
				return "";
			}
			if (str == "") {
				strText.append("\n");
			} else if (str != null) {
				strText.append(str);
				strText.append("\n");
			}
		} while (str != null);
		try {
			reader.close();
		} catch (IOException e) {
			e.printStackTrace();
			return "";
		}
		mText = str;
		return str;
	}

	private String mText = "";

	public void setText(String text) throws MethodParameterIsNullException {
		if (text == null) {
			throw new MethodParameterIsNullException();
		}
		String prevTxt = mText;
		URI uri;
		try {
			uri = getURI();
		} catch (URISyntaxException e) {
			e.printStackTrace();
			return;
		}
		if (uri.getScheme() != "file") {
			return;
		}
		String path = uri.getPath();
		BufferedWriter writer = null;
		try {
			writer = new BufferedWriter(new FileWriter(path));
		} catch (IOException e) {
			e.printStackTrace();
			return;
		}
		try {
			writer.write(text);
		} catch (IOException e) {
			e.printStackTrace();
		}
		try {
			writer.close();
		} catch (IOException e) {
			e.printStackTrace();
			return;
		}
		mText = text;
		notifyListeners(new TextChangedEvent(this, mText, prevTxt));
	}
}