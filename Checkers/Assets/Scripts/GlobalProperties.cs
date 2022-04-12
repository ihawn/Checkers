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
    public static Material HighlighterMaterial { get; private set; }
    public static Color DarkColor { get; private set; }
    public static Color DarkerColor { get; private set; }
    public static GameObject ContainerObject { get; private set; }

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
    Material highlighterMaterial;

    [SerializeField]
    Color darkColor;

    [SerializeField]
    Color darkerColor;

    [SerializeField]
    GameObject containerObject;

    public void InitializeGlobalProperties()
    {
        SquaresPerBoardSide = squaresPerBoardSide;
        SquareLength = squareLength;
        Cube = cube;
        DefaultMaterial = defaultMaterial;
        HighlighterMaterial = highlighterMaterial;
        DarkColor = darkColor;
        DarkerColor = darkerColor;
        Cylinder = cylinder;
        ContainerObject = containerObject;
    }
}
