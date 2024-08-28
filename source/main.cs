
using Godot;
using System;


public partial class main : Node3D
{
    [Export]
    public Texture2D[] VoxelTextures;
    private Label l;

    private const int VoxelSize = 1;
    private const int GridSize = 10;
    private byte[,,] voxels = new byte[10, 10, 10];


    public override void _Ready()
    {
        l = GetNode<Label>("Label");

        GenerateVoxels();
    }

    public override void _Process(double delta)
    {
        l.Text = "FPS " + Engine.GetFramesPerSecond().ToString();
    }


    private void GenerateVoxels()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                voxels[i, 0, j] = 4;
            }
        }


        voxels[1, 1, 3] = 1;
        voxels[2, 1, 3] = 1;
        voxels[4, 1, 3] = 2;
        voxels[4, 2, 3] = 3;
        voxels[3, 1, 5] = 5;

        for (int i = 0; i < VoxelTextures.Length; i++)
        {
            var arrayMesh = new ArrayMesh();
            var surfaceTool = new SurfaceTool();
            surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

            for (int j = 0; j < GridSize; j++)
            {
                for (int k = 0; k < GridSize; k++)
                {
                    for (int l = 0; l < GridSize; l++)
                    {
                        if (voxels[j, k, l] == i + 1)
                        {
                            AddVoxel(surfaceTool, new Vector3(j, k, l), i);
                        }
                    }
                }
            }
            
            surfaceTool.Index();
            arrayMesh = surfaceTool.Commit();
            GD.Print("Texture:", i.ToString(), ", IndexLen:", arrayMesh.SurfaceGetArrayIndexLen(0).ToString(), "\n");

            var meshInstance = new MeshInstance3D();
            meshInstance.Mesh = arrayMesh;

            AddChild(meshInstance);
        }
    }

    private void AddVoxel(SurfaceTool surfaceTool, Vector3 position, int tex)
    {
        var material = new StandardMaterial3D();
        material.AlbedoTexture = VoxelTextures[tex];
        surfaceTool.SetMaterial(material);

        if (!IsVoxelAt(position + Vector3.Left))
        {
            AddFace(surfaceTool, position, VoxelFaces.Left);
        }

        if (!IsVoxelAt(position + Vector3.Right))
        {
            AddFace(surfaceTool, position, VoxelFaces.Right);
        }

        if (!IsVoxelAt(position + Vector3.Up))
        {
            AddFace(surfaceTool, position, VoxelFaces.Top);
        }

        if (!IsVoxelAt(position + Vector3.Down))
        {
            AddFace(surfaceTool, position, VoxelFaces.Bottom);
        }

        if (!IsVoxelAt(position + Vector3.Forward))
        {
            AddFace(surfaceTool, position, VoxelFaces.Back);
        }

        if (!IsVoxelAt(position + Vector3.Back))
        {
            AddFace(surfaceTool, position, VoxelFaces.Front);
        }

    }
    private bool IsVoxelAt(Vector3 position)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        int z = (int)position.Z;

        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize || z < 0 || z >= GridSize)
        {
            return false;
        }

        bool result = voxels[x, y, z] != 0;
        return result;
    }


    private void AddFace(SurfaceTool surfaceTool, Vector3 position, VoxelFaces face)
    {
        Vector3[] vertices = new Vector3[4];
        Vector3 normal = Vector3.Zero;
        Vector2[] uvs = new Vector2[4];

        switch (face)
        {
            case VoxelFaces.Left:
                vertices[0] = new Vector3(0, 0, 0);
                vertices[1] = new Vector3(0, 1, 0);
                vertices[2] = new Vector3(0, 1, 1);
                vertices[3] = new Vector3(0, 0, 1);
                normal = Vector3.Left;
                break;
            case VoxelFaces.Right:
                vertices[0] = new Vector3(1, 0, 1);
                vertices[1] = new Vector3(1, 1, 1);
                vertices[2] = new Vector3(1, 1, 0);
                vertices[3] = new Vector3(1, 0, 0);
                normal = Vector3.Right;
                break;
            case VoxelFaces.Top:
                vertices[0] = new Vector3(0, 1, 1);
                vertices[1] = new Vector3(0, 1, 0);
                vertices[2] = new Vector3(1, 1, 0);
                vertices[3] = new Vector3(1, 1, 1);
                normal = Vector3.Up;
                break;
            case VoxelFaces.Bottom:
                vertices[0] = new Vector3(0, 0, 0);
                vertices[1] = new Vector3(0, 0, 1);
                vertices[2] = new Vector3(1, 0, 1);
                vertices[3] = new Vector3(1, 0, 0);
                normal = Vector3.Down;
                break;
            case VoxelFaces.Back:
                vertices[0] = new Vector3(1, 0, 0);
                vertices[1] = new Vector3(1, 1, 0);
                vertices[2] = new Vector3(0, 1, 0);
                vertices[3] = new Vector3(0, 0, 0);
                normal = Vector3.Back;
                break;
            case VoxelFaces.Front:
                vertices[0] = new Vector3(0, 0, 1);
                vertices[1] = new Vector3(0, 1, 1);
                vertices[2] = new Vector3(1, 1, 1);
                vertices[3] = new Vector3(1, 0, 1);
                normal = Vector3.Forward;
                break;
        }

  
        uvs[0] = new Vector2(0, 1);
        uvs[1] = new Vector2(0, 0);
        uvs[2] = new Vector2(1, 0);
        uvs[3] = new Vector2(1, 1);


        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[0]);
        surfaceTool.AddVertex(vertices[0] * VoxelSize + position);

        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[1]);
        surfaceTool.AddVertex(vertices[1] * VoxelSize + position);

        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[2]);
        surfaceTool.AddVertex(vertices[2] * VoxelSize + position);



        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[0]);
        surfaceTool.AddVertex(vertices[0] * VoxelSize + position);

        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[2]);
        surfaceTool.AddVertex(vertices[2] * VoxelSize + position);

        surfaceTool.SetNormal(normal);
        surfaceTool.SetUV(uvs[3]);
        surfaceTool.AddVertex(vertices[3] * VoxelSize + position);
    }
    private enum VoxelFaces
    {
        Left,
        Right,
        Top,
        Bottom,
        Back,
        Front
    }

}

