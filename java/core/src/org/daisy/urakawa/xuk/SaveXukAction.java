package org.daisy.urakawa.xuk;

import java.net.URI;

import org.daisy.urakawa.Project;
import org.daisy.urakawa.command.CommandCannotExecuteException;
import org.daisy.urakawa.event.CancellableEvent;
import org.daisy.urakawa.event.Event;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.EventHandlerImpl;
import org.daisy.urakawa.event.EventListener;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.progress.ProgressAction;
import org.daisy.urakawa.progress.ProgressCancelledException;

/**
 *
 */
public class SaveXukAction extends ProgressAction implements
		EventListener<CancellableEvent> {
	protected EventHandler<Event> mEventNotifier = new EventHandlerImpl();
	private XmlDataWriter mWriter;
	private URI mBaseUri;
	private Project mProject;

	/**
	 * @param proj
	 * @param writer
	 * @param baseUri
	 * @throws MethodParameterIsNullException
	 */
	public SaveXukAction(Project proj, XmlDataWriter writer, URI baseUri)
			throws MethodParameterIsNullException {
		mWriter = writer;
		mBaseUri = baseUri;
		mProject = proj;
		//
		if (mWriter == null || mBaseUri == null || mProject == null) {
			throw new MethodParameterIsNullException();
		}
	}

	public boolean canExecute() {
		return true;
	}

	@SuppressWarnings("unused")
	public void execute() throws CommandCannotExecuteException {
		//
		mWriter.writeStartDocument();
		mWriter.writeStartElement("Xuk", XukAble.XUK_NS);
		// TODO: add schema declaration in XML header
		try {
			registerListener(this, CancellableEvent.class);
		} catch (MethodParameterIsNullException e1) {
			System.out.println("WTF ?! This should never happen !");
			e1.printStackTrace();
		}
		try {
			mProject.xukOut(mWriter, mBaseUri, this);
			notifyFinished();
		} catch (MethodParameterIsNullException e) {
			System.out.println("WTF ?! This should never happen !");
			e.printStackTrace();
		} catch (XukSerializationFailedException e) {
			throw new RuntimeException(e);
		} catch (ProgressCancelledException e) {
			notifyCancelled();
		} finally {
			try {
				unregisterListener(this, CancellableEvent.class);
			} catch (MethodParameterIsNullException e) {
				System.out.println("WTF ?! This should never happen !");
				e.printStackTrace();
			}
		}
		mWriter.writeEndElement();
		mWriter.writeEndDocument();
	}

	public String getLongDescription() {
		return null;
	}

	public String getShortDescription() {
		return null;
	}

	public void setLongDescription(String str)
			throws MethodParameterIsNullException {
	}

	public void setShortDescription(String str)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
	}

	public void notifyProgress() {
		// TODO Auto-generated method stub
	}

	public <K extends Event> void notifyListeners(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.notifyListeners(event);
	}

	public <K extends Event> void registerListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.registerListener(listener, klass);
	}

	public <K extends Event> void unregisterListener(EventListener<K> listener,
			Class<K> klass) throws MethodParameterIsNullException {
		if (listener == null || klass == null) {
			throw new MethodParameterIsNullException();
		}
		mEventNotifier.unregisterListener(listener, klass);
	}

	public <K extends CancellableEvent> void eventCallback(K event)
			throws MethodParameterIsNullException {
		if (event == null) {
			throw new MethodParameterIsNullException();
		}
		if (event.isCancelled()) {
			requestCancel();
		}
	}
}
