using UnityEngine;

public class Chunk {
    public const int chunkLogSize = 4;
    public const int chunkSize = 1 << chunkLogSize;
    public const int chunkMask = chunkSize - 1;
    private Block[,,] data = new Block[chunkSize, chunkSize, chunkSize];

    public Chunk() {
        for(int x = 0; x < chunkSize; x++) {
            for(int y = 0; y < chunkSize; y++) {
                for(int z = 0; z < chunkSize; z++) {
                    data[x, y, z] = Block.NewBlock(BlockType.AIR);
                }
            }
        }
    }

    public Block this[int x, int y, int z] {
        get {
            return data[x, y, z];
        }
        set {
            data[x, y, z] = value;
        }
    }
}
