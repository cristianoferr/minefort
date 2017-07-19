using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Utils {
	/// <summary>
	/// Isometric helper class with utility functions
	/// </summary>
	public class Isometric {
		[SerializeField]
		private static Matrix4x4 _isoMatrix;
		[SerializeField]
		public static float _isoAngle = 26.565f;

		public static readonly Vector2 North = new Vector2(1,0); 
		public static readonly Vector2 South = North*-1;
		public static readonly Vector2 East = new Vector2(0,-1);
		public static readonly Vector2 West = East * -1;
		public static readonly Vector2 NorthEast = North + East;
		public static readonly Vector2 NorthWest = North + West;
		public static readonly Vector2 SouthEast = South + East;
		public static readonly Vector2 SouthWest = South + West;

		public static float IsoAngle {
			get { return _isoAngle; }
			set {
				_isoAngle = value;
				_isoMatrix = GetIsoMatrix(value);
			}
		}

		private static Matrix4x4 IsoMatrix {
			get {
				if (_isoMatrix == Matrix4x4.identity)
					_isoMatrix = GetIsoMatrix(IsoAngle);
				return _isoMatrix;
			}
		}

		/// <summary>
		/// Rotates a vector from the isometric coordinate system to the unity coordinate system
		/// where xy are the screen coordinates in unity units
		/// z can be neglected.
		/// </summary>
		/// <param name="isoVector">Isometric Vector</param>
		/// <returns>Vector in screen coordinates</returns>
		public static Vector3 IsoToScreen(Vector3 isoVector) {
			return GetIsoMatrix(IsoAngle).MultiplyPoint(isoVector);
		}

		/// <summary>
		///  Rotates a vector from unity's coordinate system to the isometric coordinate system
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3 ScreenToIso(Vector3 vector) {
			return GetIsoMatrix(IsoAngle).inverse.MultiplyPoint(vector);
		}

		/// <summary>
		/// returns the matrix that convers from isometric space to unity space
		/// </summary>
		/// <returns></returns>
		private static Matrix4x4 GetIsoMatrix(float isoAngle) {
			var isoAngleInRad = Mathf.Deg2Rad * isoAngle;
			var arcsintan = Mathf.Asin(Mathf.Tan(isoAngleInRad)) * Mathf.Rad2Deg;
			var rot = Quaternion.AngleAxis(-arcsintan, Vector3.right) * Quaternion.AngleAxis(-45, Vector3.up);
			return Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
		}

		/// <summary>
		/// Finds the isometric position using a screenspacePoint in Pixels and an offset on the x Axis
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <param name="xOffset"></param>
		/// <returns>The isometric position (xOffset,y,z), null instead</returns>
		public static Vector3? CreateXYZfromX(Vector2 screenSpacePoint, float xOffset) {
			return CreateXYZ(screenSpacePoint, new Vector3(-1, 0, 0), xOffset);
		}
		/// <summary>
		/// Finds the isometric position using a screenspacePoint in Pixels and an offset on the z Axis
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <param name="<Offset">/></param>
		/// <returns>The isometric position (x,y,zOffset), null instead</returns>
		public static Vector3? CreateXYZfromZ(Vector2 screenSpacePoint, float zOffset) {
			return CreateXYZ(screenSpacePoint, new Vector3(0, 0, 1), zOffset);
		}

		/// <summary>
		/// Finds the isometric point given screenspacePoint in pixels and an offset on the yAxis
		/// Note: May use this to find an isometric position on the floor at screenspacePoint that is yOffset away from the camera on the y-Axis 
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <returns>The isometric position (x,yOffset,y), null instead</returns>
		public static Vector3? CreateXYZfromY(Vector2 screenSpacePoint, float yOffset) {
			return CreateXYZ(screenSpacePoint, new Vector3(0, -1, 0), yOffset);
		}

		private static Vector3? CreateXYZ(Vector2 screenSpacePoint, Vector3 planeNormalVector, float offset) {
			var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenSpacePoint.x, screenSpacePoint.y, Camera.main.nearClipPlane));

			var plane = new Plane(planeNormalVector, offset); //isometric plane that goes through Offset

			var matrixInverse = IsoMatrix.inverse;
			var isoRay = new Ray(matrixInverse.MultiplyPoint(worldPos), matrixInverse.MultiplyPoint(new Vector3(0, 0, 1))); // isometric ray at screenspacepoint

			float distance;
			if (plane.Raycast(isoRay, out distance)) {
				return isoRay.GetPoint(distance);
			}
			return null;
		}

		public Vector3 ScreenToIsoPoint(Vector2 screenSpacePoint, float zOffset = 0f) {
			return CreateXYZfromZ(screenSpacePoint, zOffset).Value;
		}


		/// <summary>
		/// Utility function for raycasting from a screenspace point 
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <returns></returns>
		public static Ray ScreenSpaceToIsoRay(Vector2 screenSpacePoint) {
			var ray = Camera.main.ScreenPointToRay(screenSpacePoint);
			
			//rotate the origin and direction to the isometric coordinate system 
			var isoRayOrigin = ScreenToIso(ray.origin);
			var isoRayDirection = ScreenToIso(ray.direction);
			
			return new Ray(isoRayOrigin,isoRayDirection);
		}

		/// <summary>
		/// Utility function for raycasting from the mouse position
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <returns></returns>
		public static Ray MouseToIsoRay() {
			return ScreenSpaceToIsoRay(Input.mousePosition);
		}

	}
}
