using System;

public class World {
    // how far we can see
    public const int RenderDist = 5;

    // the chunks around the player. The player should be at chunk
    // VisibleX,VisibleY,VisibleZ
    private Chunk[,,] chunks = new Chunk[RenderDist * 2, RenderDist * 2, RenderDist * 2];

    // origin of world in chunk coordinates, meaning (0,0,0) is the first chunk (1,0,0) is the chunk next to it
    // as the player walks, the chunks in the above array are shifted, and these coordinates
    // are adjusted to keep the player at the center of the world
    private int c0x, c0y, c0z;

    public World() {
        for(int x = 0; x < RenderDist * 2; x++) {
            for(int y = 0; y < RenderDist * 2; y++) {
                for(int z = 0; z < RenderDist * 2; z++) {
                    chunks[x, y, z] = new Chunk(this, x, y, z);
                }
            }
        }


    }

    public Chunk GetChunk(int x, int y, int z) {
        return chunks[x, y, z];
    }


    // get the block at position (wx,wy,wz) where these are the world coordinates (not adjusted for shifts or anything)
    public Block this[int wx, int wy, int wz] {
        get {
            // first calculate which chunk we are talking about:
            int cx = (wx >> Chunk.chunkLogSize) - c0x;
            int cy = (wy >> Chunk.chunkLogSize) - c0y;
            int cz = (wz >> Chunk.chunkLogSize) - c0z;

            // request can be out of range, then return a special
            // Unknown block type
            if(cx < 0 || cx > chunks.GetLength(0))
                return new NullBlock();
            if(cy < 0 || cy > chunks.GetLength(1))
                return new NullBlock();
            if(cz < 0 || cz > chunks.GetLength(2))
                return new NullBlock();
            Chunk chunk = chunks[cx, cy, cz];

            // this figures out the coordinate of the block relative to
            // chunk origin.
            int lx = wx & Chunk.chunkMask;
            int ly = wy & Chunk.chunkMask;
            int lz = wz & Chunk.chunkMask;

            return chunk[lx, ly, lz];
        }
        set {
            // first calculate which chunk we are talking about:
            int cx = (wx >> Chunk.chunkLogSize) - c0x;
            int cy = (wy >> Chunk.chunkLogSize) - c0y;
            int cz = (wz >> Chunk.chunkLogSize) - c0z;

            // cannot modify chunks that are not within the visible area
            if(cx < 0 || cx > chunks.GetLength(0))
                throw new Exception("Cannot modify world outside visible area");
            if(cy < 0 || cy > chunks.GetLength(1))
                throw new Exception("Cannot modify world outside visible area");
            if(cz < 0 || cz > chunks.GetLength(2))
                throw new Exception("Cannot modify world outside visible area");
            Chunk chunk = chunks[cx, cy, cz];

            // this figures out the coordinate of the block relative to
            // chunk origin.
            int lx = wx & Chunk.chunkMask;
            int ly = wy & Chunk.chunkMask;
            int lz = wz & Chunk.chunkMask;

            chunk[lx, ly, lz] = value;
        }
    }

}
