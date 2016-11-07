using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour {
    public Chunk chunk;

    public Block this[int x, int y, int z] {
        get {
            return chunk[x, y, z];
        }
        set {
            chunk[x, y, z] = value;
        }
        
    }

    void Start() {

    }

    void PrerenderChunk() {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        for(int x = 0; x < Chunk.chunkSize; x++) {
            for(int y = 0; y < Chunk.chunkSize; y++) {
                for(int z = 0; z < Chunk.chunkSize; z++) {
                    var block = chunk[x, y, z];

                    if(block.type != BlockType.AIR) {
                        buildCubeFace((y == Chunk.chunkSize - 1) || (chunk[x, y + 1, z].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.TOP, block);
                        buildCubeFace((y == 0) || (chunk[x, y - 1, z].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.BOTTOM, block);
                        buildCubeFace((z == Chunk.chunkSize - 1) || (chunk[x, y, z + 1].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.NORTH, block);
                        buildCubeFace((z == 0) || (chunk[x, y, z - 1].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.SOUTH, block);
                        buildCubeFace((x == Chunk.chunkSize - 1) || (chunk[x + 1, y, z].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.EAST, block);
                        buildCubeFace((x == 0) || (chunk[x - 1, y, z].type == BlockType.AIR), vertices, uvs, tris, x, y, z, Side.WEST, block);
                    }
                }
            }
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        //mesh.Optimize();

        GetComponent<MeshCollider>().sharedMesh = null;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    void buildCubeFace(bool visible, List<Vector3> verts, List<Vector2> uvs, List<int> tris, int x, int y, int z, Side side, Block b) {
        if(!visible) return;

        var vertexIndex = verts.Count;

        switch(side) {
            case Side.TOP:
                verts.AddRange(new Vector3[] { new Vector3(x, y + 1, z), new Vector3(x, y + 1, z + 1),
                                               new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y + 1, z) });
                break;
            case Side.NORTH:
                verts.AddRange(new Vector3[] { new Vector3(x + 1, y, z + 1), new Vector3(x + 1, y + 1, z + 1),
                                               new Vector3(x, y + 1, z + 1), new Vector3(x, y, z + 1) });
                break;
            case Side.EAST:
                verts.AddRange(new Vector3[] { new Vector3(x + 1, y, z), new Vector3(x + 1, y + 1, z),
                                               new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y, z + 1) });
                break;
            case Side.WEST:
                verts.AddRange(new Vector3[] { new Vector3(x, y, z + 1), new Vector3(x, y + 1, z + 1),
                                               new Vector3(x, y + 1, z), new Vector3(x, y, z) });
                break;
            case Side.SOUTH:
                verts.AddRange(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y + 1, z),
                                               new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y, z) });
                break;
            case Side.BOTTOM:
                verts.AddRange(new Vector3[] { new Vector3(x + 1, y, z), new Vector3(x + 1, y, z + 1),
                                               new Vector3(x, y, z + 1), new Vector3(x, y, z) });
                break;
        }

        tris.AddRange(new int[] { vertexIndex, vertexIndex + 1, vertexIndex + 2,
                                  vertexIndex + 2, vertexIndex + 3, vertexIndex });
        uvs.AddRange(b.getUvs(side));
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (Vector3.one * Chunk.chunkSize * 0.5f), Vector3.one * Chunk.chunkSize);
    }
}
