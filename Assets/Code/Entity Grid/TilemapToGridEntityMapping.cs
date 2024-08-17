using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.EntityGrid
{
    public class TilemapToGridEntityMapping : ScriptableObject
    {
        public struct TilemapToGridEntityMap
        {
            // public Tilemap Tilemap;
            public GridEntityComponent GridEntityPrefab;
        }
    }
}
