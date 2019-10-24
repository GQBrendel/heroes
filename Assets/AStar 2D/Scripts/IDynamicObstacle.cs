using System.Collections.Generic;

namespace AStar_2D
{
    public interface IDynamicObstacle
    {
        // Properties
        bool IsObstructing { get; }

        // Methods
        IEnumerable<Index> occupiedIndexes();
    }
}
