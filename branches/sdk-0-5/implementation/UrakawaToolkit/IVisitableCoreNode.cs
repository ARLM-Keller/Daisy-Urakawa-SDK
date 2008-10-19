using System;

namespace urakawa.core
{
	/// <summary>
	/// Delegate for pre-visit
	/// </summary>
	/// <param name="node">The <see cref="ICoreNode"/> being visited</param>
	/// <returns>A <see cref="bool"/> indicating if the children of <paramref name="node"/>
	/// should be visited</returns>
	public delegate bool preVisitDelegate(ICoreNode node);

	/// <summary>
	/// Delegate for post-visit
	/// </summary>
	/// <param name="node">The <see cref="ICoreNode"/> being visited</param>
	public delegate void postVisitDelegate(ICoreNode node);

	/// <summary>
	/// Provides methods for accepting <see cref="ICoreNodeVisitor"/>s
	/// </summary>
	public interface IVisitableCoreNode
	{
    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in depth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptDepthFirst(ICoreNodeVisitor visitor);

    /// <summary>
    /// Accept a <see cref="ICoreNodeVisitor"/> in breadth first mode
    /// </summary>
    /// <param name="visitor">The <see cref="ICoreNodeVisitor"/></param>
    void acceptBreadthFirst(ICoreNodeVisitor visitor);
	
		/// <summary>
		/// Visits the <see cref="IVisitableCoreNode"/> depth first
		/// </summary>
		/// <param name="preVisit">The pre-visit delegate</param>
		/// <param name="postVisit">The post visit delegate</param>
		void visitDepthFirst(preVisitDelegate preVisit, postVisitDelegate postVisit);
	}
}