using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curly.GOAP;
using CurlyEditor.Utility;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System;

namespace CurlyEditor.GOAP
{
    [CustomPropertyDrawer(typeof(GOAPAgentBuilderPathAttribute))]
    public class GOAPAgentBuilderPropertyDrawer : SearchBarDrawer
    {
        public class GOAPAgentBuilderSearchProvider : ScriptableObject, ISearchWindowProvider
        {
            private Action<SearchTreeUtility.DefaultSearchTreeEntry> _onSelect;

            public GOAPAgentBuilderSearchProvider(Action<SearchTreeUtility.DefaultSearchTreeEntry> onSelect)
            {
                _onSelect = onSelect;
            }


            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                return SearchTreeUtility.CreateDefaultSearchTree(GOAPAgentBuilderRegistry.GetAgentBuilderNames());
            }

            public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
            {
                SearchTreeUtility.DefaultSearchTreeEntry entry = SearchTreeEntry.userData as SearchTreeUtility.DefaultSearchTreeEntry;
                _onSelect?.Invoke(entry);
                return true;
            }
        }

        protected override void ButtonClicked(Rect buttonPosition)
        {
            GOAPAgentBuilderSearchProvider provider = new GOAPAgentBuilderSearchProvider(OnSelectEntry);
            SearchWindowContext searchContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            SearchWindow.Open(searchContext, provider);
        }

        private void OnSelectEntry(SearchTreeUtility.DefaultSearchTreeEntry entry)
        {
            Debug.Log($"Selected {entry.Name}");
            _propertyValue = entry.Name;
        }
    }
}