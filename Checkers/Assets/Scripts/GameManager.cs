using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GlobalProperties GlobalProperties { get; private set; }
    public CheckersGame Game { get; private set; }
    public GameObject Highlighter { get; private set; }


    void Awake()
    {
        GlobalProperties = GetComponent<GlobalProperties>();
        GlobalProperties.InitializeGlobalProperties();
        HighlighterInit();
        Game = new CheckersGame(PlayerType.Human, PlayerType.Human);
    }

    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                ObjectLinker linker = hit.transform.gameObject.GetComponent<ObjectLinker>();

                if(linker.LinkedObject is CheckersPiece piece && piece.Color == Game.CurrentPlayer.PlayerColor)
                {
                    HighlightPiece(piece);
                    HighlightPossibleMoves(piece);
                }
                else if(linker.LinkedObject is CheckersSquare square)
                {
                    CheckersPiece selectedPiece = Game.CurrentPlayer.SelectedPiece;
                    if(selectedPiece != null && selectedPiece.PossibleMoves.Contains(square.BoardPosition))
                    {
                        selectedPiece.MovePieceTo(square.BoardPosition);
                        Highlighter.SetActive(false);
                        ResetLastMoveHighlight();
                    }
                }
            }
        }
    }

    void HighlightPiece(CheckersPiece piece)
    {
        piece.PossibleMoves = Game.Board.CalculatePossibleMovesForPiece(piece);
        Game.CurrentPlayer.SelectedPiece = piece;
        Highlighter.SetActive(true);
        Highlighter.transform.position = piece.PieceGameObject.transform.position + Vector3.forward*4.5f;
    }

    void HighlightPossibleMoves(CheckersPiece piece)
    {
        List<CheckersSquare> darkSquares = ResetLastMoveHighlight();
        List<GameObject> possibleMoveGameObjects = darkSquares.Where(s => piece.PossibleMoves.Contains(s.BoardPosition)).Select(x => x.SquareGameObject).ToList();

        foreach(GameObject square in possibleMoveGameObjects)
            square.GetComponent<Renderer>().material.SetColor("_Color", GlobalProperties.DarkerColor);
    }

    List<CheckersSquare> ResetLastMoveHighlight()
    {
        List<CheckersSquare> darkSquares = Game.Board.Squares.Where(s => s.Color == GlobalProperties.DarkColor).ToList();
        foreach (var square in darkSquares)
            square.SquareGameObject.GetComponent<Renderer>().material.SetColor("_Color", GlobalProperties.DarkColor);
        return darkSquares;
    }

    void HighlighterInit()
    {
        Highlighter = new GameObject("Highlighter", typeof(MeshRenderer), typeof(MeshFilter));
        Highlighter.GetComponent<MeshFilter>().mesh = GlobalProperties.Cylinder;
        Highlighter.GetComponent<Renderer>().material = GlobalProperties.HighlighterMaterial;
        Highlighter.transform.localScale = Vector3.one * GlobalProperties.SquareLength * 0.9f;
        Highlighter.transform.Rotate(90, 0, 0);
        Highlighter.transform.parent = GlobalProperties.ContainerObject.transform;
        Highlighter.SetActive(false);
    }

    public static GameObject MakeGameObjectForObject(Mesh mesh, string name, Vector2 absolutePosition, Vector3 positionOffset, Vector3 rotation, Color color, float scaleFactor = 1, float heightScaleFactor = 1)
    {
        GameObject g = new GameObject(name, typeof(ObjectLinker), typeof(MeshRenderer), typeof(MeshFilter), typeof(BoxCollider));
        g.transform.position = absolutePosition;
        g.transform.position += positionOffset;
        g.transform.Rotate(rotation);
        g.transform.localScale = Vector3.one * GlobalProperties.SquareLength * scaleFactor;
        g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y * heightScaleFactor, g.transform.localScale.z);
        g.GetComponent<MeshFilter>().mesh = mesh;
        g.GetComponent<Renderer>().material = GlobalProperties.DefaultMaterial;
        g.GetComponent<Renderer>().material.SetColor("_Color", color);
        g.transform.parent = GlobalProperties.ContainerObject.transform;

        return g;
    }
}
