using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public GlobalProperties GlobalProperties { get; private set; }
    public CheckersGame Game { get; private set; }
    public GameObject Highlighter { get; private set; }
    public UIController UIController { get; private set; }
    public bool CanMove { get; private set; }

    [SerializeField]
    PlayerType Player1Type;

    [SerializeField]
    int Player1HeuristicId;

    [SerializeField]
    PlayerType Player2Type;

    [SerializeField]
    int Player2HeuristicId;

    [SerializeField]
    bool UsePruning;

    [SerializeField]
    string CurrentPlayer;


    void Awake()
    {
        GlobalProperties = GetComponent<GlobalProperties>();
        UIController = GetComponent<UIController>();
        GlobalProperties.InitializeGlobalProperties();
        HighlighterInit();

        Player1Type = PlayerType.Human;
        Player2Type = PlayerType.SmartAI;
        UIController.ShowMenuScreen();
    }

    void Update()
    {
        if(!UIController.InMenus)
        {
            CurrentPlayer = Game.CurrentPlayer.PlayerColor == Color.black ? "black" : "white";

            int blackPiecesCount = Game.Board.Pieces.Where(p => p.Color == Color.black && Game.Board.CalculatePossibleMovesForPiece(p).Count() > 0).Count();
            int whitePiecesCount = Game.Board.Pieces.Where(p => p.Color == Color.white && Game.Board.CalculatePossibleMovesForPiece(p).Count() > 0).Count();
            Game.Board.BlackPiecesCount = blackPiecesCount;
            Game.Board.WhitePiecesCount = whitePiecesCount;

            if (blackPiecesCount > 0 && whitePiecesCount > 0)
            {
                switch (Game.CurrentPlayer.Type)
                {
                    case PlayerType.Human:
                        GetHumanInput();
                        break;

                    case PlayerType.DumbAI:
                        GetDumbInput();
                        break;

                    case PlayerType.KindaDumbAI:
                        GetSmartInput(1, Game.CurrentPlayer.HeuristicId);
                        break;

                    case PlayerType.SmartAI:
                        GetSmartInput(3, Game.CurrentPlayer.HeuristicId);
                        break;

                    case PlayerType.ReallySmartAI:
                        GetSmartInput(5, Game.CurrentPlayer.HeuristicId);
                        break;

                    case PlayerType.GeniusAI:
                        GetSmartInput(7, Game.CurrentPlayer.HeuristicId);
                        break;

                    case PlayerType.Cthulu:
                        GetSmartInput(9, Game.CurrentPlayer.HeuristicId);
                        break;
                }
            }
            else
            {
                UIController.ShowGameOverScreen(whitePiecesCount == 0 ? "black" : "white");
            }
        }
    }

    public void SetPlayerType(string arg)
    {
        var args = arg.Split(',');
        string player = args[0];
        string playerType = args[1];

        PlayerType type;
        switch (playerType)
        {
            case "Human":
                type = PlayerType.Human;
                break;
            case "Very Easy":
                type = PlayerType.DumbAI;
                break;
            case "Easy":
                type = PlayerType.KindaDumbAI;
                break;
            case "Medium":
                type = PlayerType.SmartAI;
                break;
            case "Hard":
                type = PlayerType.ReallySmartAI;
                break;
            case "Expert":
                type = PlayerType.GeniusAI;
                break;
            case "Master":
                type = PlayerType.Cthulu;
                break;
            default:
                type = PlayerType.Human;
                break;
        }

        if (player == "1")
            Player1Type = type;
        else if (player == "2")
            Player2Type = type;
    }    

    public void StartGame()
    {
        CanMove = true;
        foreach (Transform t in GlobalProperties.ContainerObject.transform)
            Destroy(t.gameObject);
        Game = new CheckersGame(Player1Type, Player2Type, Player1HeuristicId, Player2HeuristicId);
        UIController.ShowGameOverlay(showGameStats: true);
        StartCoroutine(StartGameDelay());
    }

    void GetHumanInput()
    {
        if(CanMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    ObjectLinker linker = hit.transform.gameObject.GetComponent<ObjectLinker>();

                    if (linker.LinkedObject is CheckersPiece piece && piece.Color == Game.CurrentPlayer.PlayerColor)
                    {
                        HighlightPiece(piece);
                        HighlightPossibleMoves(piece);
                    }
                    else if (linker.LinkedObject is CheckersSquare square)
                    {
                        CheckersPiece selectedPiece = Game.CurrentPlayer.SelectedPiece;
                        if (selectedPiece != null && selectedPiece.PossibleMoves.Contains(square.BoardPosition))
                        {
                            StartCoroutine(MovePieceSmoothly(selectedPiece, square.BoardPosition));
                        }
                    }
                }
            }
        }
    }

    void GetDumbInput()
    {
        if(CanMove)
        {
            List<CheckersPiece> piecesOfOwnColor = Game.Board.Pieces.Where(p => p.Color == Game.CurrentPlayer.PlayerColor).ToList();
            foreach (CheckersPiece piece in piecesOfOwnColor)
            {
                Game.Board.Pieces.FirstOrDefault(p => p.BoardPosition == piece.BoardPosition).PossibleMoves = Game.Board.CalculatePossibleMovesForPiece(piece);
            }

            List<CheckersPiece> piecesWithMoves = Game.Board.Pieces.Where(p => p.PossibleMoves.Count > 0 && p.Color == Game.CurrentPlayer.PlayerColor).ToList();
            CheckersPiece selectedPiece = piecesWithMoves[UnityEngine.Random.Range(0, piecesWithMoves.Count)];
            Vector2 chosenMove = selectedPiece.PossibleMoves[UnityEngine.Random.Range(0, selectedPiece.PossibleMoves.Count)];
            StartCoroutine(MovePieceSmoothly(selectedPiece, chosenMove));
        }
    }

    void GetSmartInput(int depth, int heuristicId)
    {
        if(CanMove)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            int alpha = int.MinValue;
            int beta = int.MaxValue;

            RawCheckersBoard rawBoard = new RawCheckersBoard(Game.Board);

            //eval, board, move, piece
            (int, RawCheckersBoard, (int, int), (int, int)) minEvaluation = (int.MaxValue, rawBoard, (-1, -1), (-1, -1));
            (int, RawCheckersBoard, (int, int), (int, int)) maxEvaluation = (int.MinValue, rawBoard, (-1, -1), (-1, -1));
            (int, RawCheckersBoard, (int, int), (int, int), int) minimaxResult = TreeOptimizer.Minimax((rawBoard, (-1, -1), (-1, -1)), minEvaluation, maxEvaluation, depth, depth, (0, 0), Game.CurrentPlayer.PlayerColor == Color.black, (0, 0), UsePruning, alpha, beta, heuristicId, 0);

            Vector2 newMovePosition = new Vector2(minimaxResult.Item3.Item1, minimaxResult.Item3.Item2);
            Vector2 pieceToMovePosition = new Vector2(minimaxResult.Item4.Item1, minimaxResult.Item4.Item2);

            while (pieceToMovePosition.x == -1 && depth > 2) //gets triggered when depth is larger than the number of moves left
            {
                depth -= 2;
                minEvaluation = (int.MaxValue, rawBoard, (-1, -1), (-1, -1));
                maxEvaluation = (int.MinValue, rawBoard, (-1, -1), (-1, -1));
                minimaxResult = TreeOptimizer.Minimax((rawBoard, (-1, -1), (-1, -1)), minEvaluation, maxEvaluation, depth, depth, (0, 0), Game.CurrentPlayer.PlayerColor == Color.black, (0, 0), UsePruning, alpha, beta, heuristicId, 0);

                newMovePosition = new Vector2(minimaxResult.Item3.Item1, minimaxResult.Item3.Item2);
                pieceToMovePosition = new Vector2(minimaxResult.Item4.Item1, minimaxResult.Item4.Item2);
            }

            //failsafe
            if (pieceToMovePosition.x == -1)
                pieceToMovePosition = Game.Board.Pieces.FirstOrDefault(p => p.Color == Game.CurrentPlayer.PlayerColor && p.PossibleMoves.Count > 0).BoardPosition;

            if (newMovePosition.x == -1)
            {
                CheckersPiece newPiece = Game.Board.Pieces.FirstOrDefault(p => p.BoardPosition == pieceToMovePosition);
                if (newPiece.PossibleMoves.Count > 0)
                    newMovePosition = newPiece.PossibleMoves[0];
                else
                    newMovePosition = Game.Board.Pieces.FirstOrDefault(p => p.Color == Game.CurrentPlayer.PlayerColor && p.PossibleMoves.Count > 0).PossibleMoves[0];
            }
            watch.Stop();

            CheckersPiece pieceToMove = Game.Board.Pieces.FirstOrDefault(p => p.BoardPosition == pieceToMovePosition);
            StartCoroutine(MovePieceSmoothly(pieceToMove, newMovePosition));

            Debug.Log("Moved (" + pieceToMovePosition.x + ", " + pieceToMovePosition.y + ") to (" + newMovePosition.x + ", " + newMovePosition.y + ")");
            UIController.PrintGameStats(watch.ElapsedMilliseconds / 1000f, minimaxResult.Item5, minimaxResult.Item1);
        }
    }

    IEnumerator MovePieceSmoothly(CheckersPiece piece, Vector2 newBoardPosition)
    {
        CanMove = false;

        Highlighter.SetActive(false);
        ResetLastMoveHighlight();

        Vector2 boardOffset = newBoardPosition - piece.BoardPosition;
        Vector2 movementOffset = boardOffset * GlobalProperties.SquareLength;
        Vector3 newPosition = piece.PieceGameObject.transform.position + new Vector3(movementOffset.x, movementOffset.y, 0f);
        Vector3 slerpCenter = (piece.PieceGameObject.transform.localPosition + newPosition) / 2;

        if (Mathf.Abs(boardOffset.x) > 1)
        {
            while(Vector3.Distance(piece.PieceGameObject.transform.position, newPosition) > 0.03f)
            {
                piece.PieceGameObject.transform.localPosition = Vector3.Slerp(piece.PieceGameObject.transform.localPosition - slerpCenter, newPosition - slerpCenter, Mathf.Min(Time.deltaTime, 1/30f) * GlobalProperties.LerpSpeed) + slerpCenter;
                yield return null;
            }
        }
        else
        {
            while (Vector3.Distance(piece.PieceGameObject.transform.position, newPosition) > 0.03f)
            {
                piece.PieceGameObject.transform.localPosition = Vector3.Lerp(piece.PieceGameObject.transform.localPosition, newPosition, Mathf.Min(Time.deltaTime, 1 / 30f) * GlobalProperties.LerpSpeed);
                yield return null;
            }
        }
        piece.PieceGameObject.transform.localPosition = newPosition;

        piece.MovePieceTo(newBoardPosition);
        SwitchPlayer();
        CanMove = true;
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

    public void TogglePruning(string state)
    {
        UsePruning = state == "on" ? true : false;
    }

    void HighlighterInit()
    {
        Highlighter = new GameObject("Highlighter", typeof(MeshRenderer), typeof(MeshFilter));
        Highlighter.GetComponent<MeshFilter>().mesh = GlobalProperties.Cylinder;
        Highlighter.GetComponent<Renderer>().material = GlobalProperties.HighlighterMaterial;
        Highlighter.transform.localScale = Vector3.one * GlobalProperties.SquareLength * 0.9f;
        Highlighter.transform.Rotate(90, 0, 0);
        Highlighter.SetActive(false);
    }


    public GameObject MakeGameObjectForObject(Mesh mesh, string name, Vector2 absolutePosition, Vector3 positionOffset, Vector3 rotation, Color color, float startDelay, float scaleFactor = 1, float heightScaleFactor = 1)
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

        StartCoroutine(SpawnObjectSmoothly(g, startDelay));

        return g;
    }
    IEnumerator SpawnObjectSmoothly(GameObject g, float startDelay)
    {
        Vector3 originalScale = g.transform.localScale;
        g.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(startDelay);

        while (Vector3.Distance(originalScale, g.transform.localScale) > 0.03f)
        {
            g.transform.localScale = Vector3.Lerp(g.transform.localScale, originalScale, Time.deltaTime * GlobalProperties.ScaleSpeed);
            yield return null;
        }
        g.transform.localScale = originalScale;
    }

    IEnumerator StartGameDelay()
    {
        UIController.InMenus = true;
        yield return new WaitForSeconds(2);
        UIController.InMenus = false;
    }

    public static void DestroyPiece(CheckersPiece piece, CheckersBoard board)
    {
        board.Squares.FirstOrDefault(s => s.BoardPosition == piece.BoardPosition).OccupyingPiece = null;
        Destroy(piece.PieceGameObject);
        board.Pieces.Remove(piece);
    }

    public void Coronation(CheckersPiece piece)
    {
        GameObject crown = Instantiate(GlobalProperties.Crown);
        crown.transform.position = piece.PieceGameObject.transform.position - new Vector3(0, 0, 0.6f); ;
        crown.transform.parent = piece.PieceGameObject.transform;
        StartCoroutine(SpawnObjectSmoothly(crown, 0));
    }
}