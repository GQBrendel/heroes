using UnityEngine;
using System.Collections;

using AStar_2D.Pathfinding;

namespace AStar_2D.Demo
{
    /// <summary>
    /// An example script that demonstrates how custom node connection checking can be implemented.
    /// </summary>
    public class SelectiveSearchGrid : SearchGrid
    {
        /// <summary>
        /// Called for every node in the search space to determine whether the connection to its neighbor is allowed.
        /// The center node and neighbor node are guarenteed to be next to each other in some way.
        /// </summary>
        /// <param name="center">The center node</param>
        /// <param name="neighbor">The neighbor to check agains</param>
        /// <returns>Return true if the connection is allowed or false if it should not be allowed</returns>
        public override bool validateConnection(PathNode center, PathNode neighbor)
        {
            // A simple demo to check whether the current node is above the neighbor node.
            // Prevent the agent from finding downward paths.
            if (center.Index.Y > neighbor.Index.Y)
                return false;

            // The connection is allowed
            return true;
        }
    }

    /// <summary>
    /// Make an AStar grid component using our new search grid implementation.
    /// This allows us to use our SelectiveSearchGrid class in an AStar component grid that can be assigned in the UNity editor.
    /// </summary>
    public class SelectiveAStarGrid : AStarGrid<SelectiveSearchGrid>
    {
        // Empty class
    }
}
