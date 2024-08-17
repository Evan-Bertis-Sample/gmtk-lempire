using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curly.GOAP;
using UnityEditor;

namespace CurlyEditor.GOAP
{
    [CustomEditor(typeof(GOAPAgentObject))]
    public class GOAPAgentObjectEditor : Editor
    {
        private class EditorState
        {
            public bool ShowMissingComponents = true;
        }

        private GOAPAgentObject _agentObject;
        private static Dictionary<GOAPAgentObject, EditorState> _editorStates = new Dictionary<GOAPAgentObject, EditorState>();
        private EditorState editorState => _editorStates[_agentObject];

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _agentObject = (GOAPAgentObject)target;

            if (!_editorStates.ContainsKey(_agentObject))
            {
                _editorStates.Add(_agentObject, new EditorState());
            }

            EditorGUILayout.Space();

            if (!DisplayValidBuilder())
            {
                return;
            }

            DisplayMissingComponents();

            // if the game is playing, display the agent status
            if (Application.isPlaying)
            {
                DisplayAgentStatus();
            }
        }

        private bool DisplayValidBuilder()
        {
            bool valid = GOAPAgentBuilderRegistry.IsValidBuilder(_agentObject.AgentPath);
            if (valid)
            {
                return true;
            }

            EditorGUILayout.BeginHorizontal(CurlyEditorStyles.LightBoxStyle);
            EditorGUILayout.LabelField("Agent Builder", CurlyEditorStyles.BoldLabel);
            EditorGUILayout.HelpBox("No valid agent builder found, please select one!", MessageType.Error);
            EditorGUILayout.EndHorizontal();

            return false;
        }

        private void DisplayMissingComponents()
        {
            bool meetsRequirements = _agentObject.MeetsRequiredComponents(out List<System.Type> missingComponents);
            if (meetsRequirements)
            {
                return;
            }

            editorState.ShowMissingComponents = CurlyEditorStyles.BetterDropDownHeader(editorState.ShowMissingComponents, $"Missing Components ({missingComponents.Count})", true);

            if (!editorState.ShowMissingComponents)
            {
                return;
            }

            EditorGUILayout.BeginVertical(CurlyEditorStyles.DefaultBox);
            foreach (System.Type component in missingComponents)
            {
                EditorGUILayout.HelpBox($"Missing component {component}", MessageType.Error);

                if (GUILayout.Button($"Add {component.Name}"))
                {
                    _agentObject.gameObject.AddComponent(component);
                }

            }
            EditorGUILayout.EndVertical();

        }

        private void DisplayAgentStatus()
        {

        }
    }
}
