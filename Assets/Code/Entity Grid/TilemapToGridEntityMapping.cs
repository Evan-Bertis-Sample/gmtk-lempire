using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.Grid
{
    public class TilemapToGridEntityMapping : ScriptableObject
    {
        public struct TilemapToGridEntityMap
        {
            // public Tilemap Tilemap;
            public GridEntity GridEntityPrefab;
        }
    }
}
