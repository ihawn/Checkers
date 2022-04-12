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
        Game = new CheckersGame(PlayerType.DumbAI, PlayerType.DumbAI);
    }

    void Update()
    {
        int blackPieceCount = Game.Board.Pieces.Where(p => p.Color == Color.black && Game.Board.CalculatePossibleMovesForPiece(p).Count() > 0).Count();
        int whitePieceCount = Game.Board.Pieces.Where(p => p.Color == Color.white && Game.Board.CalculatePossibleMovesForPiece(p).Count() > 0).Count();

        if(blackPieceCount > 0 && whitePieceCount > 0)
        {
            switch (Game.CurrentPlayer.Type)
            {
                case PlayerType.Human:
                    GetHumanInput();
                    break;

                case PlayerType.DumbAI:
                    GetDumbInput();
                    break;

                case PlayerType.SmartAI:

                    break;
            }
        }       
    }

    void GetHumanInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.name);
                ObjectLinker linker = hit.transform.gameObject.GetComponent<ObjectLinker>();

                if(linker.LinkedObject is CheckersPiece piece)// && piece.Color == Game.CurrentPlayer.PlayerColor)
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

                        SwitchPlayer();
                    }
                }
            }
        }
    }

    void GetDumbInput()
    {
        List<CheckersPiece> piecesOfOwnColor = Game.Board.Pieces.Where(p => p.Color == Game.CurrentPlayer.PlayerColor).ToList();
        foreach(CheckersPiece piece in piecesOfOwnColor)
        {
            Game.Board.Pieces.FirstOrDefault(p => p.BoardPosition == piece.BoardPosition).PossibleMoves = Game.Board.CalculatePossibleMovesForPiece(piece);
        }

        List<CheckersPiece> piecesWithMoves = Game.Board.Pieces.Where(p => p.PossibleMoves.Count > 0 && p.Color == Game.CurrentPlayer.PlayerColor).ToList();
        CheckersPiece selectedPiece = piecesWithMoves[Random.Range(0, piecesWithMoves.Count)];
        Vector2 chosenMove = selectedPiece.PossibleMoves[Random.Range(0, selectedPiece.PossibleMoves.Count)];
        selectedPiece.MovePieceTo(chosenMove);
        SwitchPlayer();
    }

    void SwitchPlayer()
    {
        Game.CurrentPlayer = Game.CurrentPlayer.PlayerColor == Color.black ? Game.Player2 : Game.Player1;
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

    public static void DestroyPiece(CheckersPiece piece, CheckersBoard board)
    {
        board.Squares.FirstOrDefault(s => s.BoardPosition == piece.BoardPosition).OccupyingPiece = null;
        Destroy(piece.PieceGameObject);
        board.Pieces.Remove(piece);
    }

    public static void Coronation(CheckersPiece piece)
    {
        GameObject crown = Instantiate(GlobalProperties.Crown);
        crown.transform.position = piece.PieceGameObject.transform.position - new Vector3(0, 0, 0.6f); ;
        crown.transform.parent = piece.PieceGameObject.transform;
    }
}
