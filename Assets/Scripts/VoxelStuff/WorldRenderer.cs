using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldRenderer : MonoBehaviour {
    public Material renderMat;
    private World w;

    private ChunkRenderer[,,] chunkRenderers = new ChunkRenderer[World.RenderDist * 2, World.RenderDist * 2, World.RenderDist * 2];

    void Start() {
        World w = new World();

        for(int x = -World.RenderDist; x < World.RenderDist; x++) {
            for(int y = -World.RenderDist; y < World.RenderDist; y++) {
                for(int z = -World.RenderDist; z < World.RenderDist; z++) {
                    GameObject newGo = new GameObject("" + x + "," + y + "," + z);
                    newGo.transform.SetParent(transform);
                    newGo.transform.position = new Vector3(x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize);
                    ChunkRenderer cr = newGo.AddComponent<ChunkRenderer>();
                    cr.chunk = w.GetChunk(x + World.RenderDist, y + World.RenderDist, z + World.RenderDist);
                    chunkRenderers[x + World.RenderDist, y + World.RenderDist, z + World.RenderDist] = cr;
                    cr.GetComponent<MeshRenderer>().material = renderMat;
                    cr.PrerenderChunk();
                }
            }
        }
    }
}

