﻿using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Den.Tools;
using Den.Tools.Matrices;
using MapMagic.Products;
using Den.Tools.GUI;

namespace MapMagic.Nodes.Biomes
{
	public class WhittakerLayer : IBiome, IOutlet<MatrixWorld>
	{
		public string name;
		public float opacity;
		public float influence;
		public string diagramName;

		public Generator Gen { get { return gen; } private set { gen = value;} }
		public Generator gen; //property is not serialized
		public void SetGen (Generator gen) => this.gen=gen;

		public ulong id; //properties not serialized
		public ulong Id { get{return id;} set{id=value;} } 
		public ulong LinkedOutletId { get; set; }  //if it's inlet. Assigned every before each clear or generate
		public ulong LinkedGenId { get; set; } 

		public IUnit ShallowCopy() => (WhittakerLayer)this.MemberwiseClone();

		public Expose.Override Override { get{return null;} set{} }

		public Graph graph;
		public Graph SubGraph => graph;
		public Graph AssignedGraph => graph;
		public TileData SubData (TileData parent) => parent.GetSubData(id);

		public WhittakerLayer () { opacity=1; }
		public WhittakerLayer (string name) { opacity=1; this.name=name; }
		public WhittakerLayer (string name, string diagramName) { opacity=1; this.name=name; this.diagramName=diagramName; }

/*		public TileData GetSubData (TileData parentData)
		{
			TileData usedData = parentData.subDatas[this];
			if (usedData == null)
			{
				usedData = new TileData(parentData);
				parentData.subDatas[this] = usedData;
			}
			return usedData;
		}*/

		public bool guiExpanded;
	}

	[Serializable]
	[GeneratorMenu (menu="Biomes", name ="Whittaker", iconName="GeneratorIcons/Whittaker", priority = 1, colorType = typeof(IBiome))]
	public class Whittaker200 : Generator, IPrepare, IMultiInlet, ICustomComplexity, IMultiLayer, ICustomClear
	{
		public static MatrixAsset[] diagramAssets;

		[Val("Heat", "Inlet")] public readonly Inlet<MatrixWorld> temperatureIn = new Inlet<MatrixWorld>();
		[Val("Moisture ", "Inlet")]	public readonly Inlet<MatrixWorld> moistureIn = new Inlet<MatrixWorld>(); 
		public IEnumerable<IInlet<object>> Inlets () { yield return temperatureIn; yield return moistureIn; }

		[Val("Sharpness")] public float sharpness = 0.6f;

		//[Val(name="Temperature Limit")] public float brightness = 0f;
		//[Val(name="Contrast")] public float contrast = 1f;

		public WhittakerLayer[] layers = new WhittakerLayer[]
		{
			new WhittakerLayer("Tropic Rainforest", "TropicalRainforest"),
			new WhittakerLayer("Mild Rainforest", "TemperateRainforest"),

			new WhittakerLayer("Tropic Forest", "TropicalForest"),
			new WhittakerLayer("Mild Forest", "TemperateForest"),
			new WhittakerLayer("Taiga", "Taiga"),

			new WhittakerLayer("Savanna", "Savanna"),
			new WhittakerLayer("Grassland", "Grassland"),
			new WhittakerLayer("Tundra", "Tundra"),

			new WhittakerLayer("Hot Desert", "Desert"),
			new WhittakerLayer("Cold Desert", "ColdDesert")
		};

		public IList<IUnit> Layers { get => layers; set {return;} } //don't set anything
		public void SetLayers(object[] ls) {return;}
		public bool Inversed => false;
		public bool HideFirst => false;


	//	public ICollection<IUnit> Layerss { get => Layers; }
	//	public void SetLayers(object[] ls) => layers = Array.ConvertAll(ls, i=>(BiomeLayer)i);
	//IMultiLayer

		public IEnumerable<IOutlet<object>> Outlets () 
		{ 
			foreach (WhittakerLayer layer in layers)
				yield return layer;
		}

		public IEnumerable<IBiome> Biomes() 
		{ 
			foreach (WhittakerLayer layer in layers)
				yield return layer;
		}

		public float Complexity
		{get{
			float sum = 0;
			foreach (WhittakerLayer layer in layers)
				if (layer.graph != null)
					sum += layer.graph.GetGenerateComplexity();
			return sum;
		}}

		public float Progress (TileData data)
		{
			float sum = 0;
			foreach (WhittakerLayer layer in layers)
			{
				if (layer.graph == null) continue;

				TileData subData = layer.SubData(data);
				if (subData == null) continue;

				sum += layer.graph.GetGenerateProgress(subData);
			}
			return sum;
		}


		public void Prepare (TileData data, Terrain terrain)
		{
			foreach (WhittakerLayer layer in layers)
			{
				if (layer.graph == null) continue;

				TileData subData = layer.SubData(data);
				if (subData == null) continue;

				layer.graph.Prepare(subData, terrain);
			}

			if (diagramAssets == null)
			{
				diagramAssets = new MatrixAsset[10];
				int i=0;
				foreach (WhittakerLayer layer in layers)
				{
					diagramAssets[i] =  Resources.Load<MatrixAsset>("MapMagic/Whittaker/" + layer.diagramName);
					i++;
				}
			}
		}


		public override void Generate (TileData data, StopToken stop) 
		{
			//reading inputs
			if (stop!=null && stop.stop) return;
			MatrixWorld temperatureMatrix = data.ReadInletProduct(temperatureIn);
			MatrixWorld moistureMatrix = data.ReadInletProduct(moistureIn);
			if (temperatureMatrix == null || moistureMatrix == null || !enabled) return; 

			if (diagramAssets == null)
				throw new Exception("Could not find Whittaker diagrams. Possibly generator is not initialized");
			Coord diagramSize = diagramAssets[0].matrix.rect.size;

			//creating biome masks
			if (stop!=null && stop.stop) return;
			MatrixWorld[] masks = new MatrixWorld[10];
			for (int i=0; i<masks.Length; i++)
				masks[i] = new MatrixWorld(temperatureMatrix.rect, temperatureMatrix.worldPos, temperatureMatrix.worldSize);

			if (stop!=null && stop.stop) return;
			Coord min = temperatureMatrix.rect.Min; Coord max = temperatureMatrix.rect.Max;
			for (int x=min.x; x<max.x; x++)
				for (int z=min.z; z<max.z; z++)
				{
					int pos = temperatureMatrix.rect.GetPos(x,z);
					float temperature = temperatureMatrix.arr[pos] * diagramSize.x;
					float moisture = moistureMatrix.arr[pos] * diagramSize.z;

					if (temperature < 0) temperature = 0; if (temperature > diagramSize.x) temperature = diagramSize.x;
					if (moisture < 0) moisture = 0; if (moisture > diagramSize.z) moisture = diagramSize.z;

					for (int m=0; m<masks.Length; m++)
					{
						float val = diagramAssets[m].matrix.GetInterpolated(temperature, moisture);
						val -= sharpness/2;
						if (val < 0) val = 0;
						masks[m].arr[pos] = val;
					}
				}

			//and opacities
			if (stop!=null && stop.stop) return;
			float[] opacities = new float[masks.Length];
			int t=0;
			foreach (WhittakerLayer layer in layers)
				{ opacities[t] = layer.opacity; t++; }

			if (stop!=null && stop.stop) return;
			Matrix.NormalizeLayers(masks, opacities, allowBelowOne:false);

			//saving products
			if (stop!=null && stop.stop) return;
			t=0;
			foreach (WhittakerLayer layer in layers)
			{ 
				data.StoreProduct(layer.Id, masks[t]); 
				t++;
			}

			//generating biomes
			for (int i=0; i<layers.Length; i++)
			{
				if (stop!=null && stop.stop) return;

				WhittakerLayer layer = layers[i];

				MatrixWorld mask;
				if (data.biomeMask == null)
					mask = masks[i]; //no need to copy for first-level biome
				else
				{
					mask = new MatrixWorld(masks[i]);
					mask.Multiply(data.biomeMask);
				}

				Graph subGraph = layer.SubGraph;
				if (subGraph == null) continue;
				TileData subData = data.CreateLoadSubData(layer.Id, mask);

				if (mask.MaxValue() > 0.0001f)
					layer.graph.Generate(subData, stop:stop, ovd:layer.graph.defaults);
			}
		}

		public void OnClearing (Graph graph, TileData data, ref bool isReady, bool totalRebuild=false) 
		{
			// What should be cleared and when:
			// - On this graph modification (inlet change):			this node (done by default), all subgraph outputs (for biomes)
			// - Subgraph modification (any subgraph node change):	this node, all subgraph relevants
			// - Exposed values change (this node change):			this node (done by default), exp related subgraph node, subgraph relevants

			//clarifying whether this generator changed directly or recursively
			bool versionChanged = data.VersionChanged(this);
			bool thisChanged = !isReady && versionChanged;
			bool inletChanged = !isReady && !versionChanged;

			//iterating subgraphs/subdatas
			foreach (WhittakerLayer layer in layers)
			{
				Graph subGraph = layer.SubGraph;
				if (subGraph == null) return;

				TileData subData = layer.SubData(data);
				if (subData == null) return; //in case biome has not been generated ever yet, but dragging field 

				//resetting exposed related nodes on this node change
				if (thisChanged)
				{
					//TODO: reset only changed generators
					foreach (IUnit expUnit in subGraph.exposed.AllUnits(subGraph))
						subData.ClearReady((Generator)expUnit);
				}

				//resetting outputs/relevants on inlet or this changed
				if (inletChanged || thisChanged)
				{
					foreach (Generator relGen in subGraph.RelevantGenerators(data.isDraft)) 
						subData.ClearReady(relGen);
				}

				//iterating in sub-graph after
				subGraph.ClearChanged(subData);

				//at the end clearing this if any subgraph relevant changed
				if (isReady)
				{
					foreach (Generator relGen in subGraph.RelevantGenerators(data.isDraft)) 
						if (!subData.IsReady(relGen))
							isReady = false;
				}
			}
		}
	}
}
