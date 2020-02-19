using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken from: http://wiki.unity3d.com/index.php?title=ReverseNormals&_ga=2.66939944.746567804.1581456812-786425175.1579363843
// Used so that we can invert the sphere and see the inside of the sphere (and apply a render texture to it)

[RequireComponent(typeof(MeshFilter))]
public class ReverseNormals : MonoBehaviour
{

	void Start()
	{
		MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (filter != null)
		{
			Mesh mesh = filter.mesh;

			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; i++)
				normals[i] = -normals[i];
			mesh.normals = normals;

			for (int m = 0; m < mesh.subMeshCount; m++)
			{
				int[] triangles = mesh.GetTriangles(m);
				for (int i = 0; i < triangles.Length; i += 3)
				{
					int temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);
			}
		}
	}
}
