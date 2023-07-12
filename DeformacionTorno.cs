using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class DeformacionTorno : MonoBehaviour
{
    private Dictionary<Vector3, List<Vector3>> vertexNeighbors;
    private Dictionary<Vector3, List<int>> vertexTriangles;
    private Dictionary<Vector3, List<Vector3>> vertexDuplicates;
    private Mesh deformerMesh;
    public Collider deformer;

    [System.Serializable]
    public enum EjeHorizontal
    {
        EjeX,
        EjeY,
        EjeZ
    }
    [Tooltip("Cantidad de deformacion que sufrira el objeto")]
    [SerializeField]
    float valorResta = 0.002f;

    [Tooltip("Precision para la colision, cuanto mas bajo mas distancia")]
    [SerializeField]
    float precisionColision = 0.001f;

    [SerializeField]
    EjeHorizontal ejeHorizontal;

    void Start()
    {
        deformerMesh = GetComponent<MeshFilter>().mesh;
        StoreVertexNeighbors();
        StoreVertexTriangles();
        StoreVertexDuplicates();
        Collider collider = GetComponent<MeshCollider>();
        collider.contactOffset = precisionColision;
        deformer.contactOffset = precisionColision;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 hitPoint = collision.contacts[0].point;
        hitPoint = transform.InverseTransformPoint(hitPoint);
        Vector3 nearestVertex = FindNearestVertex(hitPoint);
        OffsetVertices(nearestVertex, hitPoint);
    }

    void StoreVertexNeighbors()
    {
        vertexNeighbors = new Dictionary<Vector3, List<Vector3>>();

        // Accede a la malla del objeto de deformaci�n
        Vector3[] vertices = deformerMesh.vertices;
        int[] triangles = deformerMesh.triangles;

        // Itera a trav�s de cada tri�ngulo
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Obtiene los �ndices de los v�rtices del tri�ngulo
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // Obtiene los v�rtices correspondientes
            Vector3 vertex1 = vertices[index1];
            Vector3 vertex2 = vertices[index2];
            Vector3 vertex3 = vertices[index3];

            // Almacena los vecinos para cada v�rtice
            AddVertexNeighbor(vertex1, vertex2);
            AddVertexNeighbor(vertex1, vertex3);
            AddVertexNeighbor(vertex2, vertex1);
            AddVertexNeighbor(vertex2, vertex3);
            AddVertexNeighbor(vertex3, vertex1);
            AddVertexNeighbor(vertex3, vertex2);
        }
    }

    void AddVertexNeighbor(Vector3 vertex, Vector3 neighbor)
    {
        // Verifica si el v�rtice ya est� en el diccionario
        if (!vertexNeighbors.ContainsKey(vertex))
        {
            vertexNeighbors[vertex] = new List<Vector3>();
        }

        // Agrega el vecino a la lista de vecinos del v�rtice
        vertexNeighbors[vertex].Add(neighbor);
    }

    void StoreVertexTriangles()
    {
        vertexTriangles = new Dictionary<Vector3, List<int>>();

        // Accede a la malla del objeto de deformaci�n
        Mesh deformerMesh = GetComponent<MeshFilter>().mesh;
        int[] triangles = deformerMesh.triangles;

        // Itera a trav�s de cada tri�ngulo
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Obtiene los �ndices de los v�rtices del tri�ngulo
            int index1 = triangles[i];
            int index2 = triangles[i + 1];
            int index3 = triangles[i + 2];

            // Obtiene los v�rtices correspondientes
            Vector3 vertex1 = deformerMesh.vertices[index1];
            Vector3 vertex2 = deformerMesh.vertices[index2];
            Vector3 vertex3 = deformerMesh.vertices[index3];

            // Almacena el �ndice del tri�ngulo para cada v�rtice
            AddVertexTriangle(vertex1, i / 3);
            AddVertexTriangle(vertex2, i / 3);
            AddVertexTriangle(vertex3, i / 3);
        }
    }

    void AddVertexTriangle(Vector3 vertex, int triangleIndex)
    {
        // Verifica si el v�rtice ya est� en el diccionario
        if (!vertexTriangles.ContainsKey(vertex))
        {
            vertexTriangles[vertex] = new List<int>();
        }

        // Agrega el �ndice del tri�ngulo a la lista de tri�ngulos del v�rtice
        vertexTriangles[vertex].Add(triangleIndex);
    }

    void StoreVertexDuplicates()
    {
        vertexDuplicates = new Dictionary<Vector3, List<Vector3>>();

        // Accede a la malla del objeto de deformaci�n
        Mesh deformerMesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = deformerMesh.vertices;

        // Itera a trav�s de cada v�rtice
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            // Verifica si el v�rtice ya est� en el diccionario
            if (!vertexDuplicates.ContainsKey(vertex))
            {
                vertexDuplicates[vertex] = new List<Vector3>();
            }

            // Agrega el v�rtice duplicado a la lista de duplicados del v�rtice
            vertexDuplicates[vertex].Add(vertex);
        }
    }

    Vector3 FindNearestVertex(Vector3 targetPoint)
    {
        Vector3[] vertices = deformerMesh.vertices;
        float minDistance = float.MaxValue;
        Vector3 nearestVertex = Vector3.zero;

        // Itera a trav�s de cada v�rtice
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            float distance = (vertex - targetPoint).magnitude;

            // Verifica si el v�rtice actual est� m�s cerca que el v�rtice m�s cercano previo
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestVertex = vertex;
            }
        }

        return nearestVertex;
    }

    void OffsetVertices(Vector3 centerVertex, Vector3 intersectionPoint)
    {
        if (vertexDuplicates.ContainsKey(centerVertex))
        {
            // Obtiene todos los v�rtices correspondientes al v�rtice m�s cercano de la colisi�n
            List<Vector3> duplicateVertices = vertexDuplicates[centerVertex];
            Vector3[] vertices = deformerMesh.vertices;

            for (int i = 0; i < duplicateVertices.Count; i++)
            {
                Vector3 vertex = duplicateVertices[i];

                // Busca el v�rtice en el array
                for (int j = 0; j < vertices.Length; j++)
                {
                    switch (ejeHorizontal)
                    {
                        case EjeHorizontal.EjeX:
                            if (ApproximatelyEqual(vertices[j].x, vertex.x, 0.001f))
                            {
                                Vector3 offset = vertices[j];
                                offset = CalculoOffset(vertices[j], offset);
                                vertices[j] = offset;
                            }
                            break;

                        case EjeHorizontal.EjeY:
                            if (ApproximatelyEqual(vertices[j].y, vertex.y, 0.001f))
                            {
                                Vector3 offset = vertices[j];
                                offset = CalculoOffset(vertices[j], offset);
                                vertices[j] = offset;
                            }
                            break;

                        case EjeHorizontal.EjeZ:
                            if (ApproximatelyEqual(vertices[j].z, vertex.z, 0.001f))
                            {
                                Vector3 offset = vertices[j];
                                offset = CalculoOffset(vertices[j], offset);
                                vertices[j] = offset;
                            }
                            break;
                    }
                }
            }

            // Actualiza las nuevas posiciones de los v�rtices en la malla y en el diccionario
            deformerMesh.vertices = vertices;
            StoreVertexDuplicates();
        }

        deformerMesh.RecalculateNormals();
        deformerMesh.RecalculateBounds();
        GetComponent<MeshCollider>().sharedMesh = deformerMesh;
    }

    private bool ApproximatelyEqual(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    private Vector3 CalculoOffset(Vector3 centerVertex, Vector3 offset)
    {
        Vector3 centro = new Vector3();
        switch (ejeHorizontal) {

            case EjeHorizontal.EjeX:
                centro = new Vector3(centerVertex.x, 0, 0);
                break;
            case EjeHorizontal.EjeY:
                centro = new Vector3(0, centerVertex.y, 0);
                break;
            case EjeHorizontal.EjeZ:
                centro = new Vector3(0, 0, centerVertex.z);
                break;

        }
        

        Vector3 nuestraNormal = new Vector3(centro.x - centerVertex.x, centro.y - centerVertex.y, centro.z - centerVertex.z);
        nuestraNormal = nuestraNormal.normalized;

        offset = new Vector3(offset.x + (nuestraNormal.x * valorResta), offset.y + (nuestraNormal.y * valorResta), offset.z + (nuestraNormal.z * valorResta));

        return offset;
    }
}
