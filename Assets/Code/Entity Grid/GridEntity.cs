using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        public GridEntityMovementAnimation MovementAnimation;
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

        public GridEntity(Vector2Int position, Vector2Int size, BlockageType blockage)
        {
            Position = position;
            Size = size;
            Properties = new GridEntityProperties
            {
                Blockage = blockage
            };
        }

        public GridEntity(Vector2Int position, Vector2Int size, GridEntityProperties properties)
        {
            Position = position;
            Size = size;
            Properties = properties;
        }

        public void SetGrid(EntityGrid grid, bool addEntity = true)
        {
            Grid = grid;
            if (addEntity) grid.AddEntity(this);
        }
        

        public EntityGrid Grid { get; private set; }
        public Vector2Int Position;
        public Vector2Int Size;
        public GridEntityProperties Properties;
        public BlockageType Blockage => Properties.Blockage;

        public List<Vector2Int> GetOccupiedPositions()
        {
            return GetOccupiedPositions(Position, Size);
        }

        public bool MoveEntity(Vector2Int newPosition)
        {
            Assert.IsNotNull(Grid, "Grid cannot be null");
            if (Grid.IsAreaOccupied(newPosition, Size))
            {
                return false;
            }

            Grid?.MoveEntity(this, newPosition);
            Position = newPosition;
            return true;
        }

    }
}
