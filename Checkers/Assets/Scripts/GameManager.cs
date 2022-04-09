using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GlobalProperties GlobalProperties { get; private set; }
    public CheckersGame Game { get; private set; }

    void Awake()
    {
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));

        GlobalProperties = GetComponent<GlobalProperties>();
        GlobalProperties.InitializeGlobalProperties();

        Game = new CheckersGame();
    }

    void Update()
    {
        
    }

    public static GameObject MakeGameObjectForObject(Mesh mesh, string name, Vector2 absolutePosition, Vector3 positionOffset, Vector3 rotation, Color color, object link, float scaleFactor = 1, float heightScaleFactor = 1)
    {
        GameObject g = new GameObject(name, typeof(ObjectLinker), typeof(MeshRenderer), typeof(MeshFilter));
        g.transform.position = absolutePosition;
        g.transform.position += positionOffset;
        g.transform.Rotate(rotation);
        g.transform.localScale = Vector3.one * GlobalProperties.SquareLength * scaleFactor;
        g.transform.localScale = new Vector3(g.transform.localScale.x, g.transform.localScale.y * heightScaleFactor, g.transform.localScale.z);
        g.GetComponent<ObjectLinker>().LinkedObject = link;
        g.GetComponent<MeshFilter>().mesh = mesh;
        g.GetComponent<Renderer>().material = GlobalProperties.DefaultMaterial;
        g.GetComponent<Renderer>().material.color = color;

        return g;
    }
}
