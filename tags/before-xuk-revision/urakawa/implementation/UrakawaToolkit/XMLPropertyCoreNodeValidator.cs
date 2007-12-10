using System;

namespace urakawa.core
{
  /// <summary>
  /// <see cref="ICoreNodeValidator"/> that validates the <see cref="IXmlProperty"/> aspects
  /// of <see cref="ICoreNode"/>s
  /// </summary>
  public class XMLPropertyCoreNodeValidator : ICoreNodeValidator
	{
		internal XMLPropertyCoreNodeValidator()
		{
		}

		#region ICoreNodeValidator Members

    /// <summary>
    /// Determines if a given <see cref="IProperty"/> can be set for a given context <see cref="ICoreNode"/>.
    /// Only <see cref="IProperty"/>s implementing <see cref="IXmlProperty"/>s are tested. For all other <see cref="IProperty"/> types the test succeeds
    /// </summary>
    /// <param name="newProp">The given <see cref="IProperty"/></param>
    /// <param name="contextNode">The comntext <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="IProperty"/> can be set</returns>
    public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
		{
			if(newProp==null || contextNode==null)
			{
				throw(new urakawa.exception.MethodParameterIsNullException("Method canSetProperty does not accept null value arguments."));
			}

			if(!(newProp.GetType().IsAssignableFrom(typeof(XmlProperty))))
				return true; //only test XmlProperty

			if(contextNode.getParent()==null)
				return true; //allow everything for detached nodes

			if(!contextNode.getPresentation().GetType().IsInstanceOfType(typeof(Presentation)))
				return true;	//only validate nodes that are owned by a proper Presentation

			ICoreNode nearestParentWithXmlProperty = (ICoreNode)contextNode.getParent();
			while(nearestParentWithXmlProperty != null)
			{
				if(nearestParentWithXmlProperty.getProperty(typeof(XmlProperty))!=null)
					break;
				else
					nearestParentWithXmlProperty = (ICoreNode)nearestParentWithXmlProperty.getParent();
			}

			if(nearestParentWithXmlProperty != null)
			{
				CanSetXmlPropertyFragmentBuilder checkParentFragment = new CanSetXmlPropertyFragmentBuilder();
				checkParentFragment.mContextNode = contextNode;
				checkParentFragment.rootNode = nearestParentWithXmlProperty;
				checkParentFragment.mNewXmlProp = (XmlProperty)newProp;

				nearestParentWithXmlProperty.acceptDepthFirst(checkParentFragment);
				string nameOfRoot = (((XmlProperty)nearestParentWithXmlProperty.getProperty(typeof(XmlProperty))).getNamespace()=="")?((XmlProperty)nearestParentWithXmlProperty.getProperty(typeof(XmlProperty))).getNamespace()+":":"" +   ((XmlProperty)nearestParentWithXmlProperty.getProperty(typeof(XmlProperty))).getName();

				if(!XmlProperty.testFragment((Presentation)contextNode.getPresentation(),nameOfRoot,checkParentFragment.mFragment,System.Xml.XmlNodeType.Element))
					return false;
			}
			else
			{
				XmlDetector tmpDet = new XmlDetector();
				contextNode.getPresentation().getRootNode().acceptDepthFirst(tmpDet);
				if(tmpDet.mXmlDetected)
					return false; //can't set more than one XML root node, and the contextnode is not a child of the current XML root.
			}

			CanSetXmlPropertyFragmentBuilder checkChildren = new CanSetXmlPropertyFragmentBuilder();
			checkChildren.mContextNode = contextNode;
			checkChildren.rootNode = contextNode;
			checkChildren.mNewXmlProp = (XmlProperty)newProp;
			contextNode.acceptDepthFirst(checkChildren);

			string nameOfNewRoot = (((XmlProperty)newProp).getNamespace()=="")?((XmlProperty)newProp).getNamespace()+":":"" + ((XmlProperty)newProp).getName();
			if(!XmlProperty.testFragment((Presentation)contextNode.getPresentation(),nameOfNewRoot,checkChildren.mFragment,System.Xml.XmlNodeType.Element))
				return false;

			return true;
		}

		private class XmlDetector:ICoreNodeVisitor
		{
			public bool mXmlDetected = false;
			#region ICoreNodeVisitor Members

			public bool preVisit(ICoreNode node)
			{
				if(node.getProperty(typeof(XmlProperty)) != null)
				{
					mXmlDetected = true;
					return false;
				}
				return true;
			}

			public void postVisit(ICoreNode node)
			{
			}

			#endregion

		}

		private class CanSetXmlPropertyFragmentBuilder:ICoreNodeVisitor
		{
			public string mFragment = "";
			public XmlProperty mNewXmlProp;
			public ICoreNode mContextNode;
			public ICoreNode rootNode;

			#region ICoreNodeVisitor Members

			public bool preVisit(ICoreNode node)
			{
				IXmlProperty propertyToUse = (IXmlProperty) node.getProperty(typeof(XmlProperty));
				if(node==mContextNode)
					propertyToUse = mNewXmlProp;

				bool hasXmlProp = true;
				if(propertyToUse == null)
					hasXmlProp = false;

				if(node == rootNode)
				{
					mFragment += "<" + ((propertyToUse.getNamespace()=="")?propertyToUse.getNamespace() + ":":"") + propertyToUse.getName() + " >";
				}
				else
				{
					mFragment += "<" + ((propertyToUse.getNamespace()=="")?propertyToUse.getNamespace() + ":":"") + propertyToUse.getName() + " />";
				}
				return (node == rootNode) || (!hasXmlProp);
			}

			public void postVisit(ICoreNode node)
			{
				IXmlProperty propertyToUse = (IXmlProperty) node.getProperty(typeof(XmlProperty));
				if(node==mContextNode)
					propertyToUse = mNewXmlProp;
				if(node == rootNode)
				{
					mFragment += "</" + ((propertyToUse.getNamespace()=="")?propertyToUse.getNamespace() + ":":"") + propertyToUse.getName() + " >";
				}			
			}

			#endregion
		}


    /// <summary>
    /// Determines if a given child <see cref="ICoreNode"/> can be removed it's parent
    /// </summary>
    /// <param name="node">The given child <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can be removed from it's parent</returns>
    public bool canRemoveChild(ICoreNode node)
		{
			if(node.getParent()==null)
				return true;
			if(node.getPresentation()==null)
				return true;
			if(!(node.getPresentation().GetType().IsInstanceOfType(typeof(Presentation))))
				return true;

			ICoreNode nearestAncestryXmlNode = (ICoreNode)node.getParent();
			while(nearestAncestryXmlNode!=null)
			{
				if(nearestAncestryXmlNode.getProperty(typeof(XmlProperty))!=null)
					break;
			}
			if(nearestAncestryXmlNode==null)
				return true; //this node does not have any relation to XmlProperty, so allow anything!

			CanRemoveChildFragmentBuilder tmpFragment = new CanRemoveChildFragmentBuilder();
			tmpFragment.mRemovableNode = node;
			tmpFragment.mRootNode = nearestAncestryXmlNode;

			((ICoreNode)node.getParent()).acceptDepthFirst(tmpFragment);
			if(!XmlProperty.testFragment((Presentation)node.getPresentation(),tmpFragment.mRootName,tmpFragment.mFragment,System.Xml.XmlNodeType.Element))
				return false;
			return true;
		}
		private class CanRemoveChildFragmentBuilder:ICoreNodeVisitor
		{
			public ICoreNode mRootNode;
			public ICoreNode mRemovableNode;
			public string mFragment = "";
			public string mRootName = "";
			#region ICoreNodeVisitor Members

			public bool preVisit(ICoreNode node)
			{
				XmlProperty tmpXml;
				if(!(node.getProperty(typeof(XmlProperty)).GetType().IsInstanceOfType(typeof(XmlProperty))))
					return true;
				tmpXml = (XmlProperty)node.getProperty(typeof(XmlProperty));
				if(tmpXml == null)
					return true;

				if(node==mRootNode)
				{
					mFragment += "<" + tmpXml.getQName() + " >";
					mRootName = tmpXml.getQName();
				}
				else
				{
					if(node!=mRemovableNode)
					{
						mFragment += "<" + tmpXml.getQName() + " />";
					}
					return false;
				}
				return true;
			}

			public void postVisit(ICoreNode node)
			{
				XmlProperty tmpXml;
				if(!(node.getProperty(typeof(XmlProperty)).GetType().IsInstanceOfType(typeof(XmlProperty))))
					return;
				tmpXml = (XmlProperty)node.getProperty(typeof(XmlProperty));
				if(tmpXml == null)
					return;

				if(node==mRootNode)
				{
					mFragment += "</" + tmpXml.getQName() + " >";
				}
			}

			#endregion

		}


    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted as a child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to insert</param>
    /// <param name="index">The index at which to insert</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a child of <paramref name="context"/> 
    /// at index <paramref name="index"/></returns>
    public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
		{
			if(index>0)
				return canInsert(contextNode,(ICoreNode)contextNode.getChild(index-1),node,CanInsertFragmentBuilder.InsertType.InsertAfterAnchor);
			else if(index==0)
				return canInsert(contextNode,null,node,CanInsertFragmentBuilder.InsertType.InsertFirst);
			else
				throw(new urakawa.exception.MethodParameterIsOutOfBoundsException("index should be 0 or positive"));
		}

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted before a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling before <paramref name="anchorNode"/>
    /// </returns>
    public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
		{
			int indexOfAnchor=0;
			for(;indexOfAnchor<anchorNode.getParent().getChildCount();indexOfAnchor++)
			{
				if(anchorNode.getParent().getChild(indexOfAnchor)==anchorNode)
					break;
			}
			if(indexOfAnchor==0)
				return canInsert((ICoreNode)anchorNode.getParent(),null,node,CanInsertFragmentBuilder.InsertType.InsertFirst);
			else
				return canInsert((ICoreNode)anchorNode.getParent(),(ICoreNode)anchorNode.getParent().getChild(indexOfAnchor-1),node,CanInsertFragmentBuilder.InsertType.InsertAfterAnchor);
		}

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted after a given anchor <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling after <paramref name="anchorNode"/>
    /// </returns>
    public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
		{
			if(anchorNode.getParent()==null)
				return true;
			return canInsert((ICoreNode)anchorNode.getParent(),anchorNode,node,CanInsertFragmentBuilder.InsertType.InsertAfterAnchor);
		}

		private bool canInsert(ICoreNode targetNode, ICoreNode anchorNode, ICoreNode newNode, CanInsertFragmentBuilder.InsertType method)
		{
			CanInsertFragmentBuilder testFragment = new CanInsertFragmentBuilder();
			testFragment.mAnchor = anchorNode;
			testFragment.method = method;
			testFragment.mInsertable = newNode;

			ICoreNode xmlParentOfBranch = targetNode;
			while(xmlParentOfBranch.getProperty(typeof(XmlProperty))==null)
			{
				xmlParentOfBranch  = (ICoreNode)xmlParentOfBranch.getParent();
				if(xmlParentOfBranch == null)
					break;
			}
			if(xmlParentOfBranch==null)
			{
				XmlDetector tmpDet = new XmlDetector();
				targetNode.getPresentation().getRootNode().acceptDepthFirst(tmpDet);
				bool bXmlInPresentation = tmpDet.mXmlDetected;

				tmpDet = new XmlDetector();
				newNode.acceptDepthFirst(tmpDet);
				bool bXmlInNewNode = tmpDet.mXmlDetected;
				if(bXmlInPresentation && bXmlInNewNode)
					return false;
				else
					return true; //always allows the first XML node for now.
			}

			testFragment.mRoot = xmlParentOfBranch;
			xmlParentOfBranch.acceptDepthFirst(testFragment);
			XmlProperty rootXmlProp = (XmlProperty)xmlParentOfBranch.getProperty(typeof(XmlProperty));

			return XmlProperty.testFragment((Presentation)targetNode.getPresentation(),rootXmlProp.getQName(),testFragment.mFragment,System.Xml.XmlNodeType.Element);
		}

		private class CanInsertFragmentBuilder:ICoreNodeVisitor
		{
			public enum InsertType
			{
				InsertFirst,
				InsertAfterAnchor,
				InsertLast
			};
			public ICoreNode mAnchor;
			public ICoreNode mInsertable;
			public ICoreNode mRoot;
			public string mFragment = "";
			public InsertType method = InsertType.InsertFirst;

			#region ICoreNodeVisitor Members

			public bool preVisit(ICoreNode node)
			{
				XmlProperty currProp = (XmlProperty)node.getProperty(typeof(XmlProperty));
				bool rVal = false; //only go deep if no XML is present on this node.
				if(currProp==null)
					rVal = true; //forward call to children

				if(node==mRoot)
				{
					rVal = true;
					if(currProp != null)
            mFragment += "<" + currProp.getQName() + ">";
					if(method == InsertType.InsertFirst)
					{
						mInsertable.acceptDepthFirst(this);						
					}
				}
				else
				{
					if(currProp!=null)
					{
						mFragment += "<" + currProp.getQName() + " />";
						rVal = false;
					}

					if((mAnchor == node) && (InsertType.InsertAfterAnchor == method))
					{
						node.acceptDepthFirst(this);
					}
				}
				return rVal;
			}

			public void postVisit(ICoreNode node)
			{
				XmlProperty currProp = (XmlProperty)node.getProperty(typeof(XmlProperty));
				if(node==mRoot)
				{
					if(method == InsertType.InsertLast)
					{
						mInsertable.acceptDepthFirst(this);						
					}
					if(currProp != null)
						mFragment += "</" + currProp.getQName() + ">";
				}
			}

			#endregion

		}


    /// <summary>
    /// Determines if a <see cref="ICoreNode"/> can replace another <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to replace with</param>
    /// <param name="oldNode">The <see cref="ICoreNode"/> to be replaced</param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can replace <paramref name="oldNode"/> in the list of children 
    /// of the parent of <paramref name="oldNode"/></returns>
    public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
		{
			if(!(node.getPresentation().GetType().IsInstanceOfType(typeof(Presentation))))
				return true;

			ICoreNode rootNode = (ICoreNode)oldNode.getParent();
			XmlProperty parentXml = null;
			while(rootNode!= null)
			{
				parentXml = (XmlProperty)rootNode.getProperty(typeof(XmlProperty));
				if(parentXml!=null)
					break;
        rootNode = (ICoreNode)rootNode.getParent();
			}

			if(rootNode==null)
			{
				XmlDetector tmpDetector = new XmlDetector();
				node.acceptDepthFirst(tmpDetector);
				bool bXmlInNewFragment = tmpDetector.mXmlDetected;

				tmpDetector = new XmlDetector();
				node.getPresentation().getRootNode().acceptDepthFirst(tmpDetector);
				bool bXmlInTree = tmpDetector.mXmlDetected;

				if(bXmlInNewFragment && bXmlInTree)
					return false; //can't do replace, since it would make there be more than 1 root XML node
				else 
					return true;
			}

			CanReplaceChildFragmentBuilder replFrag = new CanReplaceChildFragmentBuilder();
			replFrag.mParentXmlCarryingNode = rootNode;
			replFrag.mOldChild = oldNode;
			replFrag.mNewChild = node;
			rootNode.acceptDepthFirst(replFrag);

			return XmlProperty.testFragment((Presentation)rootNode.getPresentation(), parentXml.getQName(),replFrag.mFragment,System.Xml.XmlNodeType.Element);
		}
		private class CanReplaceChildFragmentBuilder:ICoreNodeVisitor
		{
			public ICoreNode mParentXmlCarryingNode;
			public ICoreNode mOldChild;
			public ICoreNode mNewChild;
			public string mFragment;

			#region ICoreNodeVisitor Members

			public bool preVisit(ICoreNode node)
			{
				bool visitingRoot = (node==mParentXmlCarryingNode);

				XmlProperty currProp = null;
				if(node==mOldChild)
				{
					currProp = (XmlProperty)mNewChild.getProperty(typeof(XmlProperty));
				}
				else
				{
					currProp = (XmlProperty)node.getProperty(typeof(XmlProperty));
				}

				if(currProp==null)
					return true;

				if(visitingRoot)
				{
					mFragment = "<" + currProp.getQName() + ">";
				}
				else
				{
					mFragment += "<" + currProp.getQName() + " />";
				}

				return visitingRoot || (currProp==null);
			}

			public void postVisit(ICoreNode node)
			{
				bool visitingRoot = (node==mParentXmlCarryingNode);

				XmlProperty currProp = null;
				if(node==mOldChild)
				{
					currProp = (XmlProperty)mNewChild.getProperty(typeof(XmlProperty));
				}
				else
				{
					currProp = (XmlProperty)node.getProperty(typeof(XmlProperty));
				}

				if(currProp==null)
					return;

				if(visitingRoot)
				{
					mFragment = "</" + currProp.getQName() + ">";
				}
			}

			#endregion

		}


    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can replace the child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> with which to replace</param>
    /// <param name="index">The index of the child to replace</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can replace 
    /// the child of <paramref name="contextNode"/> at index <paramref name="index"/></returns>
    public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
		{
			if(index >= contextNode.getChildCount())
				return false;
      ICoreNode oldChild = (ICoreNode)contextNode.getChild(index);				
			return canReplaceChild(node,oldChild);
		}

    /// <summary>
    /// Determines if the child at a given index can be removed from a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="index">The given index</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the child of <paramref name="contentNode"/>
    /// at index <paramref name="index"/> can be removed</returns>
    public bool canRemoveChild(int index, ICoreNode contextNode)
		{
			if(contextNode.getChildCount()<=index)
				throw(new urakawa.exception.MethodParameterIsOutOfBoundsException("no child at index " + index.ToString()));
			ICoreNode toBeRemoved = (ICoreNode)contextNode.getChild(index);
			return canRemoveChild(toBeRemoved);
		}

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be appended to a given context <see cref="ICoreNode"/>
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to append</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indocating if <paramref name="node"/> can be appended to 
    /// the list of children of <paramref name="contextNode"/></returns>
    public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
		{
			return canInsert(contextNode,null,node,CanInsertFragmentBuilder.InsertType.InsertLast);
		}

    /// <summary>
    /// Determines if a given context <see cref="ICoreNode"/> can be detached from it's parent
    /// </summary>
    /// <param name="contextNode">The content <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="contextNode"/> can be detached from it's parent
    /// </returns>
    public bool canDetach(ICoreNode contextNode)
		{
			return canRemoveChild(contextNode);
		}

		#endregion
	}
}