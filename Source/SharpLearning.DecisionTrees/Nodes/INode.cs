﻿using System.Collections.Generic;

namespace SharpLearning.DecisionTrees.Nodes
{
    /// <summary>
    /// Node interface for binary decision tree nodes
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Feature index used for split
        /// </summary>
        int FeatureIndex { get; }

        /// <summary>
        /// Feature value used for split
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Right child tree index
        /// </summary>
        int RightIndex { get; }

        /// <summary>
        /// Left child tree index
        /// </summary>
        int LeftIndex { get; }

        /// <summary>
        /// Node tree index
        /// </summary>
        int NodeIndex { get; }

        /// <summary>
        /// Probabilities if availible
        /// </summary>
        Dictionary<double, double> Probabilities { get; }
    }
}
