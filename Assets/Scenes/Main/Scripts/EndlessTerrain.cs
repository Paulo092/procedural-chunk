using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Random = System.Random;

public class EndlessTerrain : MonoBehaviour
{
	#region Fields
	
	[Header("Basic Setup")] 
	public Transform playerReference;
	public Universe universeData;
	public Material baseTerrainMaterial;
	
	[Header("Generation Options")] 
	public int chunkSize = 100;
	public bool randomSeed;
	public int seed = 12345678;
	
	[Header("Pool Config")]
	public int initialPoolSize = 100;
	public int maxPoolSize = 1000;
	
	#endregion

	#region Properties

	private static Vector2 _viewerPosition;
	private static int _chunkSize;
	
	private readonly List<ChunkInfo> _loadedLands = new();
	private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new();
	
	private LocalPool _terrainPool;
	private int _chunksVisibleInViewDst;
	private const float MaxViewDst = 450;

	private Vector2 _lastVisitedChunk;
	
	#endregion

	#region Manager
	
	private void Start() {
		_chunkSize = chunkSize - 1;
		_chunksVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / _chunkSize);
		universeData.SetupBiomes();

		_terrainPool = new LocalPool(initialPoolSize, maxPoolSize, this.gameObject, universeData, baseTerrainMaterial);
	}

	void Update()
	{
		Vector3 viwerPositionReference = playerReference.position;
		_viewerPosition = new Vector2(viwerPositionReference.x, viwerPositionReference.z);

		UpdateVisibleChunks();
	}
		
	void UpdateVisibleChunks()
	{
		for(int i = 0; i < _loadedLands.Count; i++) {
			if (!_loadedLands[i].Chunk.IsNearby())
			{
				_terrainPool.ReleaseObject(_loadedLands[i].Chunk);
				_terrainChunkDictionary.Remove(_loadedLands[i].Coord);
			}
		}
		
		_loadedLands.Clear();
			
		int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

		for(int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++) {
			for(int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				float maxViewDistanceInChunks = MaxViewDst / _chunkSize;
				if (Vector2.Distance(viewedChunkCoord,
					    new Vector2(playerReference.transform.position.x, playerReference.transform.position.z) / _chunkSize) >
				    maxViewDistanceInChunks) continue;
				
				if(_terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
					_loadedLands.Add(new ChunkInfo(viewedChunkCoord, _terrainChunkDictionary[viewedChunkCoord]));
				} else {
					TerrainChunk terrainChunk = _terrainPool.GetPooledObject(viewedChunkCoord, _chunkSize, seed, universeData);
					
					_terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
					_loadedLands.Add(new ChunkInfo(viewedChunkCoord, terrainChunk));
				}
			}
		}
	}
	
	#endregion

	#region ChunkClass

	private class ChunkInfo
	{
		public readonly Vector2 Coord;
		public readonly TerrainChunk Chunk;
        
		public ChunkInfo(Vector2 coord, TerrainChunk chunk)
		{
			this.Coord = coord;
			this.Chunk = chunk;
		}
	}

	private class ObjectSpawnInfo
	{
		public GameObject Prefab;
		public Vector3 SpawnCoordinate;
		public Quaternion Rotation;
		public Vector2Int CoordinateInTerrain;

		public ObjectSpawnInfo(
			GameObject prefab,
			Vector3 spawnCoordinate,
			Quaternion rotation,
			Vector2Int coordinateInTerrain
		)
		{
			Prefab = prefab;
			SpawnCoordinate = spawnCoordinate;
			Rotation = rotation;
			CoordinateInTerrain = coordinateInTerrain;
		}
	}
	
	private class TerrainChunk
	{
		private readonly GameObject _meshObject;

		private Vector2 _position, _relativePosition;
		private Terrain _baseTerrain;
		private TerrainData _baseTerrainData;
		private Universe _universeData;
		private int _seed;
		
		private List<GameObject> _instantiatedObjects = new List<GameObject>();
		
		public TerrainChunk(Universe universeData, Material baseTerrainMaterial)
		{
			this._universeData = universeData;
			
			_meshObject = new GameObject("Terrain");
			Terrain terrain = _meshObject.AddComponent<Terrain>();
			TerrainCollider terrainCollider = _meshObject.AddComponent<TerrainCollider>();
			
			terrainCollider.providesContacts = true;
			
			TerrainData terrainData = new TerrainData();
			terrainData.heightmapResolution = 512;
			terrainData.size = new Vector3(_chunkSize, 100, _chunkSize);

			_baseTerrainData = terrainData;
			
			terrain.terrainData = _baseTerrainData;
			terrainCollider.terrainData = _baseTerrainData;
			
			// terrain.materialTemplate = terrainTexture;
			
			terrain.materialTemplate = baseTerrainMaterial;
			
			_baseTerrain = terrain;
			_baseTerrain.gameObject.layer = LayerMask.NameToLayer("Ground");
			
			SetupTextures(_baseTerrain);
			SetVisible(false);
		}

		public void Setup(Vector2 coord, int size, Transform parent, int seed)
		{
			_relativePosition = coord;
			_position = coord * size;
			_seed = seed;
			
			Vector3 positionV3 = new Vector3(_position.x,0,_position.y);

			// _meshObject.transform.position = positionV3;
			_meshObject.transform.position = positionV3 + new Vector3(-.01f * coord.x, 0, -.01f * coord.y);
			
			_meshObject.transform.localScale = Vector3.one * size / 10f;
			_meshObject.transform.parent = parent;

			GenerateHeightmapSetupAsync(
				_baseTerrain.terrainData.heightmapResolution
			);

			int alphamapWidth = _baseTerrainData.alphamapWidth,
				alphamapHeight = _baseTerrainData.alphamapHeight,
				alphamapResolution = _baseTerrainData.alphamapHeight,
				alphamapLayersCount = _baseTerrainData.terrainLayers.Length,
				heightmapResolution = _baseTerrainData.heightmapResolution;
			
			Vector3 terrainSize = _baseTerrainData.size;
			
			BiomesTexturesAndTreesSetupAsync(
				alphamapWidth,
				alphamapHeight,
				alphamapResolution,
				alphamapLayersCount
			);

			StructurePlacingSetup(heightmapResolution, terrainSize.y);
			
			SetVisible(true);
		}
		
		void SetupTextures(Terrain terrain)
		{
			TerrainLayer[] terrainLayers = new TerrainLayer[_universeData.biomeList.Length];
			int index = 0;

			Biome[] biomes = _universeData.biomeList;

			foreach (var biome in biomes)
			{
				foreach (var biomeTexture in biome.biomeTextures)
				{
					terrainLayers[index] = new TerrainLayer();
					terrainLayers[index].name = "Texture" + biome.biomeName + "_GU$" + index;
					terrainLayers[index].diffuseTexture = biomeTexture.texture;
					terrainLayers[index].tileSize = new Vector2(50f, 50f);
					biomeTexture.layer = index;
	
					index++;
				}
			}

			terrain.terrainData.terrainLayers = terrainLayers;
		}

		async void GenerateHeightmapSetupAsync(int resolution)
		{
			float[,] heights = new float[resolution, resolution];

			var result = await Task.Run(() =>
			{
				for (int x = 0; x < resolution; x++)
				{
					for (int y = 0; y < resolution; y++)
					{
						heights[y, x] = GetHeightByCoordinate(resolution, x, y);
					}
				}

				return heights;
			});
			
			_baseTerrain.terrainData.SetHeights(0, 0, result);
		}
		
		async void BiomesTexturesAndTreesSetupAsync(
			int alphamapWidth,
			int alphamapHeight,
			int alphamapResolution,
			int alphamapLayersCount
		)
		{

			int width = alphamapWidth,
				height = alphamapHeight,
				resolution = alphamapResolution,
				terrainHeight = (int) _baseTerrainData.size.y;
			
			float noiseScale = 8000f;
			
			var result = await Task.Run(() =>
			{
				float[,,] alphaMap = new float[
					alphamapWidth, 
					alphamapHeight, 
					alphamapLayersCount
				];
				
				List<ObjectSpawnInfo> treesToSpawn = new();
				
				for (int y = 0; y < width; y++)
				{
					float currentGlobalCoordinateY = ((resolution - 1) * _relativePosition.y) + y;

					for (int x = 0; x < height; x++)
					{
						float currentGlobalCoordinateX  = ((resolution - 1) * _relativePosition.x) + x;

						float temperature = Mathf.PerlinNoise(
							currentGlobalCoordinateX / noiseScale,
							currentGlobalCoordinateY / noiseScale
						);

						float humidity = Mathf.PerlinNoise(
							currentGlobalCoordinateX / (noiseScale * 1.8f),
							currentGlobalCoordinateY / (noiseScale * 1.8f)
						);

						Biome currentBiome = _universeData.GetBiomeByParameters(temperature, humidity);
						int biomeLayer = currentBiome.biomeTextures[0].layer;

						alphaMap[y, x, biomeLayer] = 1;
						
						#region Choose Tree Coordinates
						
						if (x != 0 && y != 0 && x % 70 == 0 && y % 70 == 0)
						{
							Random rand = new Random();
							float prob = rand.Next(1, 101) / 100f;
							bool canPlace = prob <= currentBiome.treeDensity;

							if (!canPlace) continue;
							
							int xOffsetInTerrain = rand.Next(0, 50);
							int yOffsetInTerrain = rand.Next(0, 50);
							
							// int xOffsetInTerrain = 0;
							// int yOffsetInTerrain = 0;
							
							float xToSpawn = (_relativePosition.x * _chunkSize) + ((x + xOffsetInTerrain) * ((float)_chunkSize / resolution));
							float yToSpawn = (_relativePosition.y * _chunkSize) + ((y + yOffsetInTerrain) * ((float)_chunkSize / resolution));

							Biome.BiomeTree chosenTree = currentBiome.GetTree();

							if (chosenTree != null)
							{
								float hToSpawn = GetHeightByCoordinate(
									resolution,
									x + xOffsetInTerrain,
									y + yOffsetInTerrain
								) * terrainHeight;
								
								treesToSpawn.Add(new ObjectSpawnInfo(
										chosenTree.treePrefab,
										new Vector3(
											xToSpawn,
											hToSpawn,
											yToSpawn
										),
										Quaternion.identity,
										new Vector2Int(x + xOffsetInTerrain, y + yOffsetInTerrain)
									)
								);
							}
						}
						
						#endregion
						
						// /*
						// #region Place Structure
						//
						// Random structureRand = new Random((int)(_relativePosition.x * _relativePosition.y * _seed));
						// float chanceToHaveAStructure = .1f;
						//
						// float xToSpawnS = (_relativePosition.x * _chunkSize) + (x * ((float)_chunkSize / resolution));
						// float yToSpawnS = (_relativePosition.y * _chunkSize) + (y * ((float)_chunkSize / resolution));
						// float hToSpawnS = GetHeightByCoordinate(
						// 	resolution,
						// 	x,
						// 	y
						// ) * terrainHeight;
						//
						// if ((structureRand.Next(1, 101) / 100f) < chanceToHaveAStructure)
						// {
						// 	structureSpawnPosition = new Vector3(xToSpawnS, hToSpawnS, yToSpawnS);
						// 	structureToSpawn = _universeData.structureList[0].prefab;
						// }
						//
						// #endregion

						// */
					}
				}
				
				// return (alphaMap, treesToSpawn, structureToSpawn, structureSpawnPosition);
				return (alphaMap, treesToSpawn);
			});

			foreach (var tree in result.treesToSpawn)
			{
				GameObject spawnedTree = Instantiate(
					tree.Prefab,
					tree.SpawnCoordinate,
					tree.Rotation
				);

				spawnedTree.transform.parent = this._meshObject.transform;
				
				_instantiatedObjects.Add(spawnedTree);
			}
			
			_baseTerrainData.SetAlphamaps(0, 0, result.alphaMap);
		}

		private void StructurePlacingSetup(int resolution, float terrainHeight)
		{
			Random structureRand = new Random((int)(_relativePosition.x * _relativePosition.y * _seed));
			float chanceToHaveAStructure = .1f;

			Random rand = new Random();
			int xLocal = rand.Next(0, resolution);
			int yLocal = rand.Next(0, resolution);
			
			float xToSpawnS = (_relativePosition.x * _chunkSize) + (xLocal * ((float)_chunkSize / resolution));
			float yToSpawnS = (_relativePosition.y * _chunkSize) + (yLocal * ((float)_chunkSize / resolution));
			float hToSpawnS = GetHeightByCoordinate(
				resolution,
				xLocal,
				yLocal
			) * terrainHeight;

			if ((structureRand.Next(1, 101) / 100f) < chanceToHaveAStructure)
			{
				Vector3 structureSpawnPosition = new Vector3(xToSpawnS, hToSpawnS, yToSpawnS);
				GameObject structureToSpawn = _universeData.structureList[0].prefab;
				
				_instantiatedObjects.Add(
				Instantiate(structureToSpawn, structureSpawnPosition, Quaternion.identity)
				);
			}
			
		}
		
		float GetHeightByCoordinate(int resolution, int x, int y)
		{
			float seedFactor = (_seed / (float)_chunkSize * resolution) / 100;
			float seedFactor2 = (seedFactor * _seed) / 1000;

			float scale = 800f;

			float sample = Mathf.PerlinNoise(
				(((resolution - 1) * _relativePosition.x) + x + seedFactor) / scale,
				(((resolution - 1) * _relativePosition.y) + y + seedFactor) / scale
			);

			sample *= (Mathf.PerlinNoise(
				((((resolution - 1) * _relativePosition.x) + x + seedFactor2) + 1000) / scale,
				((((resolution - 1) * _relativePosition.y) + y + seedFactor2) + 1000) / scale
			) + .5f);
                    
			sample *= (Mathf.PerlinNoise(
				((((resolution - 1) * _relativePosition.x) + x) + 42000) / scale * 1.5f,
				((((resolution - 1) * _relativePosition.y) + y) + -42000) / scale * 1.5f
			));
						
			return sample;
		}
		
		public void SetVisible(bool visible) {
			_meshObject.SetActive(visible);

			if (!visible)
			{
				foreach (GameObject instantiatedObject in _instantiatedObjects)
				{
					Destroy(instantiatedObject);
				}
				
				_instantiatedObjects.Clear();
			}
		}

		public Boolean IsNearby()
		{
			if (Vector2.Distance(
				    _relativePosition,
				    new Vector2(
					    _viewerPosition.x, 
					    _viewerPosition.y) / _chunkSize
					) > MaxViewDst / _chunkSize
				)
			{
				return false;
			}

			return true;
		}
		
		public void Die()
		{
			Destroy(_meshObject);
		}
	}

	#endregion

	#region PoolClass

	private class LocalPool
    {
        public GameObject Prefab;
        
	    private int _activeObjects; 
        
	    private readonly ObjectPool<TerrainChunk> _pool;
        private readonly Material _baseTerrainMaterial;
        private readonly Universe _universeData;
	    private readonly GameObject _parent;
	    private readonly int _maxPoolSize;
        
        public LocalPool(int initialPoolSize, int maxPoolSize, GameObject parent, Universe universeData, Material baseTerrainMaterial)
        {
	        this._parent = parent;
	        this._universeData = universeData;
	        this._baseTerrainMaterial = baseTerrainMaterial;
	        this._maxPoolSize = maxPoolSize;
            
            _pool = new ObjectPool<TerrainChunk>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true, 
                initialPoolSize, 
                maxPoolSize);
        }
        
        private TerrainChunk CreatePooledItem()
        {
            return new TerrainChunk(_universeData, _baseTerrainMaterial);
        }
    
        private static void OnReturnedToPool(TerrainChunk obj)
        {
            obj.SetVisible(false);
        }
    
        private void OnTakeFromPool(TerrainChunk obj)
        {
            obj.SetVisible(true);
            _activeObjects++;
        }
    
        private static void OnDestroyPoolObject(TerrainChunk obj)
        {
	        obj.Die();
        }
    
        public TerrainChunk GetPooledObject(Vector2 coord, int size, int seed, Universe universeData)
        {
            if(_activeObjects >= _maxPoolSize)
            {
                return new TerrainChunk(universeData, _baseTerrainMaterial);
            }
    
            TerrainChunk pooledObject = _pool.Get();
            pooledObject.Setup(coord, size, _parent.transform, seed);
            
            return pooledObject;
        }
    
        public void ReleaseObject(TerrainChunk obj)
        {
            _activeObjects--;
            _pool.Release(obj);
        }
    }

	#endregion
}
