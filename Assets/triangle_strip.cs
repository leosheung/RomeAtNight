using UnityEngine;
using System.Collections;

public class triangle_strip : MonoBehaviour
{
    public TextAsset startPoints;
    public TextAsset endPoints;

    public Material mat;
    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        // GL.LoadIdentity();
        GL.MultMatrix(gameObject.transform.localToWorldMatrix);
        GL.Begin(GL.TRIANGLE_STRIP);
        GL.Color(new Color(0, 0, 0, 1));
        GL.Vertex3(0.25F, 0.5F, 0);
        GL.Vertex3(0, 0.5F, 0);
        GL.Vertex3(0.25F, 0.25F, 0);
        GL.Vertex3(0, 0.25F, 0);
        GL.End();
        GL.PopMatrix();
    }
}