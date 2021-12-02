using Assets.Scripts;
using Assets.Scripts.PathFinding;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toolbag;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour, ButtonEvent
{
    public RectTransform ButtonsGridParent;
    public InputField XField;
    public InputField YField;

    public GameObject ButtonPrefab;

    public int InstanceObjecCountOnStart = 100;

    public Color BlockedButtonColor = Color.red;
    public Color FirstSelectedButtonColor = Color.blue;
    public Color SecondSelectedButtonColor = Color.cyan;
    public Color PathButtonsColor = Color.gray;
    public Color DefaultButtonColor = Color.white;

    List<PathNode<ButtonController>> buttonsGrid = new List<PathNode<ButtonController>>(50 * 50);

    Vector2 ButtonGridStartPos;

    ButtonController firstSelectedButton, secondSelectedButton;

    Pathfinding<ButtonController> pathfinding;

    public void Start()
    {
        ButtonGridStartPos = ButtonsGridParent.anchoredPosition;

        for (int i = 0; i < InstanceObjecCountOnStart; i++)
        {
            InstiantateButton(true);
        }
    }

    public void OnGenerateButtonClick()
    {
        Debug.Log($"Generate button grid { XField.text }x{ YField.text }");

        firstSelectedButton = null;
        secondSelectedButton = null;

        int xSize, ySize;

        if (!int.TryParse(XField.text, out xSize))
        {
            Debug.LogWarning($"X size is not a number");
            return;
        }

        if (!int.TryParse(YField.text, out ySize))
        {
            Debug.LogWarning($"Y size is not a number");
            return;
        }

        float buttonXSize, buttonYSize;
        buttonXSize = ButtonsGridParent.sizeDelta.x / xSize;
        buttonYSize = ButtonsGridParent.sizeDelta.y / ySize;

        ButtonsGridParent.anchoredPosition = new Vector2(ButtonGridStartPos.x + buttonXSize / 2, ButtonGridStartPos.y - buttonYSize / 2);

        int unusedAlocatedButtonCount = buttonsGrid.Count;

        int randomSeed = Random.Range(0, 4);
        randomSeed = randomSeed * 2;

        PathNode<ButtonController> node;
        ButtonController bc;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (unusedAlocatedButtonCount > 0)
                {
                    unusedAlocatedButtonCount--;

                    node = buttonsGrid[x * ySize + y];

                    if(!node.Controller.gameObject.activeSelf)
                        node.Controller.Show();

                    node.Controller.ClearText();
                }
                else
                {
                    node = InstiantateButton(false);
                }

                bc = node.Controller;
                bc.SetScale(buttonXSize, buttonYSize);
                bc.SetPosition(buttonXSize * x, -buttonYSize * y);

                bc.X = x + 1;
                bc.Y = y + 1;
                bc.ButtonEvent = this;
                bc.CurrentNode = node;

                if (node.GetHashCode() % 10 == randomSeed)
                {
                    bc.SetActive(false);
                    bc.SetColor(BlockedButtonColor);
                }
                else
                {
                    bc.SetActive(true);
                    bc.SetColor(DefaultButtonColor);
                }
            }
        }

        for (int i = buttonsGrid.Count - unusedAlocatedButtonCount; i < buttonsGrid.Count; i++)
        {
            buttonsGrid[i].Controller.Hide();
        }

        pathfinding = new Pathfinding<ButtonController>(buttonsGrid, xSize, ySize);
    }

    public void OnButtonClick(ButtonController controller)
    {
        if(firstSelectedButton == null)
        {
            firstSelectedButton = controller;

            controller.SetColor(FirstSelectedButtonColor);
            controller.SetCordsInText();
        }
        else if(secondSelectedButton == null && firstSelectedButton != controller)
        {
            secondSelectedButton = controller;

            controller.SetColor(SecondSelectedButtonColor);
            controller.SetCordsInText();

            StartSearchPath();
        }
    }

    void StartSearchPath()
    {
        /*List<PathNode<ButtonController>> result = Task.Run(() => {
            return pathfinding.FindPath(firstSelectedButton.CurrentNode, secondSelectedButton.CurrentNode);
        }).Result;

        OnSearchPathEnd(result);*/

        Task.Run(() =>
        {
            List<PathNode<ButtonController>> path = pathfinding.FindPath(firstSelectedButton.CurrentNode, secondSelectedButton.CurrentNode);

            Dispatcher.Invoke(() => {
                OnSearchPathEnd(path);
            });
        });
    }

    void OnSearchPathEnd(List<PathNode<ButtonController>> path)
    {
        if (path == null)
        {
            Debug.LogWarning("No path exist");
            return;
        }

        for (int i = 1; i < path.Count - 1; i++)
        {
            path[i].Controller.SetColor(PathButtonsColor);
            path[i].Controller.SetCordsWithDistanceInText(i);
        }

        path[path.Count - 1].Controller.SetCordsWithDistanceInText(path.Count - 1);
    }

    PathNode<ButtonController> InstiantateButton(bool hide)
    {
        GameObject newObj = Instantiate(ButtonPrefab, ButtonsGridParent);
        PathNode<ButtonController> node = new ButtonNode(newObj.GetComponent<ButtonController>());

        if(hide)
            node.Controller.Hide();

        buttonsGrid.Add(node);

        return node;
    }
}
