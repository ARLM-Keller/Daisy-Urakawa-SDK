package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsOutOfBoundsException;
import org.daisy.urakawa.exception.ObjectIsInDifferentPresentationException;

/**
 * <p>
 * Write-only ITreeNode methods.
 * </p>
 * 
 * @see org.daisy.urakawa.core.ITreeNode
 * @designConvenienceInterface see
 *                             {@link org.daisy.urakawa.DesignConvenienceInterface}
 * @see org.daisy.urakawa.DesignConvenienceInterface
 * @stereotype OptionalDesignConvenienceInterface
 */
public interface ITreeNodeWriteOnlyMethods
{
    /**
     * <p>
     * Sets the parent for this node.
     * </p>
     * <p>
     * This methods should used with caution, as the parent for a node should
     * really only be updated when its parent has added or removed this node to
     * / from its list of children. The "real" ownership relationship is
     * maintained by the list of children at the level of the parent node,
     * whereas this parent pointer is only a convenience to be able to navigate
     * the tree upwards.
     * </p>
     * 
     * @param node
     *        the new parent for this node. can be NULL (this node is a tree
     *        root).
     */
    public void setParent(ITreeNode node);

    /**
     * <p>
     * Inserts the given ITreeNode as a child of this node, at the given index.
     * </p>
     * <p>
     * Does NOT replace the existing child at the given index, but increments
     * once the indexes of all children which index is greater than or equal to
     * insertIndex. If insertIndex is equal to children.size (no following
     * siblings), then the given node is appended at the end of the existing
     * children list.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @param insertIndex
     *        must be in bounds [0..children.size].
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     *         if insertIndex is not in bounds [0..children.size].
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     * @tagvalue Events "ChildAdded"
     */
    public void insert(ITreeNode node, int insertIndex)
            throws MethodParameterIsNullException,
            MethodParameterIsOutOfBoundsException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * <p>
     * Inserts a new child ITreeNode before (sibling) a given reference child
     * ITreeNode.
     * <p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @param anchorNode
     *        cannot be null. see other conditions given by the exceptions.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TreeNodeDoesNotExistException
     *         if anchorNode is not actually if this node's tree.
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public void insertBefore(ITreeNode node, ITreeNode anchorNode)
            throws MethodParameterIsNullException,
            TreeNodeDoesNotExistException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * <p>
     * Inserts a new child ITreeNode after (sibling) a given reference child
     * ITreeNode.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @param anchorNode
     *        cannot be null. see other conditions given by the exceptions.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TreeNodeDoesNotExistException
     *         if anchorNode is not actually if this node's tree.
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public void insertAfter(ITreeNode node, ITreeNode anchorNode)
            throws TreeNodeDoesNotExistException,
            MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * <p>
     * Appends a new child ITreeNode to the end of the list of children.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public void appendChild(ITreeNode node)
            throws MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * <p>
     * Replaces a given child ITreeNode with a new given ITreeNode. the old
     * node's parent is then set to NULL.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @param oldNode
     *        cannot be null. see other conditions given by the exceptions.
     * @return the replaced node
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws TreeNodeDoesNotExistException
     *         if oldNode is not actually if this node's tree.
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public ITreeNode replaceChild(ITreeNode node, ITreeNode oldNode)
            throws TreeNodeDoesNotExistException,
            MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * Replaces the child ITreeNode at a given index with a new given ITreeNode.
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @param index
     *        must be in bounds: [0..children.size-1]
     * @return the Node that was replaced, which parent is NULL.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     * @throws MethodParameterIsOutOfBoundsException
     *         if the given index is not in bounds: [0..children.size-1]
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeHasParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public ITreeNode replaceChild(ITreeNode node, int index)
            throws MethodParameterIsOutOfBoundsException,
            MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeHasParentException, TreeNodeIsAncestorException,
            TreeNodeIsSelfException;

    /**
     * <p>
     * Detaches all children nodes of the given node, and re-attach them to this
     * node, by appending them at the end of the list of already existing child
     * nodes.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     *         "MethodParameterIsNull-NodeIsInDifferentPresentation-NodeIsAncestor-NodeIsSelf"
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     */
    public void appendChildrenOf(ITreeNode node)
            throws MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeIsAncestorException, TreeNodeIsSelfException;

    /**
     * <p>
     * Replace this node with the given node, and vice-versa.
     * </p>
     * 
     * @param node
     *        cannot be null. see other conditions given by the exceptions.
     * @throws ObjectIsInDifferentPresentationException
     *         if the given node Presentation is not the same as this
     *         Presentation.
     * @throws TreeNodeIsAncestorException
     *         if the given node is the root of this node's tree.
     * @throws TreeNodeIsSelfException
     *         if the given node is this node.
     * @throws TreeNodeHasNoParentException
     *         if the given node as a parent already (already part of a tree).
     * @throws TreeNodeIsDescendantException
     *         if the given node is a descendant of this node.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public void swapWith(ITreeNode node) throws MethodParameterIsNullException,
            ObjectIsInDifferentPresentationException,
            TreeNodeIsAncestorException, TreeNodeIsSelfException,
            TreeNodeIsDescendantException, TreeNodeHasNoParentException;

    /**
     * <p>
     * Detaches this ITreeNode instance from the tree. After such operation,
     * getParent() must return NULL. This is equivalent to setParent(NULL).
     * </p>
     * 
     * @return a reference to this node. cannot be null.
     */
    public ITreeNode detach();

    /**
     * <p>
     * Removes the child ITreeNode at a given index (as a whole sub-tree, it's a
     * "deep" tree operation).
     * </p>
     * 
     * @param index
     *        must be in bounds [0..children.size-1].
     * @return the removed node, which parent is then NULL.
     * @throws MethodParameterIsOutOfBoundsException
     *         if the given index is not in bounds [0..children.size-1].
     * @tagvalue Events "ChildRemoved"
     */
    public ITreeNode removeChild(int index)
            throws MethodParameterIsOutOfBoundsException;

    /**
     * <p>
     * Removes a given child ITreeNode (as a whole sub-tree, it's a "deep" tree
     * operation), of which parent is then NULL.
     * </p>
     * 
     * @param node
     *        node must exist as a child, cannot be null
     * @return the removed ITreeNode
     * @throws TreeNodeDoesNotExistException
     *         if the given node is not a child of this node.
     * @throws MethodParameterIsNullException
     *         NULL method parameters are forbidden
     */
    public ITreeNode removeChild(ITreeNode node)
            throws TreeNodeDoesNotExistException,
            MethodParameterIsNullException;

    /**
     * <p>
     * Swap this node with its previous sibling.
     * </p>
     * 
     * @return true if the node was successfully swapped with its previous
     *         sibling. Otherwise return false (e.g. happens when this node is
     *         the first child of its parent node, or the tree root).
     */
    public boolean swapWithPreviousSibling();

    /**
     * <p>
     * Swap this node with its next sibling.
     * </p>
     * 
     * @return true if the node was successfully swapped with its next sibling.
     *         Otherwise return false (e.g. happens when this node is the last
     *         child of its parent node, or the tree root).
     */
    public boolean swapWithNextSibling();

    /**
     * <p>
     * Separates the children of this node at a given index, by leaving the
     * lower indexed children for this node, and creating a new node with the
     * upper indexed children nodes. So, this method basically returns a shallow
     * copy of [this] node, optionally with an entire copy of its properties
     * (see the "copyProperties" method parameter), which has all children of
     * [this] node starting at [index] up to [getChildCount()-1]. [This] node
     * looses these children, but retains the previous sibling ones (from [0] to
     * [index-1]).
     * </p>
     * 
     * @param index
     *        must be in bounds [0..getChildCount()-1]
     * @param copyProperties
     * @return a shallow copy of [this] node, optionally with an entire copy of
     *         its properties (see the "copyProperties" method parameter)
     * @throws MethodParameterIsOutOfBoundsException
     *         if the given index is not in bounds [0..getChildCount()-1]
     */
    public ITreeNode splitChildren(int index, boolean copyProperties)
            throws MethodParameterIsOutOfBoundsException;
}
