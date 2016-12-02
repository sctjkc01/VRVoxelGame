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
    private struct VoxelFace {
        public bool culled;
        public bool transparent;
        public Side side;
        public Block srcBlock;

        public static bool operator ==(VoxelFace l, VoxelFace r) {
            if(l.srcBlock == null && r.srcBlock == null) return true;
            if(l.srcBlock == null ^ r.srcBlock == null) return false;
            return l.srcBlock.type == r.srcBlock.type && l.transparent == r.transparent;
        }

        public static bool operator !=(VoxelFace l, VoxelFace r) {
            return !(l == r);
        }
    }

    private Chunk chunk;
    private GameObject go;

    private int x, y, z;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> tris = new List<int>();

    public ChunkMeshProducer(Chunk c, GameObject go) {
        chunk = c;
        this.go = go;
    }

    protected override void ThreadFunction() { // Function ported from Main.greedy() at https://github.com/roboleary/GreedyMesh/blob/master/src/mygame/Main.java

        int i, j, k, l, w, h, u, v, n;
        Side side = Side.TOP;
        int[] x = new int[] { 0, 0, 0 };
        int[] q = new int[] { 0, 0, 0 };
        int[] du = new int[] { 0, 0, 0 };
        int[] dv = new int[] { 0, 0, 0 };

        VoxelFace[] mask = new VoxelFace[Chunk.chunkSize * Chunk.chunkSize];

        VoxelFace comp1, comp2;

        VoxelFace nullFace = new VoxelFace() {
            srcBlock = null
        };

        for(bool backFace = true, b = false; b != backFace; backFace = backFace && b, b = !b) {
            for(int d = 0; d < 3; d++) {
                u = (d + 1) % 3;
                v = (d + 2) % 3;

                x = new int[] { 0, 0, 0 };
                q = new int[] { 0, 0, 0 };
                q[d] = 1;

                switch(d) {
                    case 0:
                        side = backFace ? Side.WEST : Side.EAST;
                        break;
                    case 1:
                        side = backFace ? Side.BOTTOM : Side.TOP;
                        break;
                    case 2:
                        side = backFace ? Side.SOUTH : Side.NORTH;
                        break;
                }

                for(x[d] = -1; x[d] < Chunk.chunkSize;) {
                    // Generate Mask
                    n = 0;
                    for(x[v] = 0; x[v] < Chunk.chunkSize; x[v]++) {
                        for(x[u] = 0; x[u] < Chunk.chunkSize; x[u]++) {
                            comp1 = (x[d] >= 0) ? GetVoxelFace(x[0], x[1], x[2], side) : nullFace;
                            comp2 = (x[d] < Chunk.chunkSize - 1) ? GetVoxelFace(x[0] + q[0], x[1] + q[1], x[2] + q[2], side) : nullFace;

                            mask[n++] = (comp1 != nullFace && comp2 != nullFace && comp1 == comp2) ? nullFace : (backFace ? comp2 : comp1);
                        }
                    }

                    x[d]++;

                    // Generate Mesh
                    n = 0;
                    for(j = 0; j < Chunk.chunkSize; j++) {
                        for(i = 0; i < Chunk.chunkSize; ) {
                            if(mask[n] == nullFace) {
                                // Compute width
                                for(w = 1; i + w < Chunk.chunkSize && mask[n + w] != nullFace && mask[n + w] == mask[n]; w++) { }

                                // Compute height
                                bool done = false;
                                for(h = 1; j + h < Chunk.chunkSize; h++) {
                                    for(k = 0; k < w; k++) {
                                        if(mask[n + k + h * Chunk.chunkSize] == nullFace || mask[n + k + h * Chunk.chunkSize] != mask[n]) { done = true; break; }
                                    }
                                    if(done) { break; }
                                }

                                if(!mask[n].culled) { // Ensure this face hasn't been culled
                                    x[u] = i;
                                    x[v] = j;

                                    du = new int[] { 0, 0, 0 };
                                    du[u] = w;
                                    dv = new int[] { 0, 0, 0 };
                                    dv[v] = h;

                                    var vertexIndex = vertices.Count;

                                    vertices.AddRange(new Vector3[] {
                                        new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2]),
                                    });

                                    var uv = mask[n].srcBlock.getUv(side);
                                    uvs.AddRange(new Vector2[] { uv, uv, uv, uv });

                                    if(backFace)
                                        tris.AddRange(new int[] { vertexIndex + 2, vertexIndex, vertexIndex + 1,
                                                                  vertexIndex + 1, vertexIndex + 3, vertexIndex + 2});
                                    else
                                        tris.AddRange(new int[] { vertexIndex + 2, vertexIndex + 3, vertexIndex + 1,
                                                                  vertexIndex + 1, vertexIndex, vertexIndex + 2});
                                }

                                // Empty the mask
                                for(l = 0; l < h; l++) {
                                    for(k = 0; k < w; k++) {
                                        mask[n + k + l * Chunk.chunkSize] = nullFace;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    private VoxelFace GetVoxelFace(int x, int y, int z, Side side) {
        Block b = chunk[x, y, z];
        Block o = null;
        switch(side) {
            case Side.NORTH:
                o = chunk[x, y, z + 1];
                break;
            case Side.EAST:
                o = chunk[x + 1, y, z];
                break;
            case Side.WEST:
                o = chunk[x - 1, y, z];
                break;
            case Side.SOUTH:
                o = chunk[x, y, z - 1];
                break;
            case Side.TOP:
                o = chunk[x, y + 1, z];
                break;
            case Side.BOTTOM:
                o = chunk[x, y - 1, z];
                break;
        }

        return new VoxelFace() {
            culled = (o != null && o.IsOpaque), srcBlock = b, side = side, transparent = !b.IsOpaque
        };
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

        Debug.Log("Chunk Render Complete");
    }
}