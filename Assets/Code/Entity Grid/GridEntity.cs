using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Curly.Grid
{
#region Helper Types
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

#endregion

    public class GridEntity : MonoBehaviour
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

        [field: SerializeField] public EntityGrid Grid { get; private set; }
        [field: SerializeField] public Vector2Int GridSize { get; private set; } = Vector2Int.one;
        [field: SerializeField] public GridEntityProperties Properties { get; private set; }
        public Vector2Int GridPosition;
        public BlockageType Blockage => Properties.Blockage;
        public Vector3 WorldPosition => Grid.GridToWorldPosition(GridPosition);

        public Bounds GridBounds => new Bounds(new Vector3(GridPosition.x, GridPosition.y, 0), new Vector3(GridSize.x, GridSize.y, 1));
        public Bounds WorldBounds => new Bounds(WorldPosition, Grid.GridToWorldSize(GridSize));

        private void Start()
        {
            Assert.IsNotNull(Grid, "Grid cannot be null");
            SetGrid(Grid);
        }

        private void Update()
        {
            transform.position = WorldPosition;
        }

        public void SetGrid(EntityGrid grid)
        {
            Assert.IsNotNull(grid, "Grid cannot be null");
            Grid = grid;
            HashSet<GridEntity> ignore = new HashSet<GridEntity>
            {
                this
            };

            bool occupied = Grid.IsAreaOccupied(GridPosition, GridSize, true, ignore);
            if (occupied)
            {
                Debug.LogError($"Entity {name} is unable to be placed on the grid at position {GridPosition} with size {GridSize}");
                // to prevent future errors, disable the entity
                gameObject.SetActive(false);
            }
            Grid.AddEntity(this);
        }

        public List<Vector2Int> GetOccupiedGridPositions()
        {
            return GetOccupiedPositions(GridPosition, GridSize);
        }

        private void OnDestroy()
        {
            Grid.RemoveEntity(this);
        }
    }
}