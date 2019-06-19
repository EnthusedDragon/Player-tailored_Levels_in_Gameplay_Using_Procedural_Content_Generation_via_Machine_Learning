using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Assets.Scripts
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(MazeLoader))]
    public class RandomScript_Editor : Editor
    {
        //public override void OnInspectorGUI()
        //{
        //    DrawDefaultInspector(); // for other non-HideInInspector fields

        //    MazeLoader script = (MazeLoader)target;
            
        //    if (script.randomSeed) // if bool is true, show other fields
        //    {
        //        script.seed = EditorGUILayout.TextField("Seed", script.seed);
        //    }
        //}
    }
#endif
}
