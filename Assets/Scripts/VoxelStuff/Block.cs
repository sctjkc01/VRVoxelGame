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
    public abstract BlockType type {
        get;
    }

    protected static Vector2[] getUvSlot(int x, int y) {
        return new Vector2[] { new Vector2(x * 0.125f, 0.875f - (y * 0.125f)), new Vector2(x * 0.125f, 1f - (y * 0.125f)), new Vector2(0.125f + (x * 0.125f), 1f - (y * 0.125f)), new Vector2(0.125f + (x * 0.125f), 0.875f - (y * 0.125f)) };
    }

    public virtual Vector2[] getUvs(Side side) {
        return new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
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

    public override Vector2[] getUvs(Side side) {
        switch(side) {
            case Side.TOP:
                return getUvSlot(0, 0);
            case Side.BOTTOM:
                return getUvSlot(2, 0);
            default:
                return getUvSlot(1, 0);
        }
    }
}

public class DirtBlock : Block {
    public override BlockType type {
        get {
            return BlockType.DIRT;
        }
    }

    public override Vector2[] getUvs(Side side) {
        return getUvSlot(2, 0);
    }
}

public class StoneBlock : Block {
    public override BlockType type {
        get {
            return BlockType.STONE;
        }
    }

    public override Vector2[] getUvs(Side side) {
        return getUvSlot(3, 0);
    }
}

public class DebugDirectionalBlock : Block {
    public override BlockType type {
        get {
            return BlockType.DEBUGDIRECTIONAL;
        }
    }

    public override Vector2[] getUvs(Side side) {
        switch(side) {
            default:
                return getUvSlot(3, 0);
            case Side.NORTH:
                return getUvSlot(0, 1);
            case Side.SOUTH:
                return getUvSlot(1, 1);
            case Side.EAST:
                return getUvSlot(2, 1);
            case Side.WEST:
                return getUvSlot(3, 1);
        }
    }
}