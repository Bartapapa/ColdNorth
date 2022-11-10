namespace GSGD1
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(menuName = "GameSup/Tower Description", fileName ="TowerDescription")]
	public class TowerDescription : ScriptableObject
	{
		[SerializeField]
		private Tower _prefab = null;

		[SerializeField]
		private TowerFoundation _prefabFoundation = null;

        [SerializeField]
		private Sprite _icon = null;

		[SerializeField]
		private Color _iconColor = Color.white;

		[SerializeField]
		private int _woodCost = 0;

		[SerializeField]
		private int _stoneCost = 0;

        public Tower Prefab => _prefab;
		public Sprite Icon => _icon;
		public Color IconColor => _iconColor;
		public int WoodCost => _woodCost;
		public int StoneCost => _stoneCost;

		public TowerFoundation TowerFoundation => _prefabFoundation;

		public TowerFoundation Instantiate()
		{
			//TODO Instantiate foundation prefab.
			return GameObject.Instantiate(_prefabFoundation);
		}
	}
}