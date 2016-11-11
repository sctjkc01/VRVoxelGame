using UnityEngine;
using System.Collections;
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

    public void PrerenderChunk() {
        ChunkMeshProducer cmp = new ChunkMeshProducer(chunk, gameObject);
        cmp.Priority = Vector3.Distance(Vector3.zero, transform.position) * 1f; // farther = less priority
        ThreadedJobPile.AddJob(cmp);
    }

    /*
    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + (Vector3.one * Chunk.chunkSize * 0.5f), Vector3.one * Chunk.chunkSize);
    }
    */
}

public class ChunkMeshProducer : ThreadedJob {
    private Chunk chunk;
    private GameObject go;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> tris = new List<int>();

    public ChunkMeshProducer(Chunk c, GameObject go) {
        chunk = c;
        this.go = go;
    }

    public override void Start() {
        base.Start();
    }

    protected override void ThreadFunction() {
        for(int x = 0; x < Chunk.chunkSize; x++) {
            for(int y = 0; y < Chunk.chunkSize; y++) {
                for(int z = 0; z < Chunk.chunkSize; z++) {
                    var block = chunk[x, y, z];

                    if(block.type != BlockType.AIR) {
                        buildCubeFace((y == Chunk.chunkSize - 1) || (chunk[x, y + 1, z].type == BlockType.AIR), x, y, z, Side.TOP, block);
                        buildCubeFace((y == 0) || (chunk[x, y - 1, z].type == BlockType.AIR), x, y, z, Side.BOTTOM, block);
                        buildCubeFace((z == Chunk.chunkSize - 1) || (chunk[x, y, z + 1].type == BlockType.AIR), x, y, z, Side.NORTH, block);
                        buildCubeFace((z == 0) || (chunk[x, y, z - 1].type == BlockType.AIR), x, y, z, Side.SOUTH, block);
                        buildCubeFace((x == Chunk.chunkSize - 1) || (chunk[x + 1, y, z].type == BlockType.AIR), x, y, z, Side.EAST, block);
                        buildCubeFace((x == 0) || (chunk[x - 1, y, z].type == BlockType.AIR), x, y, z, Side.WEST, block);
                    }
                }
            }
        }
    }

    private void buildCubeFace(bool visible, int x, int y, int z, Side side, Block b) {
        if(!visible) return;

        var vertexIndex = vertices.Count;

        switch(side) {
            case Side.TOP:
                vertices.AddRange(new Vector3[] { new Vector3(x, y + 1, z), new Vector3(x, y + 1, z + 1),
                                               new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y + 1, z) });
                break;
            case Side.NORTH:
                vertices.AddRange(new Vector3[] { new Vector3(x + 1, y, z + 1), new Vector3(x + 1, y + 1, z + 1),
                                               new Vector3(x, y + 1, z + 1), new Vector3(x, y, z + 1) });
                break;
            case Side.EAST:
                vertices.AddRange(new Vector3[] { new Vector3(x + 1, y, z), new Vector3(x + 1, y + 1, z),
                                               new Vector3(x + 1, y + 1, z + 1), new Vector3(x + 1, y, z + 1) });
                break;
            case Side.WEST:
                vertices.AddRange(new Vector3[] { new Vector3(x, y, z + 1), new Vector3(x, y + 1, z + 1),
                                               new Vector3(x, y + 1, z), new Vector3(x, y, z) });
                break;
            case Side.SOUTH:
                vertices.AddRange(new Vector3[] { new Vector3(x, y, z), new Vector3(x, y + 1, z),
                                               new Vector3(x + 1, y + 1, z), new Vector3(x + 1, y, z) });
                break;
            case Side.BOTTOM:
                vertices.AddRange(new Vector3[] { new Vector3(x + 1, y, z), new Vector3(x + 1, y, z + 1),
                                               new Vector3(x, y, z + 1), new Vector3(x, y, z) });
                break;
        }

        tris.AddRange(new int[] { vertexIndex, vertexIndex + 1, vertexIndex + 2,
                                  vertexIndex + 2, vertexIndex + 3, vertexIndex });
        Vector2 uv = b.getUv(side);
        uvs.AddRange(new Vector2[] { uv, uv, uv, uv });
    }

    protected override void OnFinished() {
        Mesh mesh = go.GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        //mesh.Optimize();

        go.GetComponent<MeshCollider>().sharedMesh = null;
        go.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}