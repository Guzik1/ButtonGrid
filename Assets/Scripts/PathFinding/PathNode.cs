namespace Assets.Scripts.PathFinding
{
    public abstract class PathNode<controllerType>
    {
        public controllerType Controller;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode<controllerType> cameFromNode;

        public PathNode(){}

        public PathNode(controllerType controller){
            Controller = controller;
        }

        public abstract int GetX();

        public abstract int GetY();

        public abstract bool IsWalkable();

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}
