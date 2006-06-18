using System;

//TODO
//confirm namespace name
namespace urakawa.project
{
	/// <summary>
	/// Represents a projects - part of the facade API, provides methods for opening and saving XUK files
	/// </summary>
	public class Project
	{
		urakawa.core.Presentation mPresentation = null;

    /// <summary>
    /// Default constructor
    /// </summary>
		public Project()
		{
			
		}

	
		/// <summary>
		/// Opens an XUK file and loads the project from this
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the source XUK file</param>
		/// <returns>A <see cref="bool"/> indicating if the XUK file was succesfully opened and loaded</returns>
		public bool openXUK(Uri fileUri)
		{
			mPresentation = new urakawa.core.Presentation();

			System.Xml.XmlTextReader source = new System.Xml.XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;

			//move to the Presentation element
			while (! (source.Name == "Presentation" && 
				source.NodeType == System.Xml.XmlNodeType.Element)
				&&
				source.EOF == false)
			{
				source.Read();
			}

			bool didItWork = mPresentation.XUKin(source);

			return didItWork;
		}

		/// <summary>
		/// Saves the <see cref="Project"/> to a XUK file
		/// </summary>
		/// <param name="fileUri">The <see cref="Uri"/> of the destination XUK file</param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="Project"/> was succesfully saved to XUK</returns>
		public bool saveXUK(Uri fileUri)
		{
			//@todo
			//we should probably track the file encoding in the future
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(fileUri.LocalPath, System.Text.UnicodeEncoding.UTF8);
			writer.Indentation = 1;
			writer.IndentChar = ' ';
			writer.Formatting = System.Xml.Formatting.Indented;

			writeBeginningOfFile(writer);
			writeFakeMetadata(writer);

			bool didItWork = false;
			
			if (mPresentation != null)
				didItWork = mPresentation.XUKout(writer);

			writeEndOfFile(writer);

			return didItWork;
		}

		/// <summary>
		/// Gets the <see cref="urakawa.core.Presentation"/> of the <see cref="Project"/>
		/// </summary>
		/// <returns></returns>
		public urakawa.core.IPresentation getPresentation()
		{
			return mPresentation;
		}

		public void appendMetadata(object metadata)
		{
			// TODO:  Add Project.appendMetadata implementation
		}

		public System.Collections.IList getMetadataList()
		{
			// TODO:  Add Project.getMetadataList implementation
			return null;
		}

		public System.Collections.IList getMetadataList(string name)
		{
			// TODO:  Add Project.urakawa.project.IMetadata.getMetadataList implementation
			return null;
		}

		public void deleteMetadata(string name)
		{
			// TODO:  Add Project.deleteMetadata implementation
		}

		void urakawa.project.IMetadata.deleteMetadata(object metadata)
		{
			// TODO:  Add Project.urakawa.project.IMetadata.deleteMetadata implementation
		}


		
    
		private void writeBeginningOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("XUK");
			writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", 
				"http://www.w3.org/2001/XMLSchema-instance", "xuk.xsd");
			
		}

		private void writeEndOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Close();
		}

		/// <summary>
		/// this function will go away soon
		/// </summary>
		/// <param name="writer"></param>
		private void writeFakeMetadata(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("metaData");
			writer.WriteStartElement("key");
			writer.WriteAttributeString("value", "dc:Generator");
			writer.WriteAttributeString("name", "UrakawaToolkit build " + System.DateTime.Now.ToShortDateString());
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}
