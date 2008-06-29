using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace urakawa.examples
{
	/// <summary>
	/// Example implementation of a custon <see cref="CoreNode"/>
	/// </summary>
	public class ExampleCustomCoreNode : CoreNode
	{
		internal ExampleCustomCoreNode(ICorePresentation pres)
			: base(pres)
		{
			mCustomCoreNodeData = "";
			mLabel = "";
		}

		/// <summary>
		/// Override for default <see cref="Object.ToString"/> method. 
		/// Appends the value of <see cref="Label"/> to the default implementation output
		/// </summary>
		/// <returns>The string representation of <c>this</c> including the <see cref="Label"/> value</returns>
		public override string ToString()
		{
			return String.Format(
				"{0} (Label={1})", base.ToString(), Label);
		}

		/// <summary>
		/// A piece of data to decern the <see cref="ExampleCustomCoreNode"/> from a standard <see cref="CoreNode"/>
		/// </summary>
		public string CustomCoreNodeData
		{
			get
			{
				return mCustomCoreNodeData;
			}
			set
			{
				mCustomCoreNodeData = value;
			}
		}
		private string mCustomCoreNodeData;

		/// <summary>
		/// Gets or sets the label of <c>this</c>
		/// </summary>
		public string Label
		{
			get
			{
				return mLabel;
			}
			set
			{
				mLabel = value;
			}
		}
		private string mLabel;

		/// <summary>
		/// Copies the <see cref="ExampleCustomCoreNode"/>
		/// </summary>
		/// <param name="deep">If	true,	then include the node's	entire subtree.	 
		///	Otherwise, just	copy the node	itself.</param>
		///	<returns>A <see	cref="ExampleCustomCoreNode"/>	containing the copied	data.</returns>
		///	<exception cref="urakawa.exception.FactoryCanNotCreateTypeException">
		/// Thrown when the <see cref="CoreNodeFactory"/> of the <see cref="Presentation"/> to which the instance belongs
		/// can not create an <see cref="ExampleCustomCoreNode"/> instance
		///	</exception>
		public new ExampleCustomCoreNode copy(bool deep)
		{
			ExampleCustomCoreNode theCopy = this.getPresentation().getCoreNodeFactory().createNode(
				"ExampleCustomCoreNode",
				urakawa.ToolkitSettings.XUK_NS) as ExampleCustomCoreNode;
			if (theCopy == null)
			{
				throw new urakawa.exception.FactoryCanNotCreateTypeException(String.Format(
					"The CoreNode factory of the Presentation can not create an {0}:ExampleCustomCoreNode",
					urakawa.ToolkitSettings.XUK_NS));
			}
			theCopy.CustomCoreNodeData = CustomCoreNodeData;
			theCopy.Label = Label;
			copyProperties(theCopy);
			if (deep)
			{
				copyChildren(theCopy);
			}
			return theCopy;
		}

		/// <summary>
		/// Reads the attributes of a ExampleCustomCoreNode xml element
		/// </summary>
		/// <param name="source">The source <see cref="System.Xml.XmlReader"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully read</returns>
		protected override bool XukInAttributes(System.Xml.XmlReader source)
		{
			CustomCoreNodeData = source.GetAttribute("CustomCoreNodeData");
			Label = source.GetAttribute("Label");
			return base.XukInAttributes(source);
		}

		/// <summary>
		/// Writes the attributes of a ExampleCustomCoreNode xml element
		/// </summary>
		/// <param name="wr">The destination <see cref="System.Xml.XmlWriter"/></param>
		/// <returns>A <see cref="bool"/> indicating if the attributes were succesfully written</returns>
		protected override bool XukOutAttributes(System.Xml.XmlWriter wr)
		{
			wr.WriteAttributeString("CustomCoreNodeData", CustomCoreNodeData);
			wr.WriteAttributeString("Label", Label);
			return base.XukOutAttributes(wr);
		}

		/// <summary>
		/// Returns the namespace uri of the QName rpresenting a <see cref="ExampleCustomCoreNode"/> in Xuk
		/// </summary>
		/// <returns>The namespace uri</returns>
		public override string getXukNamespaceUri()
		{
			return ExampleCustomCoreNodeFactory.EX_CUST_NS;
		}
	}
}