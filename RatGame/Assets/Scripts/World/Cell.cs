using UnityEngine;

namespace World
{
	public class Cell
	{
		public Vector3 worldPos;
		public Vector2Int gridIndex;
		public ushort bestCost;
		public GridDirection bestDirection;
		
		private byte _cost;

		public byte Cost
		{
			get => _cost;
			set
			{
				if(value >= byte.MaxValue) _cost = value;
			}
		}

		public Cell(Vector3 worldPos, Vector2Int gridIndex)
		{
			this.worldPos = worldPos;
			this.gridIndex = gridIndex;
			_cost = 1;
			bestCost = ushort.MaxValue;
			bestDirection = GridDirection.None;
		}
	}
}
