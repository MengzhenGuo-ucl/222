using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VoxelGrid
{
    #region Public fields

    public Vector3Int GridSize;
    public Voxel[,,] Voxels;
    public Corner[,,] Corners;
    public Face[][,,] Faces = new Face[3][,,];
    public Edge[][,,] Edges = new Edge[3][,,];
    public Vector3 Origin;
    public Vector3 Corner;
    public float VoxelSize { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for a basic <see cref="VoxelGrid"/>
    /// Adds a game object containing a collider to each of first layer voxels
    /// </summary>
    /// <param name="size">Size of the grid</param>
    /// <param name="origin">Origin of the grid</param>
    /// <param name="voxelSize">The size of each <see cref="Voxel"/></param>
    public VoxelGrid(Vector3Int size, Vector3 origin, float voxelSize, Transform parent = null)
    {
        GridSize = size;
        Origin = origin;
        VoxelSize = voxelSize;

        Voxels = new Voxel[GridSize.x, GridSize.y, GridSize.z];

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                for (int z = 0; z < GridSize.z; z++)
                {
                    if (y == 0)
                    {
                        Voxels[x, y, z] = new Voxel(
                            new Vector3Int(x, y, z),
                            this,
                            createCollider: true,
                            parent: parent);
                    }
                    else
                    {
                        Voxels[x, y, z] = new Voxel(
                            new Vector3Int(x, y, z),
                            this);
                    }

                }
            }
        }

        MakeFaces();
        MakeCorners();
        MakeEdges();
    }

    /// <summary>
    /// Create voxel grid from the size of input image
    /// </summary>
    /// <param name="input"></input image>
    /// <param name="origin"></origin>
    /// <param name="voxelSize"></size>
    /// <param name="height"></param>
    /// <param name="parent"></param>
    /// 
    public VoxelGrid(Texture2D input, Vector3 origin, int height, float voxelSize, Transform parent = null)
    {

        // create new grid with image size
        GridSize = new Vector3Int(input.width, height, input.height);

        Origin = origin;
        VoxelSize = voxelSize;

        Voxels = new Voxel[GridSize.x, GridSize.y, GridSize.z];

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                for (int z = 0; z < GridSize.z; z++)
                {
                    if (y == 0)
                    {
                        Voxels[x, y, z] = new GraphVoxel(
                            new Vector3Int(x, y, z),
                            this,
                            1f,
                            createCollider: true,
                            parent: parent);
                    }
                    else
                    {
                        Voxels[x, y, z] = new GraphVoxel(
                            new Vector3Int(x, y, z),
                            this,
                            1f);
                    }
                }
            }
        }


        MakeFaces();
        MakeCorners();
        MakeEdges();
    }

    #endregion

    #region Grid elements constructors

    /// <summary>
    /// Creates the Faces of each <see cref="Voxel"/>
    /// </summary>
    private void MakeFaces()
    {
        // make faces
        Faces[0] = new Face[GridSize.x + 1, GridSize.y, GridSize.z];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Faces[0][x, y, z] = new Face(x, y, z, Axis.X, this);
                }

        Faces[1] = new Face[GridSize.x, GridSize.y + 1, GridSize.z];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Faces[1][x, y, z] = new Face(x, y, z, Axis.Y, this);
                }

        Faces[2] = new Face[GridSize.x, GridSize.y, GridSize.z + 1];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Faces[2][x, y, z] = new Face(x, y, z, Axis.Z, this);
                }
    }

    /// <summary>
    /// Creates the Corners of each Voxel
    /// </summary>
    private void MakeCorners()
    {
        Corner = new Vector3(Origin.x - VoxelSize / 2, Origin.y - VoxelSize / 2, Origin.z - VoxelSize / 2);

        Corners = new Corner[GridSize.x + 1, GridSize.y + 1, GridSize.z + 1];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Corners[x, y, z] = new Corner(new Vector3Int(x, y, z), this);
                }
    }

    /// <summary>
    /// Creates the Edges of each Voxel
    /// </summary>
    private void MakeEdges()
    {
        Edges[2] = new Edge[GridSize.x + 1, GridSize.y + 1, GridSize.z];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    Edges[2][x, y, z] = new Edge(x, y, z, Axis.Z, this);
                }

        Edges[0] = new Edge[GridSize.x, GridSize.y + 1, GridSize.z + 1];

        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Edges[0][x, y, z] = new Edge(x, y, z, Axis.X, this);
                }

        Edges[1] = new Edge[GridSize.x + 1, GridSize.y, GridSize.z + 1];

        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    Edges[1][x, y, z] = new Edge(x, y, z, Axis.Y, this);
                }
    }

    #endregion

    #region Grid operations


    /// <summary>
    /// Get the Faces of the <see cref="VoxelGrid"/>
    /// </summary>
    /// <returns>All the faces</returns>
    public IEnumerable<Face> GetFaces()
    {
        for (int n = 0; n < 3; n++)
        {
            int xSize = Faces[n].GetLength(0);
            int ySize = Faces[n].GetLength(1);
            int zSize = Faces[n].GetLength(2);

            for (int x = 0; x < xSize; x++)
                for (int y = 0; y < ySize; y++)
                    for (int z = 0; z < zSize; z++)
                    {
                        yield return Faces[n][x, y, z];
                    }
        }
    }

    /// <summary>
    /// Get the Voxels of the <see cref="VoxelGrid"/>
    /// </summary>
    /// <returns>All the Voxels</returns>
    public IEnumerable<Voxel> GetVoxels()
    {
        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y; y++)
                for (int z = 0; z < GridSize.z; z++)
                {
                    yield return Voxels[x, y, z];
                }
    }

    /// <summary>
    /// Get the Corners of the <see cref="VoxelGrid"/>
    /// </summary>
    /// <returns>All the Corners</returns>
    public IEnumerable<Corner> GetCorners()
    {
        for (int x = 0; x < GridSize.x + 1; x++)
            for (int y = 0; y < GridSize.y + 1; y++)
                for (int z = 0; z < GridSize.z + 1; z++)
                {
                    yield return Corners[x, y, z];
                }
    }

    /// <summary>
    /// Get the Edges of the <see cref="VoxelGrid"/>
    /// </summary>
    /// <returns>All the edges</returns>
    public IEnumerable<Edge> GetEdges()
    {
        for (int n = 0; n < 3; n++)
        {
            int xSize = Edges[n].GetLength(0);
            int ySize = Edges[n].GetLength(1);
            int zSize = Edges[n].GetLength(2);

            for (int x = 0; x < xSize; x++)
                for (int y = 0; y < ySize; y++)
                    for (int z = 0; z < zSize; z++)
                    {
                        yield return Edges[n][x, y, z];
                    }
        }
    }

    #endregion

    #region Public Methods


    /// <summary>
    /// Tries to create a black blob from the
    /// specified origin and with the specified size
    /// </summary>
    /// <param name="origin">The index of the origin</param>
    /// <param name="radius">The radius of the blob in voxels</param>
    /// <param name="picky">If the blob should skip voxels randomly as it expands</param>
    /// <param name="flat">If the blob should be located on the first layer or use all</param>
    /// <returns></returns>

    public bool GrowPlot(Vector3Int origin, int radius, int height = 0)
    {

        //List<Vector3> growingVoxel
        //A list to store the growing voxel
        List<Voxel> growingVoxel = new List<Voxel>();

        //Give them white coluor as plot
        FunctionColor plotcolor = FunctionColor.White;

        //check if the origin is valid and add it to voxel list
        if (Util.ValidateIndex(GridSize, origin))
        {
            growingVoxel.Add(Voxels[origin.x, height, origin.z]);
        }
        else return false;

        //Iterate through the neighboring layer within the radius
        for (int i = 0; i < radius; i++)
        {

            List<Voxel> availableVoxels = new List<Voxel>();

            foreach (var voxel in growingVoxel)
            {
                //Get neighbors in 2D or 3D

                Voxel[] neighbors;


                if (height == 0)
                {
                    neighbors = voxel.GetFaceNeighboursXZ().ToArray();

                }
                else
                {
                    neighbors = voxel.GetFaceNeighbours().ToArray();
                }

                //Iterate each neighbors + and check if is available
                foreach (var neighbour in neighbors)
                {
                    //check if is the available plot voxel

                    //+ if color is blue(backyard area that allows to grow)
                    if (neighbour.FColor == FunctionColor.Blue && neighbour.IsActive && Util.ValidateIndex(GridSize, neighbour.Index) && !growingVoxel.Contains(neighbour) && !availableVoxels.Contains(neighbour))
                    {
                        availableVoxels.Add(neighbour);
                    }
                }

            }

            if (availableVoxels.Count == 0) break;

            //add these available voxels to growing voxels list
            foreach (var availableVoxel in availableVoxels)
            {
                if (availableVoxel.FColor == FunctionColor.Blue)
                {
                    growingVoxel.Add(availableVoxel);
                }

            }
        }

        // set the plot color and quality
        foreach (var voxel in growingVoxel)
        {
            if (voxel.FColor == FunctionColor.Blue)
            {
                voxel.FColor = plotcolor;
                voxel.Qname = ColorQuality.Plot;
            }

        }

        return true;
    }

    // A method to store the possible voxel after xxx evaluation result  + animate one by one


    // A method to check voxel with xxx evaluation result  + animate one by one
    public bool AvailablePlotVoxel()
    {

        return true;
    }


    /// <summary>
    /// Reads an image pixel data and set the color pixels and corresponding label/quality to the grid
    /// </summary>
    /// <param name="image">The reference image</param>
    /// <param name="layer">The target layer</param>
    public void SetStatesFromImage(Texture2D inputImage, int layer = 0)
    {
        // Iterate through the XZ plane
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int z = 0; z < GridSize.z; z++)
            {
                // Get the pixel color from the image
                Color pixel = inputImage.GetPixel(x, z);

                //read voxel as its child:graphvoxel

                GraphVoxel voxel = (GraphVoxel)Voxels[x, 0, z];
                //read RGB channel
                float[] colorScores = new float[8]
                {
               Mathf.Abs(pixel.r - 0) + Mathf.Abs(pixel.g - 1) + Mathf.Abs(pixel.b - 1),//Cyan
                Mathf.Abs(pixel.r - 0) + Mathf.Abs(pixel.g - 0) + Mathf.Abs(pixel.b - 1),//Blue
                Mathf.Abs(pixel.r - 1) + Mathf.Abs(pixel.g - 0) + Mathf.Abs(pixel.b - 1),//Magenta 
                Mathf.Abs(pixel.r - 1) + Mathf.Abs(pixel.g - 0) + Mathf.Abs(pixel.b - 0),//Red
                Mathf.Abs(pixel.r - 0) + Mathf.Abs(pixel.g - 1) + Mathf.Abs(pixel.b - 0),//Green
                Mathf.Abs(pixel.r - 1) + Mathf.Abs(pixel.g - 1) + Mathf.Abs(pixel.b - 0),//Yellow
                Mathf.Abs(pixel.r - 1) + Mathf.Abs(pixel.g - 1) + Mathf.Abs(pixel.b - 1),//White
                Mathf.Abs(pixel.r - .5f) + Mathf.Abs(pixel.g - .5f) + Mathf.Abs(pixel.b - .5f)//Gray
              
                };
                //Get the colors right


                float biggestScore = 0;
                int colorIndex = 0;
                int colorQuality = 0;

                for (int i = 0; i < colorScores.Length; i++)
                {
                    if (colorScores[i] > biggestScore)
                    {
                        biggestScore = colorScores[i];
                        colorIndex = i;
                        colorQuality = i;
                    }
                }

                Voxels[x, layer, z].FColor = (FunctionColor)colorIndex;
                Voxels[x, layer, z].Qname = (ColorQuality)colorQuality;


                // assign the layermask to be used in analysis
                foreach (var voxels in Voxels)
                {
                    if (voxel.IsActive && voxel.VoxelCollider.gameObject.layer == 0)
                    {

                        if (voxel.FColor == FunctionColor.Blue) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("Backyard");
                        else if (voxel.FColor == FunctionColor.Red) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("House");
                        else if (voxel.FColor == FunctionColor.Yellow) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("Street");
                        else if (voxel.FColor == FunctionColor.Magenta) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("Frontyard");
                        else if (voxel.FColor == FunctionColor.Green) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("Tree");
                        else if (voxel.FColor == FunctionColor.White) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("Plot");
                        else if (voxel.FColor == FunctionColor.Cyan) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("LandTexture");
                        else if (voxel.FColor == FunctionColor.Gray) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("SmallBuilding");
                        else if (voxel.FColor == FunctionColor.Empty) voxel.VoxelCollider.gameObject.layer = LayerMask.NameToLayer("EmptyLand");

                    }

                }



            }
        }

    }


    public void ClearGrid()
    {
        foreach (var voxel in Voxels)
        {
            voxel.FColor = FunctionColor.Empty;
        }
    }


    // Generate image from Grid, voxel to pixel, read the plot on the top layer(ovverlapping) FROM VOXEL DATA TO IMAGE
    public Texture2D ImageFromGrid(int layer = 0, bool overlapping = false)
    {
        TextureFormat textureFormat;
        //RGB png 8 depth
        textureFormat = TextureFormat.RGB24;

        //new image
        Texture2D gridImage = new Texture2D(GridSize.x, GridSize.z, textureFormat, true);

        //iterare through all voxles, get their color and write to grid image
        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.z; j++)
            {
                //get all voxel from x,z
                var voxel = Voxels[i, 0, j];

                // assign color based on function color
                Color co;
                if (voxel.FColor == FunctionColor.White) co = Color.white;
                else if (voxel.FColor == FunctionColor.Red) co = Color.red;
               
                else if (voxel.FColor == FunctionColor.Blue) co = Color.blue;
                else if (voxel.FColor == FunctionColor.Yellow) co = Color.yellow;
                else if (voxel.FColor == FunctionColor.Green) co = Color.green;
                else if (voxel.FColor == FunctionColor.Cyan) co = Color.cyan;
                else if (voxel.FColor == FunctionColor.Magenta) co = Color.magenta;
                else if (voxel.FColor == FunctionColor.Gray) co = Color.gray;
                else co = new Color(1.0f, 0.64f, 0f); //orange  rgb value/max255

                gridImage.SetPixel(i, j, co);

            }
        }

        //from memory value to actual pixel
        gridImage.Apply();
        return gridImage;
    }

    #endregion
}


/// <summary>
/// Color coded values
/// </summary>
public enum FunctionColor
{
    Empty = -1,
    Red = 0,
    Yellow = 1,
    Green = 2,
    Cyan = 3,
    Magenta = 4,
    Blue = 5,
    White = 6,
    Gray = 7

}

public enum ColorQuality
{
    Plot = 0,
    House = 1,
    Street = 2,
    Backyard = 3,
    Frontyard = 4,
    SmallBuilding = 5,
    Tree = 6,
    EmptyLand = 7,
    LandTexture = 8

}