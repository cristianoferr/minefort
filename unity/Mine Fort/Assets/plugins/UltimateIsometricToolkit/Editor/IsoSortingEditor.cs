using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEditor;

namespace UltimateIsometricToolkit {
	[CustomEditor(typeof(IsoSorting)), CanEditMultipleObjects]
	public class IsoSortingEditor : Editor {

		IsoSorting instance;
		PropertyField[] instance_Fields;
		void OnEnable() {
			instance = target as IsoSorting;
			instance_Fields = ExposeProperties.GetProperties(instance);
		}

		public override void OnInspectorGUI() {
			if (instance == null)
				return;
			DrawDefaultInspector();
			ExposeProperties.Expose(instance_Fields);
		}
	}
}