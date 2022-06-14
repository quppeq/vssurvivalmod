﻿using Cairo;
using System;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace VSSurvivalMod.Systems.ChiselModes
{
    public class TwoByChiselMode : ChiselMode
    {
        public override int ChiselSize => 2;

        public override DrawSkillIconDelegate DrawAction(ICoreClientAPI capi) => ItemClay.Drawcreate4_svg;
    }
}
