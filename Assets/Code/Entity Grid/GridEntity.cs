using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.EntityGrid
{
    public enum BlockageType
    {
        None,
        Full,
        Partial
    }

    [System.Serializable]
    public class GridEntityProperties 
    {
        public int Team = -1;
        public BlockageType Blockage = BlockageType.None;
    }

    public class GridEntity
    {
        public static List<Vector2Int> GetOccupiedPositions(Vector2Int position, Vector2Int size)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    positions.Add(position + new Vector2Int(x, y));
                }
            }
            return positions;
        }

        public GridEntity(EntityGrid grid, Vector2Int position, Vector2Int size, BlockageType blockage)
        {
            Grid = grid;
            Position = position;
            Size = size;
            grid.AddEntity(this);
            Properties = new GridEntityProperties
            {
                Blockage = blockage
            };
        }

        public EntityGrid Grid { get; private set; }
        public Vector2Int Position { get; protected set; }
        public Vector2Int Size { get; protected set; }
        public GridEntityProperties Properties { get; private set; }
        public BlockageType Blockage => Properties.Blockage;

        public List<Vector2Int> GetOccupiedPositions()
        {
            return GetOccupiedPositions(Position, Size);
        }

        public bool MoveEntity(Vector2Int newPosition)
        {
            if (Grid.IsAreaOccupied(newPosition, Size))
            {
                return false;
            }
            Grid.MoveEntity(this, newPosition);
            Position = newPosition;
            return true;
        }

    }
}
