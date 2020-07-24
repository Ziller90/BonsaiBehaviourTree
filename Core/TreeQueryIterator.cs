﻿
using System;

namespace Bonsai.Core
{

  /// <summary>
  /// A specialized fixed-sized iterator meant to query the behaviour tree without GC allocations.
  /// </summary>
  public class TreeQueryIterator
  {
    private readonly Utility.FixedSizeStack<BehaviourNode> traversal;

    public TreeQueryIterator(int treeHeight)
    {
      // Since tree heights starts from zero, the stack needs to have treeHeight + 1 slots.
      traversal = new Utility.FixedSizeStack<BehaviourNode>(treeHeight + 1);
    }

    public BehaviourNode Next()
    {
      return PreOrderNext();
    }

    private BehaviourNode PreOrderNext()
    {
      BehaviourNode current = traversal.Pop();

      for (int i = current.ChildCount() - 1; i >= 0; --i)
      {
        BehaviourNode child = current.GetChildAt(i);
        traversal.Push(child);
      }

      return current;
    }

    /// <summary>
    /// Checks if there are still nodes to traverse.
    /// </summary>
    /// <returns></returns>
    public bool HasNext()
    {
      return traversal.Count != 0;
    }

    /// <summary>
    /// Gets the maximum maximum for the traversed branch.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="initial"></param>
    /// <note>int.MinValue is used because that is lowest possible pre order priority value.</note>
    /// <returns></returns>
    public float MaxUtility(BehaviourNode root, float initial = int.MinValue)
    {
      traversal.Push(root);

      while (HasNext())
      {
        var node = Next();
        initial = Math.Max(initial, node.UtilityValue());
      }

      traversal.ResetCount();
      return initial;
    }

    /// <summary>
    /// Sums the utility values of the traversed branch.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="initial"></param>
    /// <returns></returns>
    public float SumUtility(BehaviourNode root, float initial = 0f)
    {
      traversal.Push(root);

      while (HasNext())
      {
        var node = Next();
        initial += node.UtilityValue();
      }

      traversal.ResetCount();
      return initial;
    }

  }
}
