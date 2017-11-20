using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace  SecondMonitor.PCarsConnector.enums
{
    public enum ETerrain
    {
        [Description("TERRAIN_ROAD")]
        TerrainRoad = 0,
        [Description("TERRAIN_LOW_GRIP_ROAD")]
        TerrainLowGripRoad,
        [Description("TERRAIN_BUMPY_ROAD1")]
        TerrainBumpyRoad1,
        [Description("TERRAIN_BUMPY_ROAD2")]
        TerrainBumpyRoad2,
        [Description("TERRAIN_BUMPY_ROAD3")]
        TerrainBumpyRoad3,
        [Description("TERRAIN_MARBLES")]
        TerrainMarbles,
        [Description("TERRAIN_GRASSY_BERMS")]
        TerrainGrassyBerms,
        [Description("TERRAIN_GRASS")]
        TerrainGrass,
        [Description("TERRAIN_GRAVEL")]
        TerrainGravel,
        [Description("TERRAIN_BUMPY_GRAVEL")]
        TerrainBumpyGravel,
        [Description("TERRAIN_RUMBLE_STRIPS")]
        TerrainRumbleStrips,
        [Description("TERRAIN_DRAINS")]
        TerrainDrains,
        [Description("TERRAIN_TYREWALLS")]
        TerrainTyrewalls,
        [Description("TERRAIN_CEMENTWALLS")]
        TerrainCementwalls,
        [Description("TERRAIN_GUARDRAILS")]
        TerrainGuardrails,
        [Description("TERRAIN_SAND")]
        TerrainSand,
        [Description("TERRAIN_BUMPY_SAND")]
        TerrainBumpySand,
        [Description("TERRAIN_DIRT")]
        TerrainDirt,
        [Description("TERRAIN_BUMPY_DIRT")]
        TerrainBumpyDirt,
        [Description("TERRAIN_DIRT_ROAD")]
        TerrainDirtRoad,
        [Description("TERRAIN_BUMPY_DIRT_ROAD")]
        TerrainBumpyDirtRoad,
        [Description("TERRAIN_PAVEMENT")]
        TerrainPavement,
        [Description("TERRAIN_DIRT_BANK")]
        TerrainDirtBank,
        [Description("TERRAIN_WOOD")]
        TerrainWood,
        [Description("TERRAIN_DRY_VERGE")]
        TerrainDryVerge,
        [Description("TERRAIN_EXIT_RUMBLE_STRIPS")]
        TerrainExitRumbleStrips,
        [Description("TERRAIN_GRASSCRETE")]
        TerrainGrasscrete,
        [Description("TERRAIN_LONG_GRASS")]
        TerrainLongGrass,
        [Description("TERRAIN_SLOPE_GRASS")]
        TerrainSlopeGrass,
        [Description("TERRAIN_COBBLES")]
        TerrainCobbles,
        [Description("TERRAIN_SAND_ROAD")]
        TerrainSandRoad,
        [Description("TERRAIN_BAKED_CLAY")]
        TerrainBakedClay,
        [Description("TERRAIN_ASTROTURF")]
        TerrainAstroturf,
        [Description("TERRAIN_SNOWHALF")]
        TerrainSnowhalf,
        [Description("TERRAIN_SNOWFULL")]
        TerrainSnowfull,
        //-------------
        [Description("TERRAIN_MAX")]
        TerrainMax
    }
}
