using UnityEngine;
using System.Collections.Generic;
using System;

public enum BlockType {
    AIR, GRASS, DIRT, STONE, DEBUGDIRECTIONAL, NULL
}
public enum Side {
    TOP, NORTH, EAST, WEST, SOUTH, BOTTOM
}

public abstract class Block {
    public const int SpriteCount = 8;

    public abstract BlockType type {
        get;
    }

    public virtual Vector2 getUv(Side side) {
        return new Vector2(0, 7);
    }

    public static Block NewBlock(BlockType type) {
        switch(type) {
            case BlockType.NULL:
            default:
                return new NullBlock();
            case BlockType.AIR:
                return new AirBlock();
            case BlockType.DIRT:
                return new DirtBlock();
            case BlockType.GRASS:
                return new GrassBlock();
            case BlockType.STONE:
                return new StoneBlock();
            case BlockType.DEBUGDIRECTIONAL:
                return new DebugDirectionalBlock();
        }
    }

    public static Block NewBlock(int type) {
        switch(type) {
            case -1:
            default:
                return new NullBlock();
            case 0:
                return new AirBlock();
            case 1:
                return new DirtBlock();
            case 2:
                return new GrassBlock();
            case 3:
                return new StoneBlock();
            case 255:
                return new DebugDirectionalBlock();
        }
    }
}

public class NullBlock : Block {
    public override BlockType type {
        get {
            return BlockType.NULL;
        }
    }
}

public class AirBlock : Block {
    public override BlockType type {
        get {
            return BlockType.AIR;
        }
    }
}

public class GrassBlock : Block {
    public override BlockType type {
        get {
            return BlockType.GRASS;
        }
    }

    public override Vector2 getUv(Side side) {
        switch(side) {
            case Side.TOP:
                return new Vector2(0, 7);
            case Side.BOTTOM:
                return new Vector2(2, 7);
            default:
                return new Vector2(1, 7);
        }
    }
}

public class DirtBlock : Block {
    public override BlockType type {
        get {
            return BlockType.DIRT;
        }
    }

    public override Vector2 getUv(Side side) {
        return new Vector2(2, 7);
    }
}

public class StoneBlock : Block {
    public override BlockType type {
        get {
            return BlockType.STONE;
        }
    }

    public override Vector2 getUv(Side side) {
        return new Vector2(3, 7);
    }
}

public class DebugDirectionalBlock : Block {
    public override BlockType type {
        get {
            return BlockType.DEBUGDIRECTIONAL;
        }
    }

    public override Vector2 getUv(Side side) {
        switch(side) {
            default:
                return new Vector2(3, 7);
            case Side.NORTH:
                return new Vector2(0, 1);
            case Side.SOUTH:
                return new Vector2(1, 1);
            case Side.EAST:
                return new Vector2(2, 1);
            case Side.WEST:
                return new Vector2(3, 1);
        }
    }
}