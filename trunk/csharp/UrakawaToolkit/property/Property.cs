using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using urakawa.core;
using urakawa.xuk;

namespace urakawa.property
{
	/// <summary>
	/// Implementation of <see cref="Property"/> that in it self does nothing. 
	/// This class is intended as a base class for built-in or custom implementations of <see cref="Property"/>
	/// </summary>
	public class Property : WithPresentation, IXukAble, IValueEquatable<Property>, urakawa.events.IChangeNotifier
	{

		#region IChangeNotifier Members

		/// <summary>
		/// Event fired after the <see cref="Property"/> has changed. 
		/// The event fire before any change specific event 
		/// </summary>
		public event EventHandler<urakawa.events.DataModelChangeEventArgs> changed;
		/// <summary>
		/// Fires the <see cref="changed"/> event 
		/// </summary>
		/// <param name="args">The arguments of the event</param>
		protected void notifyChanged(urakawa.events.DataModelChangeEventArgs args)
		{
			EventHandler<urakawa.events.DataModelChangeEventArgs> d = changed;
			if (d != null) d(this, args);
		}
		#endregion

		/// <summary>
		/// Default constructor - should only be used from subclass constructors or <see cref="IGenericPropertyFactory"/>s
		/// </summary>
		protected internal Property()
		{
		}

		private TreeNode mOwner = null;

		/// <summary>
		/// Tests if a the <see cref="Property"/> can be validly added to a given potential owning <see cref="TreeNode"/>
		/// </summary>
		/// <param name="potentialOwner">The potential new owner</param>
		/// <returns>A <see cref="bool"/> indicating if the property can be added</returns>
		/// <exception cref="exception.MethodParameterIsNullException">
		/// Thrown when <paramref name="potentialOwner"/> is <c>null</c>
		/// </exception>
		public virtual bool canBeAddedTo(TreeNode potentialOwner)
		{
			if (potentialOwner == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not test if the Property can be added to a null TreeNode");
			}
			return true;
		}

		/// <summary>
		/// Creates a copy of the property
		/// </summary>
		/// <returns>The copy</returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown if the property has not been initialized with an owning <see cref="TreeNode"/>
		/// </exception>
		/// <exception cref="exception.FactoryCannotCreateTypeException">
		/// Thrown if the <see cref="IGenericPropertyFactory"/> associated with the property via. it's owning <see cref="TreeNode"/>
		/// can not create an <see cref="Property"/> mathcing the Xuk QName of <c>this</c>
		/// </exception>
		/// <remarks>
		/// In subclasses of <see cref="Property"/> the implementor should override <see cref="copyProtected"/> and if the impelemntor
		/// wants the copy method of his subclass to have "correct" type he should create a new version of <see cref="copy"/> 
		/// that delegates the copy operation to <see cref="copyProtected"/> followed by type casting. 
		/// See <see cref="urakawa.property.xml.XmlProperty.copy"/>
		/// and <see cref="urakawa.property.xml.XmlProperty.copyProtected"/> for an example of this.
 		/// </remarks>
		public Property copy()
		{
			return copyProtected();
		}
		
		/// <summary>
		/// Protected version of <see cref="copy"/>. Override this method in subclasses to copy additional data
		/// </summary>
		/// <returns>A copy of <c>this</c></returns>
		protected virtual Property copyProtected()
		{
			Property theCopy = getTreeNodeOwner().getPresentation().getPropertyFactory().createProperty(
				getXukLocalName(), getXukNamespaceUri());
			if (theCopy == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The PropertyFactory can not create a Property of type matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			return theCopy;
		}

		/// <summary>
		/// Gets a property with identical content to this but compatible with a given destination <see cref="Presentation"/>
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported property</returns>
		public Property export(Presentation destPres)
		{
			return exportProtected(destPres);
		}

		/// <summary>
		/// Gets a property with identical content to this but compatible with a given destination <see cref="Presentation"/>.
		/// Override this method in subclasses to export additional data
		/// </summary>
		/// <param name="destPres">The destination presentation</param>
		/// <returns>The exported property</returns>
		protected virtual Property exportProtected(Presentation destPres)
		{
			Property exportedProp = destPres.getPropertyFactory().createProperty(getXukLocalName(), getXukNamespaceUri());
			if (exportedProp == null)
			{
				throw new exception.FactoryCannotCreateTypeException(String.Format(
					"The PropertyFactory of the export destination Presentation can not create a Property of type matching QName {1}:{0}",
					getXukLocalName(), getXukNamespaceUri()));
			}
			return exportedProp;
		}

		/// <summary>
		/// Gets the owner <see cref="TreeNode"/> of the property
		/// </summary>
		/// <returns>The owner</returns>
		public TreeNode getTreeNodeOwner()
		{
			if (mOwner == null)
			{
				throw new exception.IsNotInitializedException(
					"The Property has not been initialized with an owning TreeNode");
			}
			return mOwner;
		}

		/// <summary>
		/// Sets the owner <see cref="TreeNode"/> of the property - for internal use only
		/// </summary>
		/// <param name="newOwner">The new owner</param>
		/// <exception cref="exception.PropertyAlreadyHasOwnerException">
		/// Thrown when the setting the new owner to a non-<c>null</c> value 
		/// and the property already has a different owning <see cref="TreeNode"/>
		/// </exception>
		public virtual void setTreeNodeOwner(TreeNode newOwner)
		{
			if (newOwner != null)
			{
				if (mOwner != null && newOwner != mOwner)
				{
					throw new exception.PropertyAlreadyHasOwnerException(
						"The Property is already owner by a different TreeNode");
				}
				if (newOwner.getPresentation() != getPresentation())
				{
					throw new exception.NodeInDifferentPresentationException(
						"The property can not have a owning TreeNode from a different Presentation");
				}
			}
			mOwner = newOwner;
		}
		
		#region IXUKAble members

		/// <summary>
		/// Reads the <see cref="Property"/> from a Property xuk element
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		public void xukIn(XmlReader source)
		{
			if (source == null)
			{
				throw new exception.MethodParameterIsNullException("Can not xukIn from an null source XmlReader");
			}
			if (source.NodeType != XmlNodeType.Element)
			{
				throw new exception.XukException("Can not read Property from a non-element node");
			}
			try
			{
				xukInAttributes(source);
				if (!source.IsEmptyElement)
				{
					while (source.Read())
					{
						if (source.NodeType == XmlNodeType.Element)
						{
							xukInChild(source);
						}
						else if (source.NodeType == XmlNodeType.EndElement)
						{
							break;
						}
						if (source.EOF) throw new exception.XukException("Unexpectedly reached EOF");
					}
				}

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukIn of Property: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Reads the attributes of a Property xuk element.
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInAttributes(XmlReader source)
		{
		}

		/// <summary>
		/// Reads a child of a Property xuk element. 
		/// </summary>
		/// <param name="source">The source <see cref="XmlReader"/></param>
		protected virtual void xukInChild(XmlReader source)
		{
			bool readItem = false;
			// Read known children, when read set readItem to true


			if (!(readItem || source.IsEmptyElement))
			{
				source.ReadSubtree().Close();//Read past unknown child 
			}
		}

		/// <summary>
		/// Write a Property element to a XUK file representing the <see cref="Property"/> instance
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		public void xukOut(XmlWriter destination, Uri baseUri)
		{
			if (destination == null)
			{
				throw new exception.MethodParameterIsNullException(
					"Can not xukOut to a null XmlWriter");
			}

			try
			{
				destination.WriteStartElement(getXukLocalName(), getXukNamespaceUri());
				xukOutAttributes(destination, baseUri);
				xukOutChildren(destination, baseUri);
				destination.WriteEndElement();

			}
			catch (exception.XukException e)
			{
				throw e;
			}
			catch (Exception e)
			{
				throw new exception.XukException(
					String.Format("An exception occured during xukOut of Property: {0}", e.Message),
					e);
			}
		}

		/// <summary>
		/// Writes the attributes of a Property element
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutAttributes(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Write the child elements of a Property element.
		/// </summary>
		/// <param name="destination">The destination <see cref="XmlWriter"/></param>
		/// <param name="baseUri">
		/// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
		/// if <c>null</c> absolute <see cref="Uri"/>s are written
		/// </param>
		protected virtual void xukOutChildren(XmlWriter destination, Uri baseUri)
		{

		}

		/// <summary>
		/// Gets the local name part of the QName representing a <see cref="Property"/> in Xuk
		/// </summary>
		/// <returns>The local name part</returns>
		public virtual string getXukLocalName()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// Gets the namespace uri part of the QName representing a <see cref="Property"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri part</returns>
		public virtual string getXukNamespaceUri()
		{
			return urakawa.ToolkitSettings.XUK_NS;
		}

		#endregion

		#region IValueEquatable<Property> Members

		/// <summary>
		///	Determines if a given other <see cref="Property"/> has the same value as <c>this</c>
		/// </summary>
		/// <param name="other">The other property</param>
		/// <returns>A <see cref="bool"/> indicating the value equality</returns>
		public virtual bool valueEquals(Property other)
		{
			if (!this.GetType().IsInstanceOfType(other)) return false;
			return true;
		}

		#endregion
	}
}
