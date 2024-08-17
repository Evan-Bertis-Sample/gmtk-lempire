using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Curly.EntityGrid;

namespace CurlyEditor.EntityGrid
{
    [CustomEditor(typeof(GridEntityComponent))]
    public class GridEntityComponentEditor : Editor
    {
        private GridEntityComponent _gridEntityComponent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _gridEntityComponent = (GridEntityComponent)target;

            CheckForGrid();
            if (_gridEntityComponent.GridComponent == null)
            {
                return;
            }

            DrawEntityInfo();
        }

        private void OnSceneGUI()
        {
            _gridEntityComponent = (GridEntityComponent)target;
            if (_gridEntityComponent.GridComponent == null)
            {
                return;
            }

            // snap the entity to the grid
            Vector2Int gridPosition = _gridEntityComponent.GridComponent.WorldToGridPosition(_gridEntityComponent.transform.position);
            Vector3 snappedPosition = _gridEntityComponent.GridComponent.GridToWorldPosition(gridPosition);
            _gridEntityComponent.transform.position = snappedPosition;

            // draw the entity bounds
            switch (_gridEntityComponent.Blockage)
            {
                case BlockageType.None:
                    Handles.color = Color.blue;
                    break;
                case BlockageType.Partial:
                    Handles.color = Color.yellow;
                    break;
                case BlockageType.Full:
                    Handles.color = Color.red;
                    break;
            }

            List<Vector2Int> occupiedPositions = GridEntity.GetOccupiedPositions(gridPosition, _gridEntityComponent.Size);
            foreach (Vector2Int position in occupiedPositions)
            {
                Vector3 worldPosition = _gridEntityComponent.GridComponent.GridToWorldPosition(position);
                Vector3 worldSize = _gridEntityComponent.GridComponent.GridToWorldSize(Vector2Int.one);
                Handles.DrawWireCube(worldPosition, worldSize);
            }

        }

        private void CheckForGrid()
        {
            if (_gridEntityComponent.GridComponent == null)
            {
                // check if there is a grid in the parent
                EntityGridComponent grid = _gridEntityComponent.GetComponentInParent<EntityGridComponent>();
                if (grid != null)
                {
                    _gridEntityComponent.SetGrid(grid);
                }
                else
                {
                    // put a message saying that there is no grid in the parent
                    EditorGUILayout.BeginHorizontal(CurlyEditorStyles.LightBoxStyle);
                    EditorGUILayout.LabelField("Grid", CurlyEditorStyles.BoldLabel);
                    EditorGUILayout.HelpBox("No grid found in the parent, please add one!", MessageType.Error);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawEntityInfo()
        {
            EditorGUILayout.BeginHorizontal(CurlyEditorStyles.LightBoxStyle);
            EditorGUILayout.LabelField("Entity Info", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(CurlyEditorStyles.LightBoxStyle);
            EditorGUILayout.LabelField("Grid Position", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.LabelField(_gridEntityComponent.GridPosition.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(CurlyEditorStyles.LightBoxStyle);
            EditorGUILayout.LabelField("Blockage", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.LabelField(_gridEntityComponent.Blockage.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }
}