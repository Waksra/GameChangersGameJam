using UnityEditor;
using UnityEngine;

namespace World
{
    public enum FlowFieldDisplayType { None, AllIcons, DestinationIcon, CostField, IntegrationField };
 
    public class GridDebug : MonoBehaviour
    {
        public GridController gridController;
        public bool displayGrid;
 
        public FlowFieldDisplayType curDisplayType;
 
        private Vector2Int _gridSize;
        private float _cellRadius;
        private FlowField _curFlowField;
 
        private Sprite[] _ffIcons;
 
        private void Start()
        {
            _ffIcons = Resources.LoadAll<Sprite>("Sprites/FFIcons");
        }
 
        public void SetFlowField(FlowField newFlowField)
        {
            _curFlowField = newFlowField;
            _cellRadius = newFlowField.CellRadius;
            _gridSize = newFlowField.GridSize;
        }
    
        public void DrawFlowField()
        {
            ClearCellDisplay();
 
            switch (curDisplayType)
            {
                case FlowFieldDisplayType.AllIcons:
                    DisplayAllCells();
                    break;
 
                case FlowFieldDisplayType.DestinationIcon:
                    DisplayDestinationCell();
                    break;
            }
        }
 
        private void DisplayAllCells()
        {
            if (_curFlowField == null) { return; }
            foreach (Cell curCell in _curFlowField.Grid)
            {
                DisplayCell(curCell);
            }
        }
 
        private void DisplayDestinationCell()
        {
            if (_curFlowField == null) { return; }
            DisplayCell(_curFlowField.destinationCell);
        }
 
        private void DisplayCell(Cell cell)
        {
            GameObject iconGO = new GameObject();
            SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
            iconGO.transform.parent = transform;
            iconGO.transform.position = cell.worldPos;
 
            if (cell.Cost == 0)
            {
                iconSR.sprite = _ffIcons[3];
                Quaternion newRot = Quaternion.Euler(90, 0, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.Cost == byte.MaxValue)
            {
                iconSR.sprite = _ffIcons[2];
                Quaternion newRot = Quaternion.Euler(90, 0, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.North)
            {
                iconSR.sprite = _ffIcons[0];
                Quaternion newRot = Quaternion.Euler(90, 0, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.South)
            {
                iconSR.sprite = _ffIcons[0];
                Quaternion newRot = Quaternion.Euler(90, 180, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.East)
            {
                iconSR.sprite = _ffIcons[0];
                Quaternion newRot = Quaternion.Euler(90, 90, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.West)
            {
                iconSR.sprite = _ffIcons[0];
                Quaternion newRot = Quaternion.Euler(90, 270, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.NorthEast)
            {
                iconSR.sprite = _ffIcons[1];
                Quaternion newRot = Quaternion.Euler(90, 0, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.NorthWest)
            {
                iconSR.sprite = _ffIcons[1];
                Quaternion newRot = Quaternion.Euler(90, 270, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.SouthEast)
            {
                iconSR.sprite = _ffIcons[1];
                Quaternion newRot = Quaternion.Euler(90, 90, 0);
                iconGO.transform.rotation = newRot;
            }
            else if (cell.bestDirection == GridDirection.SouthWest)
            {
                iconSR.sprite = _ffIcons[1];
                Quaternion newRot = Quaternion.Euler(90, 180, 0);
                iconGO.transform.rotation = newRot;
            }
            else
            {
                iconSR.sprite = _ffIcons[0];
            }
        }
 
        public void ClearCellDisplay()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }
    
        private void OnDrawGizmos()
        {
            if (displayGrid)
            {
                if (_curFlowField == null)
                {
                    DrawGrid(gridController.GridSize, Color.yellow, gridController.CellRadius);
                }
                else
                {
                    DrawGrid(_gridSize, Color.green, _cellRadius);
                }
            }
        
            if (_curFlowField == null) { return; }
 
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
 
            switch (curDisplayType)
            {
                case FlowFieldDisplayType.CostField:
 
                    foreach (Cell curCell in _curFlowField.Grid)
                    {
                        Handles.Label(curCell.worldPos, curCell.Cost.ToString(), style);
                    }
                    break;
                
                case FlowFieldDisplayType.IntegrationField:
 
                    foreach (Cell curCell in _curFlowField.Grid)
                    {
                        Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
                    }
                    break;
            }
        
        }
 
        private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
        {
            Gizmos.color = drawColor;
            for (int x = 0; x < drawGridSize.x; x++)
            {
                for (int y = 0; y < drawGridSize.y; y++)
                {
                    Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
                    center -= gridController.StartPosition;
                    Vector3 size = Vector3.one * drawCellRadius * 2;
                    Gizmos.DrawWireCube(center, size);
                }
            }
        }
    }
}