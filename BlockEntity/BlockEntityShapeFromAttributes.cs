﻿using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.GameContent
{

    public abstract class BlockEntityShapeFromAttributes : BlockEntity
    {
        public string Type;
        protected BlockShapeFromAttributes clutterBlock;
        protected MeshData mesh;
        public float MeshAngleRad { get; internal set; }

        

        public override void Initialize(ICoreAPI api)
        {
            base.Initialize(api);

            clutterBlock = Block as BlockShapeFromAttributes;

            if (Type != null)
            {
                initShape();
            }
        }

        protected virtual void initShape()
        {
            if (Type == null || Api.Side == EnumAppSide.Server) return;

             var cprops = clutterBlock?.GetTypeProps(Type, null, this);

            if (cprops != null)
            {
                mesh = clutterBlock.GenMesh(cprops).Clone().Rotate(new Vec3f(0.5f, 0.5f, 0.5f), 0, MeshAngleRad + cprops.Rotation.Y * GameMath.DEG2RAD, 0).Scale(Vec3f.Zero, 1, 0.98f + GameMath.MurmurHash3Mod(Pos.X, Pos.Y, Pos.Z, 100) / 100f * 0.04f, 1);
            }
        }

        public override void OnBlockPlaced(ItemStack byItemStack = null)
        {
            base.OnBlockPlaced(byItemStack);

            Type = byItemStack?.Attributes.GetString("type");

            initShape();
        }

        public override void FromTreeAttributes(ITreeAttribute tree, IWorldAccessor worldAccessForResolve)
        {
            Type = tree.GetString("type");
            MeshAngleRad = tree.GetFloat("meshAngle");

            if (mesh == null && Api != null && worldAccessForResolve.Side == EnumAppSide.Client)
            {
                initShape();
                MarkDirty(true);
            }

            base.FromTreeAttributes(tree, worldAccessForResolve);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            tree.SetString("type", Type);
            tree.SetFloat("meshAngle", MeshAngleRad);

            base.ToTreeAttributes(tree);
        }

        public override bool OnTesselation(ITerrainMeshPool mesher, ITesselatorAPI tessThreadTesselator)
        {
            mesher.AddMeshData(mesh);
            return true;
        }

    }
}
