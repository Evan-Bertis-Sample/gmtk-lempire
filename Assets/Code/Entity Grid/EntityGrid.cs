using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace Curly.Grid
{
    public class EntityGrid : MonoBehaviour
    {
        [field: SerializeField] public float CellsPerUnit { get; private set; } = 1f;

        private Dictionary<Vector2Int, List<GridEntity>> _entities = new Dictionary<Vector2Int, List<GridEntity>>();
        private Dictionary<GridEntity, List<Vector2Int>> _entityPositions = new Dictionary<GridEntity, List<Vector2Int>>();
        HashSet<GridEntity> _entitiesSet = new HashSet<GridEntity>();

        public Vector2Int WorldToGridPosition(Vector3 worldPosition) => new Vector2Int(Mathf.FloorToInt(worldPosition.x * CellsPerUnit), Mathf.FloorToInt(worldPosition.y * CellsPerUnit));
        public Vector3 GridToWorldPosition(Vector2Int gridPosition) => new Vector3(gridPosition.x / CellsPerUnit, gridPosition.y / CellsPerUnit, 0);
        public Vector2Int WorldToGridSize(Vector3 worldSize) => new Vector2Int(Mathf.CeilToInt(worldSize.x * CellsPerUnit), Mathf.CeilToInt(worldSize.y * CellsPerUnit));
        public Vector3 GridToWorldSize(Vector2Int gridSize) => new Vector3(gridSize.x / CellsPerUnit, gridSize.y / CellsPerUnit, 1);

        public static Bounds GetGridBounds(IEnumerable<GridEntity> entities)
        {
            Bounds bounds = new Bounds();
            foreach (GridEntity entity in entities)
            {
                bounds.Encapsulate(entity.WorldPosition);
            }
            return bounds;
        }

        public static Bounds GetWorldBounds(IEnumerable<GridEntity> entities)
        {
            Bounds bounds = new Bounds();
            foreach (GridEntity entity in entities)
            {
                bounds.Encapsulate(entity.WorldBounds);
            }
            return bounds;
        }

        public Bounds GetGridBounds() => GetGridBounds(_entitiesSet);
        public Bounds GetWorldBounds() => GetWorldBounds(_entitiesSet);

        public List<GridEntity> GetEntities()
        {
            return new List<GridEntity>(_entitiesSet);
        }

        public bool AddEntity(GridEntity entity)
        {
            Assert.IsNotNull(entity, "Entity cannot be null");
            bool able = AbleToAddEntity(entity);
            if (!able)
            {
                return false;
            }

            List<Vector2Int> positions = entity.GetOccupiedGridPositions();
            foreach (Vector2Int position in positions)
            {
                if (!_entities.ContainsKey(position))
                {
                    _entities.Add(position, new List<GridEntity>());
                }
                _entities[position].Add(entity);
            }
            _entityPositions.Add(entity, positions);
            _entitiesSet.Add(entity);

            return true;
        }

        public bool AbleToAddEntity(GridEntity entity)
        {
            bool canAllowPartialBlockage = entity.Blockage == BlockageType.Partial;
            bool isAreaOccupied = IsAreaOccupied(entity.GridPosition, entity.GridSize, canAllowPartialBlockage);
            bool ableToAddEntity = entity.Blockage == BlockageType.None ? !isAreaOccupied : true;
            return ableToAddEntity;
        }

        public void RemoveEntity(GridEntity entity)
        {
            List<Vector2Int> positions = _entityPositions[entity];
            foreach (Vector2Int position in positions)
            {
                _entities.Remove(position);
            }
            _entityPositions.Remove(entity);
            _entitiesSet.Remove(entity);
        }

        public void MoveEntity(GridEntity entity, Vector2Int newPosition, HashSet<GridEntity> ignoreEntities = null)
        {
            Assert.IsNotNull(entity, "Entity cannot be null");
            Assert.IsTrue(_entityPositions.ContainsKey(entity), "Entity is not in the grid");

            if (ignoreEntities == null)
            {
                ignoreEntities = new HashSet<GridEntity>();
            }

            ignoreEntities.Add(entity);

            // lets check if the new position is available
            if (IsAreaOccupied(newPosition, entity.GridSize, entity.Blockage == BlockageType.Partial, ignoreEntities))
            {
                Debug.LogWarning("Cannot move entity to position " + newPosition + " because it is occupied");
                return;
            }

            // remove the entity from the old position
            foreach (Vector2Int position in _entityPositions[entity])
            {
                _entities[position].Remove(entity);
                if (_entities[position].Count == 0)
                {
                    _entities.Remove(position);
                }
            }

            // add the entity to the new position
            entity.GridPosition = newPosition;
            List<Vector2Int> newPositions = entity.GetOccupiedGridPositions();
            foreach (Vector2Int position in newPositions)
            {
                if (!_entities.ContainsKey(position))
                {
                    _entities.Add(position, new List<GridEntity>());
                }
                _entities[position].Add(entity);
            }
            _entityPositions[entity] = newPositions;
        }

        public bool IsAreaOccupied(Vector2Int position, Vector2Int size, bool allowForPartialBlockage = false, HashSet<GridEntity> ignoreEntities = null)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int currentPosition = position + new Vector2Int(x, y);
                    if (IsPositionBlocked(currentPosition, allowForPartialBlockage, ignoreEntities))
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public bool IsPositionBlocked(Vector2Int position, bool allowForPartialBlockage = false, HashSet<GridEntity> ignoreEntities = null)
        {
            if (!_entities.ContainsKey(position))
            {
                Debug.Log("Position is not blocked");
                return false;
            }

            foreach (GridEntity entity in _entities[position])
            {
                if (ignoreEntities != null && ignoreEntities.Contains(entity))
                {
                    continue;
                }

                if (entity.Blockage == BlockageType.Full)
                {
                    Debug.Log("Position is blocked by " + entity.name);
                    return true;
                }
                if (entity.Blockage == BlockageType.Partial && !allowForPartialBlockage)
                {
                    Debug.Log("Position is partially blocked by " + entity.name);
                    return true;
                }

            }

            return false;
        }

        private void OnDrawGizmos()
        {
            HashSet<GridEntity> entities = new HashSet<GridEntity>(_entitiesSet);

            #if UNITY_EDITOR
            // grab all of the entities in the scene where their grid is this grid
            GridEntity[] allEntities = FindObjectsOfType<GridEntity>();
            foreach (GridEntity entity in allEntities)
            {
                if (entity.Grid == this)
                {
                    entities.Add(entity);
                }
            }
            #endif

            // find the bounds of the grid, based on the entities
            Bounds bounds = EntityGrid.GetWorldBounds(entities);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            // draw the grid lines, in a light blue
            Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.5f);
            for (int x = 0; x <= bounds.size.x; x++)
            {
                Vector3 start = bounds.min + new Vector3(x, 0, 0);
                Vector3 end = bounds.min + new Vector3(x, bounds.size.y, 0);
                Gizmos.DrawLine(start, end);
            }

            for (int y = 0; y <= bounds.size.y; y++)
            {
                Vector3 start = bounds.min + new Vector3(0, y, 0);
                Vector3 end = bounds.min + new Vector3(bounds.size.x, y, 0);
                Gizmos.DrawLine(start, end);
            }

            // draw all of the entities on the grid
            foreach (GridEntity entity in entities)
            {
                switch (entity.Blockage)
                {
                    case BlockageType.None:
                        Gizmos.color = Color.blue;
                        break;
                    case BlockageType.Partial:
                        Gizmos.color = Color.yellow;
                        break;
                    case BlockageType.Full:
                        Gizmos.color = Color.red;
                        break;
                }
                Gizmos.DrawWireCube(entity.WorldPosition, GridToWorldSize(entity.GridSize));
            }
        }

    }
}