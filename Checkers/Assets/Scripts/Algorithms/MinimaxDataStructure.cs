using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MinimaxDataStructure<BoardType, PieceType>
{
    public virtual BoardType Board { get; set; }
    public virtual List<PieceType> Moves { get; set; }
    public virtual PieceType Piece { get; set; }
}

public class DecisionTreeBranchResult : MinimaxDataStructure<RawCheckersBoard, (int, int)>
{
    
}
