using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Curly.Grid;

namespace CurlyEditor.Grid
{
    [CustomEditor(typeof(GridEntity))]
    public class GridEntityComponentEditor : Editor
    {
        private GridEntity _gridEntityComponent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _gridEntityComponent = (GridEntity)target;

            CheckForGrid();
            if (_gridEntityComponent.Grid == null)
            {
                return;
            }

            DrawEntityInfo();
        }

        private void OnSceneGUI()
        {
            _gridEntityComponent = (GridEntity)target;
            if (_gridEntityComponent.Grid == null)
            {
                return;
            }

            // snap the entity to the grid
            Vector2Int gridPosition = _gridEntityComponent.Grid.WorldToGridPosition(_gridEntityComponent.transform.position);
            Vector3 snappedPosition = _gridEntityComponent.Grid.GridToWorldPosition(gridPosition);
            _gridEntityComponent.transform.position = snappedPosition;
            _gridEntityComponent.GridPosition = gridPosition;

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

            List<Vector2Int> occupiedPositions = GridEntity.GetOccupiedPositions(gridPosition, _gridEntityComponent.GridSize);
            foreach (Vector2Int position in occupiedPositions)
            {
                Vector3 worldPosition = _gridEntityComponent.Grid.GridToWorldPosition(position);
                Vector3 worldSize = _gridEntityComponent.Grid.GridToWorldSize(Vector2Int.one);
                Handles.DrawWireCube(worldPosition, worldSize);
            }

        }

        private void CheckForGrid()
        {
            if (_gridEntityComponent.Grid == null)
            {
                // check if there is a grid in the parent
                EntityGrid grid = _gridEntityComponent.GetComponentInParent<EntityGrid>();
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
            EditorGUILayout.BeginVertical(CurlyEditorStyles.LightBoxStyle);
            EditorGUILayout.LabelField("Entity Info", CurlyEditorStyles.BoldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grid Position", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.LabelField(_gridEntityComponent.GridPosition.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Blockage", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.LabelField(_gridEntityComponent.Blockage.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}