using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.EntityGrid
{
    public class EntityGrid
    {
        public enum BlockageType
        {
            None,
            Full,
            Partial
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


            public EntityGrid Grid { get; private set; }
            public Vector2Int Position { get; protected set; }
            public Vector2Int Size { get; protected set; }
            public BlockageType Blockage { get; private set; }

            public List<Vector2Int> GetOccupiedPositions()
            {
                return GetOccupiedPositions(Position, Size);
            }

            public bool MoveEntity(Vector2Int newPosition)
            {
                if (Grid.IsAreaOccupied(newPosition, Size))
                {
                    return false;
                }
                Grid.MoveEntity(this, newPosition);
                Position = newPosition;
                return true;
            }

        }

        private Dictionary<Vector2Int, GridEntity> _entities = new Dictionary<Vector2Int, GridEntity>();
        private Dictionary<GridEntity, List<Vector2Int>> _entityPositions = new Dictionary<GridEntity, List<Vector2Int>>();

        public void AddEntity(GridEntity entity)
        {
            List<Vector2Int> positions = entity.GetOccupiedPositions();
            foreach (Vector2Int position in positions)
            {
                _entities.Add(position, entity);
            }
            _entityPositions.Add(entity, positions);
        }

        public void RemoveEntity(GridEntity entity)
        {
            List<Vector2Int> positions = _entityPositions[entity];
            foreach (Vector2Int position in positions)
            {
                _entities.Remove(position);
            }
            _entityPositions.Remove(entity);
        }

        public void MoveEntity(GridEntity entity, Vector2Int newPosition) => entity.MoveEntity(newPosition);

        public bool IsAreaOccupied(Vector2Int position, Vector2Int size, bool allowBlockage = false)
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
                    if (!allowBlockage && _entities[currentPosition].Blockage != BlockageType.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}