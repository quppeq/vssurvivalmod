﻿using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.GameContent
{
    public class BlockLakeIce : BlockForLiquidsLayer
    {
        int waterBlock;

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            waterBlock = api.World.GetBlock(new AssetLocation("water-still-7")).BlockId;
        }

        public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
        {
            bool preventDefault = false;
            foreach (BlockBehavior behavior in BlockBehaviors)
            {
                EnumHandling handled = EnumHandling.PassThrough;

                behavior.OnBlockBroken(world, pos, byPlayer, ref handled);
                if (handled == EnumHandling.PreventDefault) preventDefault = true;
                if (handled == EnumHandling.PreventSubsequent) return;
            }

            if (preventDefault) return;

            if (world.Side == EnumAppSide.Server && (byPlayer == null || byPlayer.WorldData.CurrentGameMode != EnumGameMode.Creative))
            {
                ItemStack[] drops = GetDrops(world, pos, byPlayer, dropQuantityMultiplier);

                if (drops != null)
                {
                    for (int i = 0; i < drops.Length; i++)
                    {
                        if (SplitDropStacks)
                        {
                            for (int k = 0; k < drops[i].StackSize; k++)
                            {
                                ItemStack stack = drops[i].Clone();
                                stack.StackSize = 1;
                                world.SpawnItemEntity(stack, new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5), null);
                            }
                        }

                    }
                }

                world.PlaySoundAt(Sounds?.GetBreakSound(byPlayer), pos.X, pos.Y, pos.Z, byPlayer);
            }

            SpawnBlockBrokenParticles(pos);
            world.BlockAccessor.SetLiquidBlock(waterBlock, pos);
        }

        public override bool ShouldReceiveServerGameTicks(IWorldAccessor world, BlockPos pos, Random offThreadRandom, out object extra)
        {
            extra = null;
            ClimateCondition conds = world.BlockAccessor.GetClimateAt(pos, EnumGetClimateMode.NowValues);

            float chance = GameMath.Clamp((conds.Temperature - 2f) / 20f, 0, 1);
            return offThreadRandom.NextDouble() < chance;
        }

        public override void OnServerGameTick(IWorldAccessor world, BlockPos pos, object extra = null)
        {
            world.BlockAccessor.SetLiquidBlock(waterBlock, pos);
        }

        public override bool ShouldMergeFace(int facingIndex, Block neighbourIce, int intraChunkIndex3d)
        {
            return BlockMaterial == neighbourIce.BlockMaterial;
        }
    }
}
