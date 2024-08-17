using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.EntityGrid
{
    public class EntityGridComponent : MonoBehaviour
    {
        [field: SerializeField] public float CellsPerUnit { get; private set; } = 1f;
        public EntityGrid Grid = new EntityGrid();

        public Bounds GridBounds => Grid.GetBounds();
        public void AddEntity(GridEntity entity) => Grid.AddEntity(entity);
        public void RemoveEntity(GridEntity entity) => Grid.RemoveEntity(entity);
        public void MoveEntity(GridEntity entity, Vector2Int newPosition) => Grid.MoveEntity(entity, newPosition);
        public bool IsAreaOccupied(Vector2Int position, Vector2Int size, bool allowBlockage = false) => Grid.IsAreaOccupied(position, size, allowBlockage);

        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => new Vector2Int(Mathf.FloorToInt(worldPosition.x * CellsPerUnit), Mathf.FloorToInt(worldPosition.z * CellsPerUnit));
        public Vector3 GridToWorldPosition(Vector2Int gridPosition) => new Vector3(gridPosition.x / CellsPerUnit, 0, gridPosition.y / CellsPerUnit);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Bounds bounds = GridBounds;
            // make sure that the bounds are at least 10x10 (x and y)
            bounds.size = new Vector3(Mathf.Max(10, bounds.size.x), Mathf.Max(10, bounds.size.y), 0);

            Debug.Log("Bounds: " + bounds);

            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            // draw the grid lines
            for (float x = bounds.min.x; x <= bounds.max.x; x += 1f / CellsPerUnit)
            {
                Gizmos.DrawLine(new Vector3(x, bounds.min.y, 0), new Vector3(x, bounds.max.y, 0));
            }

            for (float y = bounds.min.z; y <= bounds.max.y; y += 1f / CellsPerUnit)
            {
                Gizmos.DrawLine(new Vector3(bounds.min.x, y, 0), new Vector3(bounds.max.x, y, 0));
            }

            Gizmos.color = Color.blue;
            // draw the entities
            foreach (GridEntity entity in Grid.GetEntities())
            {
                Gizmos.DrawCube(GridToWorldPosition(entity.Position + entity.Size / 2), new Vector3(entity.Size.x, 0.1f, entity.Size.y));
            }
        }

    }
}