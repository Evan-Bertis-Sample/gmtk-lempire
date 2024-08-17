using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace CurlyEditor.Utility
{
    public static class SearchTreeUtility
    {
        public class DefaultSearchTreeEntry : IComparable<DefaultSearchTreeEntry>
        {
            public string Name;
            public object UserData;
            public int Level;

            public DefaultSearchTreeEntry(string name, object userData, int level)
            {
                Name = name;
                UserData = userData;
                Level = level;
            }

            public int CompareTo(DefaultSearchTreeEntry other)
            {
                return Name.CompareTo(other.Name);
            }
        }

        public static Func<string, string, int, DefaultSearchTreeEntry> CreateDefaultItem = (path, leafName, level) =>
        {
            return new DefaultSearchTreeEntry(leafName, path, level);
        };

        public static List<SearchTreeEntry> CreateSearchTree<T>(List<string> elements, Func<string, string, int, T> createItem)
        {
            Debug.Log("Creating search tree");
            // group names are like: "SFX/Explosions", "Music/Background", "Ambience/Forest", "SFX/Weapons"
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            // add the root
            entries.Add(new SearchTreeGroupEntry(new GUIContent(""), 0));
            // build the tree like a file system
            foreach (string path in elements)
            {
                string[] pathParts = path.Split('/');
                // get each part of the path and add it to the tree
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    string pathPart = pathParts[i];
                    SearchTreeEntry parent = entries.Find(e => e.name == pathPart);
                    if (parent == null)
                    {
                        parent = new SearchTreeGroupEntry(new GUIContent(pathPart), i + 1);
                        entries.Add(parent);
                    }
                }

                // add the leaf
                string leafName = pathParts[pathParts.Length - 1];
                T content = createItem(path, leafName, pathParts.Length);

                SearchTreeEntry leaf = new SearchTreeEntry(new GUIContent(leafName))
                {
                    level = pathParts.Length,
                    userData = content
                };

                entries.Add(leaf);
            }

            return entries;
        }

        public static List<SearchTreeEntry> CreateDefaultSearchTree(List<string> elements)
        {
            return CreateSearchTree<DefaultSearchTreeEntry>(elements, CreateDefaultItem);
        }
    }
}