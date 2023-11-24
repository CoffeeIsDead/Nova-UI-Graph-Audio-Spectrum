using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using Nova;

public class UIGraphNovaSpectrum : MonoBehaviour
{

    [Header("UIBlock Refs")]
    [SerializeField] List<UIBlock> dots;
    [SerializeField] List<UIBlock> lines;
    [SerializeField] List<UIBlock> sections;

    [Header("Fill Mesh Generator Ref")]
    [SerializeField] UIGraphMeshNova meshNova;

    [Header("Spectrum Data")]
    [SerializeField] CoffeeAudioSpectrum audioSpectrum;

    [Header("Sprites")]
    [SerializeField] Sprite dotSprite;
    [SerializeField] Sprite tileSprite;

    [Header("Sizes")]
    [SerializeField] float dotSize;
    [SerializeField] float lineWidth;
    [SerializeField] float sectionWidth;

    [Header("Colors")]
    [SerializeField] Color dotColor;
    [SerializeField] Color lineColor;
    [SerializeField] Color sectionColor;

    [Header("Points")]
    [SerializeField] public List<int> graphPoints;
    [SerializeField] List<string> graphPointsName;

    private float graphWidth;
    private float graphHeight;
    private float graphY;
    private float graphX;

    private void Awake()
    {

        graphHeight = this.gameObject.GetComponent<UIBlock>().Size.Y.Value;
        graphWidth = this.gameObject.GetComponent<UIBlock>().Size.X.Value;

    }

    private void Start()
    {

        BuildGraph();

    }

    private void Update()
    {

        UpdateGraph();

    }

    private void UpdateGraph()
    {

        UIBlock lastDot = null;

        for (int i = 0; i < graphPoints.Count; i++)
        {

            graphPoints[i] = Mathf.Clamp(Mathf.RoundToInt(audioSpectrum.bandsBuffer[i]), 0, 100);

            float newY = graphY * graphPoints[i];
            UIBlock dotHolder = dots[i];
            UIBlock sectionHolder = sections[i];
            UIBlock2D section = sectionHolder.GetChild(0).GetComponent<UIBlock2D>();

            dotHolder.Position.Y = newY;

            sectionHolder.Position.Y = newY;
            section.Size.Y = newY;

            if (lastDot != null)
            {

                UIBlock lineHolder = lines[i - 1];
                UIBlock2D line = lineHolder.GetChild(0).GetComponent<UIBlock2D>();
                Vector2 direction = (dotHolder.Position.XY.Value - lastDot.Position.XY.Value).normalized;
                float distance = Vector2.Distance(lastDot.Position.XY.Value, dotHolder.Position.XY.Value);
                Length2 position = lastDot.Position.XY.Value + direction * distance * 0.5f;
                lineHolder.Position = new Length3(position.X.Value, position.Y.Value, 0);
                lineHolder.transform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
                line.Size.X = distance;

            }

            lastDot = dotHolder;

        }

        meshNova.GetVertices();

    }

    public void BuildGraph()
    {

        graphY = graphHeight / 100f;
        graphX = graphWidth / graphPoints.Count;

        UIBlock lastDot = null;

        for (int i = 0; i < graphPoints.Count; i++)
        {

            float dotX;
            float dotY = graphY * graphPoints[i];

            if (graphPoints[i] == 0)
            {

                dotX = 0.5f * graphX;

            }
            else
            {

                dotX = ((i + 1) * graphX) - (graphX * 0.5f);

            }

            UIBlock newDot = CreateGraphDot(new Length3(dotX, dotY, 0), i);

            if (lastDot != null)
            {

                MakeGraphLine(lastDot.Position.XY.Value, newDot.Position.XY.Value, i);

            }
            else
            {

                MakeSectionLine(newDot.Position.XY.Value, i);

            }

            lastDot = newDot;

        }

        meshNova.GetVertices();

    }

    private UIBlock CreateGraphDot(Length3 position, int dotNum)
    {

        UIBlock dotHolder = dots[dotNum];
        UIBlock2D dot = dotHolder.transform.GetChild(0).GetComponent<UIBlock2D>();
        dotHolder.Position = position;
        dot.Color = dotColor;
        dot.SetImage(dotSprite);
        dot.Size = new Length3(dotSize, dotSize, 0);
        dot.Visible = true;

        return dotHolder;

    }

    private void MakeGraphLine(Vector2 dotPositionA, Vector2 dotPositionB, int dotNum)
    {

        UIBlock lineHolder = lines[dotNum - 1];
        UIBlock2D line = lineHolder.GetChild(0).GetComponent<UIBlock2D>();
        Vector2 direction = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        Length2 position = dotPositionA + direction * distance * 0.5f;
        lineHolder.Position = new Length3(position.X.Value, position.Y.Value, 0);
        lineHolder.transform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
        line.Color = lineColor;
        line.SetImage(tileSprite);
        line.Size = new Length3(distance, lineWidth, 0);
        line.Visible = true;

        if (dotNum < graphPoints.Count)
        {

            MakeSectionLine(dotPositionB, dotNum);

        }
    }

    private void MakeSectionLine(Vector2 postition, int dotNum)
    {

        UIBlock sectionHolder = sections[dotNum];
        UIBlock2D section = sectionHolder.GetChild(0).GetComponent<UIBlock2D>();
        sectionHolder.Position = new Length3(postition.x, postition.y, 0);
        section.Color = sectionColor;
        section.SetImage(tileSprite);
        section.Size = new Length3(sectionWidth, postition.y, 0);
        section.Visible = true;

    }
}
