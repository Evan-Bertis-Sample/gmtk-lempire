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


        public Bounds GetGridBounds()
        {
            Bounds bounds = new Bounds();
            foreach (Vector2Int position in _entities.Keys)
            {
                bounds.Encapsulate(new Vector3(position.x, 0, position.y));
            }
            return bounds;
        }

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

        public void MoveEntity(GridEntity entity, Vector2Int newPosition)
        {
            Assert.IsNotNull(entity, "Entity cannot be null");
            Assert.IsTrue(_entityPositions.ContainsKey(entity), "Entity is not in the grid");

            // lets check if the new position is available
            if (IsAreaOccupied(newPosition, entity.GridSize, entity.Blockage == BlockageType.Partial))
            {
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

        public bool IsAreaOccupied(Vector2Int position, Vector2Int size, bool allowForPartialBlockage = false)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int currentPosition = position + new Vector2Int(x, y);
                    if (!_entities.ContainsKey(currentPosition))
                    {
                        return false;
                    }
                    if (!allowForPartialBlockage && IsPositionBlocked(currentPosition, false))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsPositionBlocked(Vector2Int position, bool allowForPartialBlockage = false)
        {
            if (!_entities.ContainsKey(position))
            {
                return false;
            }

            foreach (GridEntity entity in _entities[position])
            {
                if (entity.Blockage == BlockageType.Full)
                {
                    return true;
                }
                if (entity.Blockage == BlockageType.Partial && !allowForPartialBlockage)
                {
                    return true;
                }

            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Bounds bounds = GetGridBounds();
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
            foreach (GridEntity entity in GetEntities())
            {
                Gizmos.DrawWireCube(GridToWorldPosition(entity.GridPosition + entity.GridSize / 2), new Vector3(entity.GridSize.x, 0.1f, entity.GridSize.y));
            }
        }

    }
}