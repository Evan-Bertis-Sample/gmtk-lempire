using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Curly.EntityGrid
{
    public class EntityGridComponent : MonoBehaviour
    {
        public static EntityGridComponent GetComponentFromGrid(GridEntity find)
        {
            // find all the entity grids in the scene
            EntityGridComponent[] entityGrids = FindObjectsOfType<EntityGridComponent>();
            foreach (EntityGridComponent entityGrid in entityGrids)
            {
                if (entityGrid.Grid == find.Grid)
                {
                    return entityGrid;
                }
            }

            return null;
        }


        [field: SerializeField] public float CellsPerUnit { get; private set; } = 1f;
        public EntityGrid Grid = new EntityGrid();

        public Bounds GridBounds => Grid.GetBounds();
        public bool AddEntity(GridEntity entity) => Grid.AddEntity(entity);
        public void RemoveEntity(GridEntity entity) => Grid.RemoveEntity(entity);
        public bool IsAreaOccupied(Vector2Int position, Vector2Int size, bool allowBlockage = false) => Grid.IsAreaOccupied(position, size, allowBlockage);

        private Dictionary<GridEntity, GridEntityComponent> _entityComponents = new Dictionary<GridEntity, GridEntityComponent>();

        public void AddEntity(GridEntityComponent entityComponent)
        {
            if (_entityComponents.ContainsKey(entityComponent.GridEntity))
            {
                return;
            }

            _entityComponents.Add(entityComponent.GridEntity, entityComponent);
            AddEntity(entityComponent.GridEntity);
        }

        public void RemoveEntity(GridEntityComponent entityComponent)
        {
            if (!_entityComponents.ContainsKey(entityComponent.GridEntity))
            {
                return;
            }

            _entityComponents.Remove(entityComponent.GridEntity);
            RemoveEntity(entityComponent.GridEntity);
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => new Vector2Int(Mathf.FloorToInt(worldPosition.x * CellsPerUnit), Mathf.FloorToInt(worldPosition.y * CellsPerUnit));
        public Vector3 GridToWorldPosition(Vector2Int gridPosition) => new Vector3(gridPosition.x / CellsPerUnit, gridPosition.y / CellsPerUnit, 0);
        public Vector2Int WorldToGridSize(Vector3 worldSize) => new Vector2Int(Mathf.CeilToInt(worldSize.x * CellsPerUnit), Mathf.CeilToInt(worldSize.y * CellsPerUnit));
        public Vector3 GridToWorldSize(Vector2Int gridSize) => new Vector3(gridSize.x / CellsPerUnit, gridSize.y / CellsPerUnit, 1);

        public void MoveEntity(GridEntity entity, Vector2Int newPosition)
        {
            Grid.MoveEntity(entity, newPosition);

            if (_entityComponents.ContainsKey(entity))
            {
                GridEntityComponent entityComponent = _entityComponents[entity];
                entityComponent.transform.position = GridToWorldPosition(newPosition);
            }
        }

        private void Awake()
        {
            Grid = new EntityGrid(); // just reset the grid
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Bounds bounds = GridBounds;
            // make sure that the bounds are at least 10x10 (x and y)
            bounds.size = new Vector3(Mathf.Max(10, bounds.size.x), Mathf.Max(10, bounds.size.y), 0);
            Debug.Log("Bounds: " + bounds);

            // draw the grid
            Gizmos.color = Color.red;

            Gizmos.color = Color.green;
            for (int x = (int)bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = (int)bounds.min.y; y < bounds.max.y; y++)
                {
                    Gizmos.DrawWireCube(GridToWorldPosition(new Vector2Int(x, y)), new Vector3(1 / CellsPerUnit, 1 / CellsPerUnit, 0));
                }
            }


            Gizmos.color = Color.blue;
            // draw the entities
            foreach (GridEntity entity in Grid.GetEntities())
            {
                Gizmos.DrawWireCube(GridToWorldPosition(entity.Position + entity.Size / 2), new Vector3(entity.Size.x, 0.1f, entity.Size.y));
            }
        }

    }
}