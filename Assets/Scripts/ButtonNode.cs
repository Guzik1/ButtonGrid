using Assets.Scripts.PathFinding;

namespace Assets.Scripts
{
    public class ButtonNode : PathNode<ButtonController>
    {
        public ButtonNode(ButtonController controller) : base(controller) { }

        public override int GetX()
        {
            return Controller.X - 1;
        }

        public override int GetY()
        {
            return Controller.Y - 1;
        }

        public override bool IsWalkable()
        {
            return Controller.Walkable;
        }
    }
}
