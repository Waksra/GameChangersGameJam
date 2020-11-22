using UnityEngine;

namespace World
{
    public class GridController : StaticManager<GridController>
    {
        public Vector2Int gridWorldSize;
        public float cellSize = 1f;

        private float _cellRadius;
        private Vector2Int _gridSize;
        private Vector3 _startPosition;

        private FlowField _flowField;
        private GridDebug _debug;

        public float CellRadius => _cellRadius;
        public Vector2Int GridSize => _gridSize;

        public Vector3 StartPosition => _startPosition;

        private void OnValidate()
        {
            _cellRadius = cellSize / 2;
            _gridSize.x = (int) (gridWorldSize.x / cellSize);
            _gridSize.y = (int) (gridWorldSize.y / cellSize);

            _startPosition.x = gridWorldSize.x / 2f;
            _startPosition.z = gridWorldSize.y / 2f;
            _startPosition.y = -_cellRadius;
            _startPosition -= transform.position;
        }

        private void Start()
        {
            InitializeFlowField();
            if (TryGetComponent(out _debug))
                _debug.SetFlowField(_flowField);

            _flowField.CreateCostField();
        }

        private void InitializeFlowField()
        {
            _flowField = new FlowField(_cellRadius, _gridSize, _startPosition);
            _flowField.CreateGrid();
        }

        public Vector3 GetDirectionFromWorldPosition(Vector3 worldPosition)
        {
            Cell cell = _flowField.GetCellFromWorldPos(worldPosition);
            Vector3 direction;
            direction.x = cell.bestDirection.Vector.x;
            direction.z = cell.bestDirection.Vector.y;
            direction.y = 0.0f;
            return direction;
        }

        public void PathToWorldPosition(Vector3 worldPosition)
        {
            Cell targetCell = _flowField.GetCellFromWorldPos(worldPosition);
            if (targetCell == _flowField.destinationCell)
                return;

            PathToCell(targetCell);
        }

        public void PathToCell(Cell targetCell)
        {
            _flowField.CreateIntegrationField(targetCell);
            _flowField.CreateFlowField();
            _debug.DrawFlowField();
        }

        /*private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
    
            Vector3 size = Vector3.one * cellSize;
            
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector3 center = new Vector3(cellSize * x + _cellRadius, 0, cellSize * y + _cellRadius);
                    center -= _startPosition;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }*/
    }
}