using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Curly.EntityGrid
{
    public class EntityGrid
    {
        private Dictionary<Vector2Int, List<GridEntity>> _entities = new Dictionary<Vector2Int, List<GridEntity>>();
        private Dictionary<GridEntity, List<Vector2Int>> _entityPositions = new Dictionary<GridEntity, List<Vector2Int>>();
        HashSet<GridEntity> _entitiesSet = new HashSet<GridEntity>();

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

        public Bounds GetBounds()
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

            List<Vector2Int> positions = entity.GetOccupiedPositions();
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
            bool isAreaOccupied = IsAreaOccupied(entity.Position, entity.Size, canAllowPartialBlockage);
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
            if (IsAreaOccupied(newPosition, entity.Size, entity.Blockage == BlockageType.Partial))
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
            entity.Position = newPosition;
            List<Vector2Int> newPositions = entity.GetOccupiedPositions();
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

        public List<Vector2Int> CalculatePath(Vector2Int start, Vector2Int end)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(start);
            Vector2Int currentPosition = start;
            while (currentPosition != end)
            {
                Vector2Int nextPosition = GetNextPosition(currentPosition, end);
                path.Add(nextPosition);
                currentPosition = nextPosition;
            }
            return path;
        }

        private Vector2Int GetNextPosition(Vector2Int current, Vector2Int target)
        {
            Vector2Int direction = target - current;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return new Vector2Int(current.x + (int)Mathf.Sign(direction.x), current.y);
            }
            else
            {
                return new Vector2Int(current.x, current.y + (int)Mathf.Sign(direction.y));
            }
        }


    }
}