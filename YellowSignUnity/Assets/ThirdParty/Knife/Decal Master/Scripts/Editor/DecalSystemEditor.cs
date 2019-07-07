using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Knife.DeferredDecals
{
    [CustomEditor(typeof(DeferredDecalsSystem))]
    public class DecalSystemEditor : Editor
    {
        DeferredDecalsSystem system;

        public override void OnInspectorGUI()
        {
            GUILayout.Label(DecalEditor.Header);
            base.OnInspectorGUI();

            if (system.TerrainDecals != DeferredDecalsSystem.TerrainDecalsType.None)
            {
                if (GUILayout.Button("Copy terrain heightmaps"))
                {
                    system.CopyHeightmaps();
                }
            }
        }

        private void OnEnable()
        {
            system = target as DeferredDecalsSystem;
        }

        [MenuItem("GameObject/3D Object/Decal")]
        public static void CreateDecal()
        {
            var scene = SceneView.sceneViews[0] as SceneView;

            GameObject decalInstance = new GameObject("Decal", typeof(Decal));

            decalInstance.transform.position = scene.pivot;
            Selection.activeObject = decalInstance;
            Undo.RegisterCreatedObjectUndo(decalInstance, "Create Decal");
        }
    }
}