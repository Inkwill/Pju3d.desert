using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;


public sealed class TerrainTool : MonoBehaviour
{
	public enum TerrainModificationAction
	{
		Raise,
		Lower,
		Flatten,
		Sample,
		SampleAverage,
		PaintTexture,
	}

	public int brushWidth = 1;
	public int brushHeight = 1;

	public Camera camera_;
	[Range(0.001f, 0.1f)]
	public float strength;

	public TerrainModificationAction modificationAction = TerrainModificationAction.PaintTexture;

	public Terrain _targetTerrain;
	private TerrainData _initTerrainData;

	private float _sampledHeight;

	private void Start()
	{
		_initTerrainData = new TerrainData();
		DuplicateTerrainData(GetTerrainData(), _initTerrainData);
	}

	private void Undo()
	{
		DuplicateTerrainData(_initTerrainData, GetTerrainData());
	}
	private void Update()
	{
		// if (Input.GetMouseButton(0))
		// {
		// 	if (Physics.Raycast(camera_.ScreenPointToRay(Input.mousePosition), out var hit))
		// 	{
		// 		if (hit.transform.TryGetComponent(out Terrain terrain)) _targetTerrain = terrain;

		// 		switch (modificationAction)
		// 		{
		// 			case TerrainModificationAction.Raise:

		// 				RaiseTerrain(hit.point, strength, brushWidth, brushHeight);

		// 				break;

		// 			case TerrainModificationAction.Lower:

		// 				LowerTerrain(hit.point, strength, brushWidth, brushHeight);

		// 				break;

		// 			case TerrainModificationAction.Flatten:

		// 				FlattenTerrain(hit.point, _sampledHeight, brushWidth, brushHeight);

		// 				break;

		// 			case TerrainModificationAction.Sample:

		// 				_sampledHeight = SampleHeight(hit.point);

		// 				break;

		// 			case TerrainModificationAction.SampleAverage:

		// 				_sampledHeight = SampleAverageHeight(hit.point, brushWidth, brushHeight);

		// 				break;
		// 			case TerrainModificationAction.PaintTexture:

		// 				UpdateTerrainTexture(hit.point, 1, brushWidth, brushHeight);

		// 				break;
		// 		}
		// 	}
		// }
		// if (Input.GetMouseButton(1))
		// {
		// 	Debug.Log("Undo!");
		// 	Undo();
		// }
		if (Input.GetKeyDown("space"))
		{
			ChangeTerrain(TerrainModificationAction.Lower, transform.position);

		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			LocalNavMeshBuilder builder = GetComponent<LocalNavMeshBuilder>();

			GameObject obj = Instantiate(Resources.Load("building_hotel"), builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
			//GameObject obj = Instantiate(prefab, builder.GetNavMeshRandomPos(gameObject), Quaternion.Euler(0, 180, 0)) as GameObject;
			Debug.Log("build at :" + obj.transform.position);
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Undo();
		}
	}

	public void ChangeTerrain(TerrainModificationAction actionType, Vector3 worldPosition)
	{
		switch (actionType)
		{
			case TerrainModificationAction.Raise:

				RaiseTerrain(worldPosition, strength, brushWidth, brushHeight);

				break;

			case TerrainModificationAction.Lower:

				LowerTerrain(worldPosition, strength, brushWidth, brushHeight);

				break;

			case TerrainModificationAction.Flatten:

				FlattenTerrain(worldPosition, _sampledHeight, brushWidth, brushHeight);

				break;

			case TerrainModificationAction.Sample:

				_sampledHeight = SampleHeight(worldPosition);

				break;

			case TerrainModificationAction.SampleAverage:

				_sampledHeight = SampleAverageHeight(worldPosition, brushWidth, brushHeight);

				break;
			case TerrainModificationAction.PaintTexture:

				UpdateTerrainTexture(worldPosition, 1, brushWidth, brushHeight);

				break;
		}

	}

	private TerrainData GetTerrainData() => _targetTerrain.terrainData;

	private int GetHeightmapResolution() => GetTerrainData().heightmapResolution;

	private Vector3 GetTerrainSize() => GetTerrainData().size;

	private void DuplicateTerrainData(TerrainData from, TerrainData to)
	{
		to.heightmapResolution = from.heightmapResolution;
		to.size = from.size;
		to.wavingGrassAmount = from.wavingGrassAmount;
		to.wavingGrassSpeed = from.wavingGrassSpeed;
		to.wavingGrassStrength = from.wavingGrassStrength;
		to.wavingGrassTint = from.wavingGrassTint;
		to.treeInstances = from.treeInstances;
		to.alphamapResolution = from.alphamapResolution;
		to.baseMapResolution = from.baseMapResolution;

		// copy details
		to.terrainLayers = from.terrainLayers;
		to.detailPrototypes = from.detailPrototypes;
		to.treePrototypes = from.treePrototypes;
		to.RefreshPrototypes();

		float[,] heights = from.GetHeights(0, 0, from.heightmapResolution,
			from.heightmapResolution);
		to.SetHeights(0, 0, heights);

		float[,,] alphaMap = from.GetAlphamaps(0, 0, from.alphamapWidth, from.alphamapHeight);
		// int texturesCount = from.alphamapLayers;
		// Debug.Log("from.alphamapHeight = " + from.alphamapHeight);
		// Debug.Log("from.alphamapWidth = " + from.alphamapWidth);
		// Debug.Log("texturesCount = " + texturesCount);

		// for (int y = 0; y < from.alphamapHeight; y++)
		// {
		// 	for (int x = 0; x < from.alphamapWidth; x++)
		// 	{
		// 		for (var i = 0; i < texturesCount; i++)
		// 		{
		// 			// If the current texture index matches the specified texture index, set its alpha value to 1
		// 			// Otherwise, set its alpha value to 0
		// 			alphaMap[y, x, i] = (i == 1) ? 1.0f : 0.0f;
		// 		}
		// 	}
		// }
		// Debug.Log("alphaMap = " + alphaMap.GetLength(0));
		to.SetAlphamaps(0, 0, alphaMap);
	}

	public Vector3 WorldToTerrainPosition(Vector3 worldPosition)
	{
		var terrainPosition = worldPosition - _targetTerrain.GetPosition();

		var terrainSize = GetTerrainSize();

		var heightmapResolution = GetHeightmapResolution();

		terrainPosition = new Vector3(terrainPosition.x / terrainSize.x, terrainPosition.y / terrainSize.y, terrainPosition.z / terrainSize.z);

		return new Vector3(terrainPosition.x * heightmapResolution, 0, terrainPosition.z * heightmapResolution);
	}

	public Vector2Int GetBrushPosition(Vector3 worldPosition, int brushWidth, int brushHeight)
	{
		var terrainPosition = WorldToTerrainPosition(worldPosition);

		var heightmapResolution = GetHeightmapResolution();

		return new Vector2Int((int)Mathf.Clamp(terrainPosition.x - brushWidth / 2.0f, 0.0f, heightmapResolution), (int)Mathf.Clamp(terrainPosition.z - brushHeight / 2.0f, 0.0f, heightmapResolution));
	}

	public Vector2Int GetSafeBrushSize(int brushX, int brushY, int brushWidth, int brushHeight)
	{
		var heightmapResolution = GetHeightmapResolution();

		while (heightmapResolution - (brushX + brushWidth) < 0) brushWidth--;

		while (heightmapResolution - (brushY + brushHeight) < 0) brushHeight--;

		return new Vector2Int(brushWidth, brushHeight);
	}

	public void RaiseTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
	{
		var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);

		var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

		var terrainData = GetTerrainData();

		var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

		for (var y = 0; y < brushSize.y; y++)
		{
			for (var x = 0; x < brushSize.x; x++)
			{
				heights[y, x] += strength * Time.deltaTime;
			}
		}

		terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
	}

	public void LowerTerrain(Vector3 worldPosition, float strength, int brushWidth, int brushHeight)
	{
		var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);

		var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

		var terrainData = GetTerrainData();

		var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

		for (var y = 0; y < brushSize.y; y++)
		{
			for (var x = 0; x < brushSize.x; x++)
			{
				heights[y, x] -= strength * Time.deltaTime;
			}
		}

		terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
	}

	public void FlattenTerrain(Vector3 worldPosition, float height, int brushWidth, int brushHeight)
	{
		var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);

		var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

		var terrainData = GetTerrainData();

		var heights = terrainData.GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

		for (var y = 0; y < brushSize.y; y++)
		{
			for (var x = 0; x < brushSize.x; x++)
			{
				heights[y, x] = height;
			}
		}

		terrainData.SetHeights(brushPosition.x, brushPosition.y, heights);
	}

	public float SampleHeight(Vector3 worldPosition)
	{
		var terrainPosition = WorldToTerrainPosition(worldPosition);

		return GetTerrainData().GetInterpolatedHeight((int)terrainPosition.x, (int)terrainPosition.z);
	}

	public float SampleAverageHeight(Vector3 worldPosition, int brushWidth, int brushHeight)
	{
		var brushPosition = GetBrushPosition(worldPosition, brushWidth, brushHeight);

		var brushSize = GetSafeBrushSize(brushPosition.x, brushPosition.y, brushWidth, brushHeight);

		var heights2D = GetTerrainData().GetHeights(brushPosition.x, brushPosition.y, brushSize.x, brushSize.y);

		var heights = new float[heights2D.Length];

		var i = 0;

		for (int y = 0; y <= heights2D.GetUpperBound(0); y++)
		{
			for (int x = 0; x <= heights2D.GetUpperBound(1); x++)
			{
				heights[i++] = heights2D[y, x];
			}
		}

		return heights.Average();
	}

	private void UpdateTerrainTexture(Vector3 worldPosition, int textureIndex, int textureWidth, int textureHeight)
	{
		// Get the position of the texture brush and the safe size of the brush
		Vector2Int brushPosition = GetBrushPosition(worldPosition, textureWidth, textureHeight);
		Vector2Int brush = GetSafeBrushSize(brushPosition.x, brushPosition.y, textureWidth, textureHeight);
		// Get the terrain data at the location
		TerrainData terrainData = GetTerrainData();
		// Get the number of textures on the terrain
		int texturesCount = terrainData.alphamapLayers;
		// Create a 3D array to hold the new alpha values for each texture on the terrain
		float[,,] textureAlphas = new float[brush.y, brush.x, texturesCount];
		// Loop through each pixel in the brush area and set the alpha value for the specified texture index to 1, and 0 for all others
		for (var y = 0; y < brush.y; y++)
		{
			for (var x = 0; x < brush.x; x++)
			{
				for (var i = 0; i < texturesCount; i++)
				{
					// If the current texture index matches the specified texture index, set its alpha value to 1
					// Otherwise, set its alpha value to 0
					textureAlphas[y, x, i] = (i == textureIndex) ? 1.0f : 0.0f;
				}
			}
		}
		// Set the alpha map at the specified position to the updated texture alphas
		terrainData.SetAlphamaps(brushPosition.x, brushPosition.y, textureAlphas);
	}
}