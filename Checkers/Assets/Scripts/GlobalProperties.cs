using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProperties : MonoBehaviour
{
    public static int SquaresPerBoardSide { get; private set; }
    public static float SquareLength { get; private set; }
    public static Mesh Cube { get; private set; }
    public static Mesh Cylinder { get; private set; }

    public static Material DefaultMaterial { get; private set; }
    public static Color DarkColor { get; private set; }

    [SerializeField]
    int squaresPerBoardSide = 8;

    [SerializeField]
    float squareLength = 5;

    [SerializeField]
    Mesh cube;

    [SerializeField]
    Mesh cylinder;

    [SerializeField]
    Material defaultMaterial;

    [SerializeField]
    Color darkColor;

    public void InitializeGlobalProperties()
    {
        SquaresPerBoardSide = squaresPerBoardSide;
        SquareLength = squareLength;
        Cube = cube;
        DefaultMaterial = defaultMaterial;
        DarkColor = darkColor;
        Cylinder = cylinder;
    }
}
