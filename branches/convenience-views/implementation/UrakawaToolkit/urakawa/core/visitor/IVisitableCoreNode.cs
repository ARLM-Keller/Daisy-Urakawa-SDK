using System;
using urakawa.core;

namespace urakawa.core.visitor
{
	/// <summary>
	/// Delegate for pre-visit
	/// </summary>
	/// <param localName="node">The <see cref="ICoreNode"/> being visited</param>
	/// <returns>A <see cref="bool"/> indicating if the children of <paramref localName="node"/>
	/// should be visited</returns>
	public delegate bool preVisitDelegate(ICoreNode node);

	/// <summary>
	/// Delegate for post-visit
	/// </summary>
	/// <param localName="node">The <see cref="ICoreNode"/> being visited</param>
	public delegate void postVisitDelegate(ICoreNode node);

	/// <summary>
	/// Provides methods for accepting <see cref="ICoreNodeVisitor"/>s
	/// </summary>
	public interface IVisitableCoreNode
	{
    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param localName="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptDepthFirst(ICoreNodeVisitor visitor);

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param localName="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptBreadthFirst(ICoreNodeVisitor visitor);
	
		/// <summary>
		/// Visits the <see cref="IVisitableCoreNode"/> depth first
		/// </summary>
		/// <param localName="preVisit">The pre-visit delegate</param>
		/// <param localName="postVisit">The post visit delegate</param>
		void visitDepthFirst(preVisitDelegate preVisit, postVisitDelegate postVisit);
	}
}
