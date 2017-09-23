using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk2 : MonoBehaviour
{
    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _mesh;

	void Start () {
		_mesh = new Mesh();
	    _mesh.name = "Chunck";

        AddCube();

        _mesh.vertices = _vertices.ToArray();
	    _mesh.triangles = _triangles.ToArray();

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = _mesh;
        GetComponent<MeshFilter>().mesh = _mesh;
	}

    private void AddCube()
    {
        AddCubeFront();
        AddCubeBack();
        AddCubeLeft();
        AddCubeRight();
        AddCubeTop();
        AddCubeBottom();
    }

    private void AddCubeFront()
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, -0.5f));
        _vertices.Add(new Vector3(-0.5f, 0.5f, -0.5f));
    }

    private void AddCubeBack()
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f, -0.5f, 0.5f));
        _vertices.Add(new Vector3(0.5f, -0.5f, 0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, 0.5f));
        _vertices.Add(new Vector3(-0.5f, 0.5f, 0.5f));
    }

    private void AddCubeLeft()
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f, -0.5f, 0.5f));
        _vertices.Add(new Vector3(-0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(-0.5f, 0.5f, -0.5f));
        _vertices.Add(new Vector3(-0.5f, 0.5f, 0.5f));
    }

    private void AddCubeRight()
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(0.5f, -0.5f, 0.5f));
        _vertices.Add(new Vector3(0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, 0.5f));
    }

    private void AddCubeTop()
    {
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f, 0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, 0.5f, 0.5f));
        _vertices.Add(new Vector3(-0.5f, 0.5f, 0.5f));
    }

    private void AddCubeBottom()
    {
        _triangles.Add(1 + _vertices.Count);
        _triangles.Add(2 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(3 + _vertices.Count);
        _triangles.Add(0 + _vertices.Count);
        _triangles.Add(1 + _vertices.Count);

        _vertices.Add(new Vector3(-0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, -0.5f, -0.5f));
        _vertices.Add(new Vector3(0.5f, -0.5f, 0.5f));
        _vertices.Add(new Vector3(-0.5f, -0.5f, 0.5f));
    }
}
