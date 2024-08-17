using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Curly.EntityGrid
{
    public class GridEntityComponent : MonoBehaviour
    {
        [field: SerializeField] public EntityGridComponent Grid { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
        [field: SerializeField] public GridEntityProperties Properties { get; private set; }
        public GridEntity GridEntity { get; private set; }

        public Vector2Int Position => Grid.WorldToGridPosition(transform.position);
        public BlockageType Blockage => Properties.Blockage;

        private void Start()
        {
            Assert.IsNotNull(Grid, "Grid cannot be null");
            SetGrid(Grid);
        }

        public void MoveEntity(Vector2Int newPosition)
        {
            if (Grid.IsAreaOccupied(newPosition, Size))
            {
                return;
            }
            Grid.MoveEntity(GridEntity, newPosition);
        }

        public void SetGrid(EntityGridComponent grid)
        {
            Assert.IsNotNull(grid, "Grid cannot be null");

            if (Grid != null)
            {
                Grid.RemoveEntity(GridEntity);
            }

            Grid = grid;
            GridEntity = new GridEntity(grid.Grid, Position, Size, Properties);
            bool able = Grid.AddEntity(GridEntity);

            if (!able)
            {
                Debug.LogError("Entity could not be added to the grid");
            }
        }

        private void OnDestroy()
        {
            Grid.RemoveEntity(GridEntity);
        }
    }
}