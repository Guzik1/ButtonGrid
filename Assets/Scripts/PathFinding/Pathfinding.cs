using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PathFinding
{
    public class Pathfinding<ControllerType>
    {
        private const int MOVE_COST = 10;
        private const int STRAIGHT_COST = 14;

        List<PathNode<ControllerType>> grid;
        List<PathNode<ControllerType>> openList;
        List<PathNode<ControllerType>> closedList;

        int width, height;

        public Pathfinding(List<PathNode<ControllerType>> grid, int width, int height)
        {
            this.grid = grid;
            this.width = width;
            this.height = height;
        }

        public List<PathNode<ControllerType>> FindPath(PathNode<ControllerType> startNode, PathNode<ControllerType> endNode)
        {
            if (startNode == null || endNode == null)
            {
                return null;
            }

            openList = new List<PathNode<ControllerType>> { startNode };
            closedList = new List<PathNode<ControllerType>>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode<ControllerType> pathNode = grid[x * height + y];
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                   pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode<ControllerType> currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode<ControllerType> neighbourNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neighbourNode)) continue;
                    if (!neighbourNode.IsWalkable())
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }

            return null;
        }

        List<PathNode<ControllerType>> GetNeighbourList(PathNode<ControllerType> currentNode)
        {
            List<PathNode<ControllerType>> neighbourList = new List<PathNode<ControllerType>>();
            
            // Left
            if (currentNode.GetX() - 1 >= 0)
            {
                AddToNeighbourList(neighbourList, currentNode, -1, 0);
            }

            // Right
            if (currentNode.GetX() + 1 < width)
            {
                AddToNeighbourList(neighbourList, currentNode, +1, 0);
            }

            // Down
            if (currentNode.GetY() - 1 >= 0)
            {
                AddToNeighbourList(neighbourList, currentNode, 0, -1);
            }

            // Up
            if (currentNode.GetY() + 1 < height)
            {
                AddToNeighbourList(neighbourList, currentNode, 0, +1);
            }

            return neighbourList;
        }

        void AddToNeighbourList(List<PathNode<ControllerType>> neighbourList, PathNode<ControllerType> currentNode, int changeX, int changeY)
        {
            neighbourList.Add(grid[(currentNode.GetX() + changeX) * height + currentNode.GetY() + changeY]);
        }

        int CalculateDistanceCost(PathNode<ControllerType> a, PathNode<ControllerType> b)
        {
            int xDistance = Mathf.Abs(a.GetX() - b.GetX());
            int yDistance = Mathf.Abs(a.GetY() - b.GetY());
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
        }

        List<PathNode<ControllerType>> CalculatePath(PathNode<ControllerType> endNode)
        {
            List<PathNode<ControllerType>> path = new List<PathNode<ControllerType>>();
            path.Add(endNode);
            PathNode<ControllerType> currentNode = endNode;

            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }

            path.Reverse();
            return path;
        }

        PathNode<ControllerType> GetLowestFCostNode(List<PathNode<ControllerType>> pathNodeList)
        {
            PathNode<ControllerType> lowestFCostNode = pathNodeList[0];
            for (int i = 1; i < pathNodeList.Count; i++)
            {
                if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }

            return lowestFCostNode;
        }
    }
}