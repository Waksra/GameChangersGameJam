using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public class FlowField
    {
        public Cell destinationCell;
        
        public Cell[,] Grid { get; private set; }
        public Vector2Int GridSize { get; private set; }
        public float CellRadius { get; private set; }
        public Vector3 StartPosition { get; private set; }

        private float cellDiameter;

        public FlowField(float cellRadius, Vector2Int gridSize, Vector3 startPosition)
        {
            CellRadius = cellRadius;
            cellDiameter = cellRadius * 2f;
            GridSize = gridSize;
            StartPosition = startPosition;
        }

        public void CreateGrid()
        {
            Grid = new Cell[GridSize.x, GridSize.y];

            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Vector3 worldPosition = new Vector3(cellDiameter * x + CellRadius, 0.0f, cellDiameter * y + CellRadius);
                    worldPosition -= StartPosition;
                    Grid[x,y] = new Cell(worldPosition, new Vector2Int(x, y));
                }
            }
        }

        public void CreateCostField()
        {
            Vector3 cellHalfExtents = Vector3.one * CellRadius;
            int layerMask = 1 << (int) Layers.Impassible;

            foreach (Cell cell in Grid)
            {
                Collider[] obstacles =
                    Physics.OverlapBox(cell.worldPos, cellHalfExtents, Quaternion.identity, layerMask);
                foreach (Collider collider in obstacles)
                {
                    switch ((Layers)collider.gameObject.layer)
                    {
                        case Layers.Impassible:
                            cell.Cost = 255;
                            continue;
                    }
                }
            }
        }

        public void CreateIntegrationField(Cell destinationCell)
        {
            ResetCellBestCost();
            
            if (this.destinationCell != null)
                this.destinationCell.isDestination = false;
            
            this.destinationCell = destinationCell;

            destinationCell.isDestination = true;
            destinationCell.bestCost = 0;

            Queue<Cell> cellsToCheck = new Queue<Cell>();
            cellsToCheck.Enqueue(destinationCell);

            while (cellsToCheck.Count > 0)
            {
                Cell currentCell = cellsToCheck.Dequeue();
                List<Cell> neighbourCells = GetNeighbourCells(currentCell.gridIndex, GridDirection.CardinalDirections);
                foreach (Cell neighbourCell in neighbourCells)
                {
                    if(neighbourCell.Cost == byte.MaxValue) continue;
                    ushort testCost = (ushort)(neighbourCell.Cost + currentCell.bestCost);
                    if (testCost < neighbourCell.bestCost)
                    {
                        neighbourCell.bestCost = testCost;
                        cellsToCheck.Enqueue(neighbourCell);
                    }
                }
            }
        }

        public void CreateFlowField()
        {
            foreach (Cell cell in Grid)
            {
                List<Cell> neighbours = GetNeighbourCells(cell.gridIndex, GridDirection.AllDirections);
                int bestCost = cell.bestCost;
                foreach (Cell neighbour in neighbours)
                {
                    if (neighbour.bestCost < bestCost)
                    {
                        bestCost = neighbour.bestCost;
                        cell.bestDirection =
                            GridDirection.GetDirectionFromVector2Int(neighbour.gridIndex - cell.gridIndex);
                    }
                }
            }
        }

        public void ResetCellBestCost()
        {
            foreach (Cell cell in Grid)
            {
                cell.bestCost = ushort.MaxValue;
            }
        }

        private List<Cell> GetNeighbourCells(Vector2Int gridIndex, List<GridDirection> directions)
        {
            List<Cell> neighbourCells = new List<Cell>();

            foreach (Vector2Int direction in directions)
            {
                Cell neighbour = GerCellAtRelativePosition(gridIndex, direction);
                if (neighbour != null)
                    neighbourCells.Add(neighbour);
            }

            return neighbourCells;
        }

        private Cell GerCellAtRelativePosition(Vector2Int origin, Vector2Int direction)
        {
            Vector2Int cellPosition = origin + direction;

            if (cellPosition.x < 0 || cellPosition.x >= GridSize.x || cellPosition.y < 0 ||
                cellPosition.y >= GridSize.y)
                return null;

            return Grid[cellPosition.x, cellPosition.y];
        }
        
        public Cell GetCellFromWorldPos(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + StartPosition.x) / (GridSize.x * cellDiameter);
            float percentY = (worldPosition.z + StartPosition.z) / (GridSize.y * cellDiameter);
 
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
 
            int x = Mathf.Clamp(Mathf.RoundToInt((GridSize.x) * percentX), 0, GridSize.x - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt((GridSize.y) * percentY), 0, GridSize.y - 1);
            
            return Grid[x, y];
        }

        private enum Layers
        {
            Impassible = 8
        }
    }
}