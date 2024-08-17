using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.EntityGrid
{
    public class GridEntityComponent : MonoBehaviour
    {
        [field: SerializeField] public EntityGridComponent Grid { get; private set; }
        [field: SerializeField] public Vector2Int Position { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
        [field: SerializeField] public BlockageType Blockage { get; private set; } = BlockageType.None;

        public GridEntity GridEntity { get; private set; }

        public void Initialize(EntityGridComponent grid, Vector2Int position, Vector2Int size, BlockageType blockage)
        {
            Grid = grid;
            Position = position;
            Size = size;
            Blockage = blockage;
            GridEntity = new GridEntity(grid.Grid, position, size, blockage);
        }

        public void MoveEntity(Vector2Int newPosition)
        {
            if (Grid.IsAreaOccupied(newPosition, Size))
            {
                return;
            }
            Grid.MoveEntity(GridEntity, newPosition);
            Position = newPosition;
        }

        private void OnDestroy()
        {
            Grid.RemoveEntity(GridEntity);
        }
    }
}