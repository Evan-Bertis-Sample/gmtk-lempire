using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Curly.EntityGrid
{
    public class GridEntityComponent : MonoBehaviour
    {
        [field: SerializeField] public EntityGridComponent GridComponent { get; private set; }
        [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
        [field: SerializeField] public GridEntityProperties Properties { get; private set; }
        public GridEntity GridEntity { get; private set; }
        public Vector2Int GridPosition => GridComponent ? GridComponent.WorldToGridPosition(transform.position) : Vector2Int.zero;
        public BlockageType Blockage => Properties.Blockage;

        private void Start()
        {
            Assert.IsNotNull(GridComponent, "Grid cannot be null");
            SetGrid(GridComponent);
        }


        public void SetGrid(EntityGridComponent grid)
        {
            Assert.IsNotNull(grid, "Grid cannot be null");

            if (GridComponent != null && GridEntity != null)
            {
                GridComponent.RemoveEntity(GridEntity);
            }
            if (GridEntity == null) GridEntity = new GridEntity(GridPosition, Size, Properties);

            GridComponent = grid;
            GridEntity.SetGrid(GridComponent.Grid, false);
            bool able = GridComponent.AddEntity(GridEntity);

            if (!able)
            {
                Debug.LogError("Entity could not be added to the grid");
            }
        }

        private void OnDestroy()
        {
            GridComponent.RemoveEntity(GridEntity);
        }
    }
}