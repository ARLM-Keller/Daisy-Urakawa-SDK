using System;
using System.Xml;
using urakawa.core;
using urakawa.property;
using urakawa.property.xml;

namespace urakawa.examples
{
    /// <summary>
    /// Example implementation of a custom <see cref="Property"/>
    /// </summary>
    public class ExampleCustomProperty : XmlProperty
    {
        /// <summary>
        /// The Xuk namespace uri of <see cref="ExampleCustomTreeNode"/>s 
        /// </summary>
        public new const string XUK_NS = "http://www.daisy.org/urakawa/example";

        /// <summary>
        /// The data of the custom property
        /// </summary>
        public string CustomData = "";

        /// <summary>
        /// Creates a copy of the <see cref="ExampleCustomProperty"/>
        /// </summary>
        /// <returns>The copy</returns>
        public new ExampleCustomProperty Copy()
        {
            return CopyProtected() as ExampleCustomProperty;
        }

        /// <summary>
        /// Protected version of <see cref="Copy"/> - in place as part of a technicality to have <see cref="Copy"/>
        /// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected override Property CopyProtected()
        {
            ExampleCustomProperty exProp = (ExampleCustomProperty) base.CopyProtected();

            exProp.CustomData = CustomData;
            return exProp;
        }

        /// <summary>
        /// Exports the <see cref="ExampleCustomProperty"/> to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported <see cref="ExampleCustomProperty"/></returns>
        public new ExampleCustomProperty Export(Presentation destPres)
        {
            return ExportProtected(destPres) as ExampleCustomProperty;
        }

        /// <summary>
        /// Protected version of <see cref="Export"/> - in place as part of a technicality to have <see cref="Export"/>
        /// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
        /// </summary>
        /// <returns>The export</returns>
        protected override Property ExportProtected(Presentation destPres)
        {
            ExampleCustomProperty exProp = base.ExportProtected(destPres) as ExampleCustomProperty;
            if (exProp == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The property factory can not create a ExampleCustomProperty matching QName {0}:{1}",
                                                                         XukNamespaceUri, XukLocalName));
            }
            exProp.CustomData = CustomData;
            return exProp;
        }

        #region IXUKAble Members

        /// <summary>
        /// Reads data from the attributes of the ExampleCustomProperty element
        /// </summary>
        /// <param name="source">The source xml reader</param>
        protected override void XukInAttributes(XmlReader source)
        {
            base.XukInAttributes(source);
            CustomData = source.GetAttribute("customData");
        }

        /// <summary>
        /// Writes data to attributes of the ExampleCustomProperty element
        /// </summary>
        /// <param name="destination">The destination xml writer</param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (CustomData != null)
            {
                destination.WriteAttributeString("customData", CustomData);
            }
            base.XukOutAttributes(destination, baseUri);
        }

        #endregion

        #region IValueEquatable<Property> Members
        public override bool ValueEquals(WithPresentation other)
        {
            if (!base.ValueEquals(other))
            {
                return false;
            }

            ExampleCustomProperty otherz = other as ExampleCustomProperty;
            if (otherz == null)
            {
                return false;
            }

            if (CustomData != otherz.CustomData) return false;

            return true;
        }

        #endregion
    }
}