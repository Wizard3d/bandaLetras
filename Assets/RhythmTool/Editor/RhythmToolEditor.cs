using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(RhythmTool))]
public class RhythmToolEditor : Editor {



	public override void OnInspectorGUI()
	{
        serializedObject.Update();

        RhythmTool rhythmTool = (RhythmTool)target;
		
		EditorGUILayout.LabelField("Total frames:", rhythmTool.totalFrames.ToString());
		EditorGUILayout.LabelField("Last Frame:", rhythmTool.lastFrame.ToString());
		EditorGUILayout.LabelField("Current Frame:", rhythmTool.currentFrame.ToString());
		EditorGUILayout.Separator();
	
		EditorGUILayout.LabelField("BPM:", rhythmTool.bpm.ToString());
		EditorGUILayout.LabelField("Beat Length:", rhythmTool.beatLength.ToString() + " frames");
		EditorGUILayout.Separator();

		EditorGUI.BeginDisabledGroup(Application.isPlaying);

		SerializedProperty calculateTempo = serializedObject.FindProperty("_calculateTempo");
		EditorGUILayout.PropertyField(calculateTempo);

		SerializedProperty preCalculate = serializedObject.FindProperty("_preCalculate");
		EditorGUILayout.PropertyField(preCalculate);

		if(preCalculate.boolValue){
			SerializedProperty storeAnalyses = serializedObject.FindProperty("_storeAnalyses");
			EditorGUILayout.PropertyField(storeAnalyses);
		}

		EditorGUI.EndDisabledGroup();

		if(!preCalculate.boolValue) {
			SerializedProperty lead = serializedObject.FindProperty("_lead");
			EditorGUILayout.IntSlider(lead,300,10000);
		}

		serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
	}
}
