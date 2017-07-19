using Assets.UltimateIsometricToolkit.Scripts.External;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Core {
	/// <summary>
	/// Wrapper class for the current sorting strategy
	/// </summary>
	[ExecuteInEditMode]
	public class IsoSorting : Singleton<IsoSorting> {
		[SerializeField] private SortingStrategy _sortingStrategy;
		[HideInInspector]public bool Dirty = true;

		
		[SerializeField, HideInInspector] private float _isoAngle = 26.565f; //add

		
		[ExposeProperty]
		public float IsoAngle { 
			get { return _isoAngle; }
			set {
				Isometric._isoAngle = Mathf.Clamp(value, 0, 90);
				_isoAngle = Mathf.Clamp(value, 0, 90);
			}
		}

		void OnEnable() {
			Isometric.IsoAngle = IsoAngle;
		}

		void OnDisable() {
			Isometric.IsoAngle = IsoAngle;
		}
		
		void Start() {
			IsoAngle = _isoAngle;
		}


		public void Resolve(IsoTransform isoTransform) {
			Dirty = true;
			if (_sortingStrategy != null)
				_sortingStrategy.Resolve(isoTransform);
			else
				Debug.LogError("Missing SortingStrategy on IsoSorting component");
		}

		

		public void Update() {
			if (!Dirty)
				return;
			if (_sortingStrategy != null)
				_sortingStrategy.Sort();
			else
				Debug.LogError("Missing SortingStrategy on IsoSorting component");
			Dirty = false;
		}

		public void Remove(IsoTransform isoTransform) {
			if (_sortingStrategy != null)
				_sortingStrategy.Remove(isoTransform);
			else
				Debug.LogError("Missing SortingStrategy on IsoSorting component");
		}


		
	}
}
