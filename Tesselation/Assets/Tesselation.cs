using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tesselation : MonoBehaviour
{
    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;

    private Mesh m_Mesh;

    [SerializeField]
    private int m_Subdivisions = 0;
    [SerializeField]
    private float m_Radius = 1;


    void Awake()
    {
        m_Mesh = new Mesh();
        m_Mesh.name = "Octahedron";
        GetComponent<MeshFilter>().mesh = m_Mesh;
        m_Vertices = new List<Vector3>();
        m_Triangles = new List<int>();
    }

    void Start()
    {
        GenerateTesselatedMesh(m_Subdivisions, m_Radius);
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 150 * Time.deltaTime, 0));
    }

    //This function will set the default mesh
    private void GenerateMesh()
    {
        m_Vertices.Add(new Vector3(0, 1, 0)); //boven1
        m_Vertices.Add(new Vector3(0, 0, -1));//achter2
        m_Vertices.Add(new Vector3(1, 0, 0)); //rechts3
        m_Vertices.Add(new Vector3(0, 0, 1)); //voor4
        m_Vertices.Add(new Vector3(-1, 0, 0));//links5
        m_Vertices.Add(new Vector3(0, -1, 0));//onder6

        m_Triangles.Add(0); m_Triangles.Add(1); m_Triangles.Add(4);//links achter boven 1
        m_Triangles.Add(0); m_Triangles.Add(2); m_Triangles.Add(1);//rechts achter boven2
        m_Triangles.Add(0); m_Triangles.Add(3); m_Triangles.Add(2);//rechts voor boven  3
        m_Triangles.Add(0); m_Triangles.Add(4); m_Triangles.Add(3);//links voor boven   4
        m_Triangles.Add(5); m_Triangles.Add(4); m_Triangles.Add(1);//links achter onder 5
        m_Triangles.Add(5); m_Triangles.Add(1); m_Triangles.Add(2);//rechts achter onder6
        m_Triangles.Add(5); m_Triangles.Add(2); m_Triangles.Add(3);//rechts voor onder  7
        m_Triangles.Add(5); m_Triangles.Add(3); m_Triangles.Add(4);//links voor onder   8

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.triangles = m_Triangles.ToArray();

        m_Mesh.RecalculateNormals();
    }

    //This function will generate the tesselation for the default mesh
    private void GenerateTesselatedMesh(int subDivs, float rad)
    {
        GenerateMesh();

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        for (int j = 0; j < subDivs; j++)
        {
            for (int x = 0; x < m_Vertices.Count; x++)
            {
                newVertices.Add(Normalize(m_Vertices[x], m_Radius));
            }

            int StartCount = newVertices.Count;

            for (int i = 0; i < m_Triangles.Count; i += 3)
            {
                int one = m_Triangles[i];
                int two = m_Triangles[i + 1];
                int three = m_Triangles[i + 2];

                Vector3 newOne = Normalize((m_Vertices[one] + m_Vertices[two]) * 0.5f, m_Radius);
                Vector3 newTwo = Normalize((m_Vertices[one] + m_Vertices[three]) * 0.5f, m_Radius);
                Vector3 newThree = Normalize((m_Vertices[two] + m_Vertices[three]) * 0.5f, m_Radius);

                newVertices.Add(newOne);
                newVertices.Add(newTwo);
                newVertices.Add(newThree);

                newTriangles.Add(one);
                newTriangles.Add(StartCount + i);
                newTriangles.Add(StartCount + i + 1);

                newTriangles.Add(StartCount + i);
                newTriangles.Add(StartCount + i + 2);
                newTriangles.Add(StartCount + i + 1);

                newTriangles.Add(StartCount + i);
                newTriangles.Add(two);
                newTriangles.Add(StartCount + i + 2);

                newTriangles.Add(StartCount + i + 1);
                newTriangles.Add(StartCount + i + 2);
                newTriangles.Add(three);
            }
            TransferList(m_Vertices, newVertices);
            TransferList(m_Triangles, newTriangles);
        }

        m_Mesh.vertices = m_Vertices.ToArray();
        m_Mesh.triangles = m_Triangles.ToArray();

        m_Mesh.RecalculateNormals();
    }

    //This is a self written normalize function
    private Vector3 Normalize(Vector3 vector, float targetLength)
    {
        float length = Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z));

        float x = vector.x / length * targetLength;
        float y = vector.y / length * targetLength;
        float z = vector.z / length * targetLength;

        return new Vector3(x, y, z);
    }

    //These functions are used to copy lists to a new list
    private void TransferList(List<Vector3> listOne, List<Vector3> listTwo)
    {
        listOne.Clear();

        for (int i = 0; i < listTwo.Count; i++)
        {
            listOne.Add(listTwo[i]);
        }
        listTwo.Clear();
    }
    private void TransferList(List<int> listOne, List<int> listTwo)
    {
        listOne.Clear();

        for (int i = 0; i < listTwo.Count; i++)
        {
            listOne.Add(listTwo[i]);
        }
        listTwo.Clear();
    }
}