using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GL_drawer : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static Material lineMaterial;

    // public TextAsset startPoints;
    // public TextAsset endPoints;
    int ptCount = 286263;
    // int ptCount = 129051;

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    Vector3[] starts;
    Vector3[] ends;

    public GameObject camera;

    public int darkness = 78;
    public float scaleF = 0.01f;

    public Material lineMat;

    float speed;
    Vector3 lastSpeed = new Vector3(0, 0, 0);
    Vector3 locationLastFrame;
    
    // Will be called after all regular rendering is done
    void Start()
    {
        starts = new Vector3[ptCount];
        ends = new Vector3[ptCount];

        string currentLine;
        // TextReader reader = File.OpenText("D:/Luccia/PointCloud/Assets/TextFile/CarpenterStarts.txt");
        TextReader reader = File.OpenText("C:/GSD_2016_Fall/PointCloud/Assets/TextFile/_Gund_starts.txt");

        for (int i = 0; i < ptCount; i++)
        {
            currentLine = reader.ReadLine();
            starts[i] = new Vector3(System.Convert.ToSingle(currentLine.Split(',')[0]) / scaleF,
                                    System.Convert.ToSingle(currentLine.Split(',')[1]) / scaleF,
                                    System.Convert.ToSingle(currentLine.Split(',')[2]) / scaleF);

        }

        reader.Close();

        // TextReader reader2 = File.OpenText("D:/Luccia/PointCloud/Assets/TextFile/CarpenterEnds.txt");
        TextReader reader2 = File.OpenText("C:/GSD_2016_Fall/PointCloud/Assets/TextFile/_Gund_ends.txt");

        for (int i = 0; i < ptCount; i++)
        {
            currentLine = reader2.ReadLine();
            ends[i] = new Vector3(System.Convert.ToSingle(currentLine.Split(',')[0]) / scaleF,
                                  System.Convert.ToSingle(currentLine.Split(',')[1]) / scaleF,
                                  System.Convert.ToSingle(currentLine.Split(',')[2]) / scaleF);

        }
        locationLastFrame = camera.transform.position;
    }

    void Update()
    {
        if (speed <= 5)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                speed += 0.03f;
            }

            else if (speed >= 0)
            {
                if (speed >= 0.0f)
                    speed -= 0.1f;
            }
        }
        else
        {
            speed -= 0.3f;
        }
        camera.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_WalkSpeed = speed;
        Debug.Log(speed);

        // if (Input.GetMouseButtonDown(0))
            // Application.CaptureScreenshot(Time.frameCount.ToString() + ".png", 6);

        //speed = camera.GetComponent<Rigidbody>().velocity.magnitude;
    }

    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);
        // lineMat.SetPass(0);


        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        if (true)
        {
            GL.Begin(GL.LINES);
            for (int i = 0; i < ptCount / (4 + speed * 4); i++)
            {
                // float a = i / (float)lineCount;
                // float angle = a * Mathf.PI * 2;
                // Vertex colors change from red to green
                // GL.Color(new Color(1, 1, 1, 0.1F));
                int index = Random.Range(0, ptCount - 1);
                float c = Vector3.Distance((starts[index] + ends[index]) / 2, camera.transform.position) / darkness;
                
                GL.Color(new Color(1-c, 1-c, 1-c, 0.1f));
                
                // One vertex at transform position
                GL.Vertex3(starts[index].y / (1 + speed / 10), 
                           starts[index].z * (1 + speed / 10), 
                           starts[index].x / (1 + speed / 10));
                
                // Another vertex at edge of circle
                GL.Vertex3(ends[index].y / (1 + speed / 10), 
                           ends[index].z * (1 + speed / 10), 
                           ends[index].x / (1 + speed / 10));
                
            }
            GL.End();
        }

        if (false)
        {
            // Draw lines
            GL.Begin(GL.QUADS);
            for (int i = 0; i < ptCount / 1; i++)
            {
                // float a = i / (float)lineCount;
                // float angle = a * Mathf.PI * 2;
                // Vertex colors change from red to green
                // GL.Color(new Color(1, 1, 1, 0.1F));
                int index = Random.Range(0, ptCount - 1);
                float c = Vector3.Distance((starts[index] + ends[index]) / 2, camera.transform.position) / darkness;
                
                drawQuads(c, starts[index], ends[index]);
            }
            GL.End();
        }

        if (false)
        {
            GL.Begin(GL.TRIANGLES);
            GL.PopMatrix();
            for (int i = 0; i < ptCount / 10; i++)
            {
                int index = Random.Range(0, ptCount - 1);
                drawTriangle(starts[index], new Color(0, 1, 1, 0.3f));
                drawTriangle(ends[index], new Color(0.5f, 0.4f, 1, 0.3f));
            }
            GL.End();
        }
    }

    void drawTriangle(Vector3 pt, Color color)
    {
        GL.Color(color);
        GL.Vertex3(pt.y, pt.z - .05f, pt.x);
        GL.Vertex3(pt.y - .015f, pt.z + .015f, pt.x);
        GL.Vertex3(pt.y + .015f, pt.z + .015f, pt.x);
    }

    void drawQuads(float c, Vector3 start, Vector3 end)
    {
        GL.Color(new Color(1 - c, 1 - c, 1, 0.2f));
        GL.Vertex3(start.y, start.z, -start.x);
        GL.Vertex3(start.y, start.z + 0.1f, -start.x);
        GL.Vertex3(end.y, end.z, -end.x);
        GL.Vertex3(end.y, end.z + .1f, -end.x);
        
    }
}