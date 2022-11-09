namespace GSGD1
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif //UNITY_EDITOR

	public class Path : MonoBehaviour
	{
		[SerializeField]
		private List<Transform> _waypoints = null;

		[SerializeField]
		private bool _showGizmos = true;

		[SerializeField]
		private float _gizmoStartSize = .5f;

		[SerializeField]
		private Color _lineColor = Color.white;

		[SerializeField]
		private Vector3 _gizmoLineYOffset = new Vector3(0, 0.5f, 0);

		public List<Transform> Waypoints
		{
			get
			{
				return _waypoints;
			}
		}

		public Transform FirstWaypoint
		{
			get
			{
				if (_waypoints != null && _waypoints.Count > 1)
				{
					return _waypoints[0];
				}
				else
				{
					return null;
				}
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (_showGizmos == false || _waypoints == null)
			{
				return;
			}

			Gizmos.color = _lineColor;
			Gizmos.DrawSphere(_waypoints[0].position, _gizmoStartSize);

			for (int i = 0, length = _waypoints.Count - 1; i < length; i++)
			{
				Transform currentWaypoint = _waypoints[i];
				Transform nextWaypoint = _waypoints[i + 1];
				if (currentWaypoint != null && nextWaypoint != null)
				{
					Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
					var color = Handles.color;
					Handles.color = _lineColor;
					{
						Handles.DrawLine(currentWaypoint.position + _gizmoLineYOffset, nextWaypoint.position + _gizmoLineYOffset);
					}
					Handles.color = color;
				}
			}

            Gizmos.color = Color.white;
            Gizmos.DrawRay(_waypoints[0].position + Vector3.up, ((_waypoints[1].position - _waypoints[0].position + Vector3.up).normalized * 2f));
        }
#endif //UNITY_EDITOR

	}
}