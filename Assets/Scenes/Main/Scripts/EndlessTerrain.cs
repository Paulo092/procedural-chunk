using System;
using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EndlessTerrain : MonoBehaviour
{
	// Provisorio
	public Material terrainTexture;
	public GameObject treePrefab;
	
	public Transform viewer;
	
	private static Vector2 _viewerPosition;
	private static int _chunkSize;
	
	private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	private readonly List<ChunkInfo> _farLands = new List<ChunkInfo>();
	
	private const float MaxViewDst = 450;
	private int _chunksVisibleInViewDst;
	private LocalPool _terrainPool;
	
	private void Start() {
		_chunkSize = 100 - 1;
		_chunksVisibleInViewDst = Mathf.RoundToInt(MaxViewDst / _chunkSize);

		_terrainPool = new LocalPool(this.gameObject, terrainTexture, treePrefab);
	}

	void Update()
	{
		Vector3 viwerPositionReference = viewer.position;
		_viewerPosition = new Vector2(viwerPositionReference.x, viwerPositionReference.z);
		UpdateVisibleChunks();
	}
		
	void UpdateVisibleChunks() {

		for(int i = 0; i < _farLands.Count; i++) {
			if (!_farLands[i].Chunk.IsNearby())
			{
				_terrainPool.ReleaseObject(_farLands[i].Chunk);
				_terrainChunkDictionary.Remove(_farLands[i].Coord);
			}
		}
		
		_farLands.Clear();
			
		int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

		for(int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++) {
			for(int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				float maxViewDistanceInChunks = MaxViewDst / _chunkSize;
				if (Vector2.Distance(viewedChunkCoord,
					    new Vector2(viewer.transform.position.x, viewer.transform.position.z) / _chunkSize) >
				    maxViewDistanceInChunks) continue;
				
				if(_terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
					_farLands.Add(new ChunkInfo(viewedChunkCoord, _terrainChunkDictionary[viewedChunkCoord]));
				} else {
					TerrainChunk terrainChunk = _terrainPool.GetPooledObject(viewedChunkCoord, _chunkSize);
					
					_terrainChunkDictionary.Add(viewedChunkCoord, terrainChunk);
					_farLands.Add(new ChunkInfo(viewedChunkCoord, terrainChunk));
				}
			}
		}
	}

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

	private class TerrainChunk
	{

		private readonly GameObject _meshObject;

		private Vector2 _position, _relativePosition;
		private Terrain _baseTerrain;

		// Provisorio
		private GameObject treePrefab;
		
		// Provisorio
		// public TerrainChunk() {
		public TerrainChunk(Material terrainTexture, GameObject treePrefab) {
			// Cria um novo objeto Terrain
			_meshObject = new GameObject("Terrain");
			Terrain terrain = _meshObject.AddComponent<Terrain>();
			TerrainCollider terrainCollider = _meshObject.AddComponent<TerrainCollider>();

			// Cria e configura o TerrainData
			TerrainData terrainData = new TerrainData();
			terrainData.heightmapResolution = 512;
			terrainData.size = new Vector3(_chunkSize, 100, _chunkSize);
			// terrainData.SetHeights(0, 0, GeneratePerlinNoiseHeightmap(terrainData.heightmapResolution));

			// Associa o TerrainData ao Terrain e ao TerrainCollider
			terrain.terrainData = terrainData;
			terrainCollider.terrainData = terrainData;
			
			// Provisorio
			terrain.materialTemplate = terrainTexture;
			this.treePrefab = treePrefab;
			
			// _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			// _meshObject.AddComponent<MeshCollider>();

			_baseTerrain = terrain;
			_baseTerrain.gameObject.layer = LayerMask.NameToLayer("Ground");
			
			SetVisible(false);
		}

		float[,] GeneratePerlinNoiseHeightmap(int resolution)
		{
			float[,] heights = new float[resolution, resolution];

			float factorX = ((float)_chunkSize / (float)resolution) + _position.x;
			float factorY = ((float)_chunkSize / (float)resolution) + _position.y;

			float scale = 800f;
			float max = 0, min = 100;

			for (int x = 0; x < resolution; x++)
			{
				for (int y = 0; y < resolution; y++)
				{
					// float xCoord = (float)x / (float)resolution * size + offsetX;
					// float zCoord = (float)z / (float)resolution * size + offsetZ;

					// float sample = Mathf.PerlinNoise(xCoord * 0.005f, zCoord * 0.005f);
					float sample = Mathf.PerlinNoise(
						(((resolution - 1) * _relativePosition.x) + x) / scale,
						(((resolution - 1) * _relativePosition.y) + y) / scale
					);

					sample *= (Mathf.PerlinNoise(
						((((resolution - 1) * _relativePosition.x) + x) + 1000) / scale,
						((((resolution - 1) * _relativePosition.y) + y) + 1000) / scale
					) + .5f);
                    
					float novoX = (resolution * _relativePosition.x) + x;
					float novoY = (resolution * _relativePosition.y) + y;
					
					// sample = sample / (130 - 10 * ((_relativePosition.x + _relativePosition.y) % 10));

					// sample = (256 * sample - 16 * ((_relativePosition.x + _relativePosition.y) % 16)) / 256f;
					// sample = ((_relativePosition.x + _relativePosition.y) % 2 == 0 ? 0f : 0.5f);
					// sample = sample * ((16 * ((_relativePosition.x + _relativePosition.y) % 16)) + 256) / 512f;
     
					// Ultimo
					// sample *= (Mathf.Abs((novoX + novoY + 827269 * _relativePosition.x + 89189189 * _relativePosition.x + 131313 * (((long)novoX) ^ ((long)novoY)) + 424242 * (((long)_relativePosition.x) | ((long)_relativePosition.y))) % 2048 - 1024) / 1024);
                    
                    
                    
					heights[y, x] = sample ;  // Troca z e x para corrigir a rotação do terren
				}
			}
			
			return heights;
		}
		
		public void Setup(Vector2 coord, int size, Transform parent)
		{
			this._relativePosition = coord;
			
			_position = coord * size;
			
			Vector3 positionV3 = new Vector3(_position.x,0,_position.y);

			_meshObject.transform.position = positionV3;
			_meshObject.transform.localScale = Vector3.one * size / 10f;
			_meshObject.transform.parent = parent;
			
			_baseTerrain.terrainData.SetHeights(0, 0, GeneratePerlinNoiseHeightmap(_baseTerrain.terrainData.heightmapResolution));

			AddTrees(_baseTerrain);
			
			SetVisible(true);
		}
		
		void AddTrees(Terrain terrain)
		{
			TerrainData terrainData = terrain.terrainData;

			TreePrototype treePrototype = new TreePrototype();
			treePrototype.prefab = treePrefab;

			terrainData.treePrototypes = new TreePrototype[] { treePrototype };

			float terrainWidth = terrain.terrainData.size.x;
			float terrainLength = terrain.terrainData.size.z;
			
			for (int i = 0; i < 100; i++)
			{
				TreeInstance treeInstance = new TreeInstance();
				
				float x = Random.Range(0f, terrainWidth) / terrainWidth;
				float z = Random.Range(0f, terrainLength) / terrainLength;
				float y = terrain.SampleHeight(new Vector3(x * terrainWidth, 0, z * terrainLength)) / terrain.terrainData.size.y;

				treeInstance.position = new Vector3(x, y, z);
				treeInstance.prototypeIndex = 0;
				treeInstance.widthScale = 1;
				treeInstance.heightScale = 1;
				treeInstance.color = Color.white;
				treeInstance.lightmapColor = Color.white;

				terrain.AddTreeInstance(treeInstance);
			}
		}

		public void SetVisible(bool visible) {
			_meshObject.SetActive(visible);
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
	
	private class LocalPool
    {
    
        private const int InitialPoolSize = 100;
        private const int MaxPoolSize = 1000;
    
        private readonly ObjectPool<TerrainChunk> _pool;
        public GameObject Prefab;
	    private readonly GameObject _parent;
        private int _activeObjects;
    
        // Provisorio
        // public LocalPool(GameObject parent)
        private Material terrainTexture;
        private GameObject treePrefab;
        
        public LocalPool(GameObject parent, Material terrainTexture, GameObject treePrefab)
        {
	        // Provisorio
	        this.terrainTexture = terrainTexture;
	        this.treePrefab = treePrefab;
	        
	        this._parent = parent;
            
            _pool = new ObjectPool<TerrainChunk>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true, 
                InitialPoolSize, 
                MaxPoolSize);
        }
        
        private TerrainChunk CreatePooledItem()
        {
	        // Provisorio
            // return new TerrainChunk();
            return new TerrainChunk(terrainTexture, treePrefab);
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
    
        public TerrainChunk GetPooledObject(Vector2 coord, int size)
        {
            if(_activeObjects >= MaxPoolSize)
            {
                // Debug.LogWarning("Max pool size reached. Cannot spawn new objects.");
                // Provisorio
                // return new TerrainChunk();
                return new TerrainChunk(terrainTexture, treePrefab);
            }
    
            TerrainChunk pooledObject = _pool.Get();
            pooledObject.Setup(coord, size, _parent.transform);
            
            return pooledObject;
        }
    
        public void ReleaseObject(TerrainChunk obj)
        {
            _activeObjects--;
            _pool.Release(obj);
        }
    }
}
