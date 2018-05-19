﻿/**
 * Kopernicus Planetary System Modifier
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2017 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using LibNoise;
using System;
using System.Linq;
using Kopernicus.Components;
using Kopernicus.Configuration.NoiseLoader;
using Kopernicus.UI;
using UnityEngine;
using RidgedMultifractal = LibNoise.RidgedMultifractal;

namespace Kopernicus
{
    namespace Configuration
    {
        namespace ModLoader
        {
            [RequireConfigType(ConfigType.Node)]
            public class VertexPlanet : ModLoader<PQSMod_VertexPlanet>
            {
                // Loader for the SimplexWrapper
                [RequireConfigType(ConfigType.Node)]
                public class SimplexLoader : ITypeParser<KopernicusSimplexWrapper>
                {
                    // Loaded wrapper
                    public KopernicusSimplexWrapper Value { get; set; }

                    // deformity
                    [ParserTarget("deformity")]
                    public NumericParser<Double> deformity
                    {
                        get { return Value.deformity; }
                        set { Value.deformity = value; }
                    }

                    // frequency
                    [ParserTarget("frequency")]
                    public NumericParser<Double> frequency
                    {
                        get { return Value.frequency; }
                        set { Value.frequency = value; }
                    }

                    // octaves
                    [ParserTarget("octaves")]
                    public NumericParser<Double> octaves
                    {
                        get { return Value.octaves; }
                        set { Value.octaves = value; }
                    }

                    // persistance
                    [ParserTarget("persistance")]
                    public NumericParser<Double> persistance
                    {
                        get { return Value.persistance; }
                        set { Value.persistance = value; }
                    }

                    // seed
                    [ParserTarget("seed")]
                    public NumericParser<Int32> seed
                    {
                        get { return Value.seed; }
                        set { Value.seed = value; }
                    }

                    // Default constructor
                    [KittopiaConstructor(KittopiaConstructor.Parameter.Empty)]
                    public SimplexLoader()
                    {
                        Value = new KopernicusSimplexWrapper(0, 0, 0, 0);
                    }

                    // Runtime constructor
                    public SimplexLoader(PQSMod_VertexPlanet.SimplexWrapper simplex)
                    {
                        Value = new KopernicusSimplexWrapper(simplex);
                    }

                    // Runtime constructor
                    public SimplexLoader(KopernicusSimplexWrapper simplex)
                    {
                        Value = simplex;
                    }

                    /// <summary>
                    /// Convert Parser to Value
                    /// </summary>
                    public static implicit operator KopernicusSimplexWrapper(SimplexLoader parser)
                    {
                        return parser.Value;
                    }
        
                    /// <summary>
                    /// Convert Value to Parser
                    /// </summary>
                    public static implicit operator SimplexLoader(KopernicusSimplexWrapper value)
                    {
                        if (value == null)
                            return null;
                        return new SimplexLoader(value);
                    }
        
                    /// <summary>
                    /// Convert Value to Parser
                    /// </summary>
                    public static implicit operator SimplexLoader(PQSMod_VertexPlanet.SimplexWrapper value)
                    {
                        if (value == null)
                            return null;
                        return new SimplexLoader(value);
                    }
                }

                // Loader for Noise
                [RequireConfigType(ConfigType.Node)]
                public class NoiseModLoader : ITypeParser<PQSMod_VertexPlanet.NoiseModWrapper>
                {
                    // The loaded noise
                    public PQSMod_VertexPlanet.NoiseModWrapper Value { get; set; }

                    // deformity
                    [ParserTarget("deformity")]
                    public NumericParser<Double> deformity
                    {
                        get { return Value.deformity; }
                        set { Value.deformity = value; }
                    }

                    // frequency
                    [ParserTarget("frequency")]
                    public NumericParser<Double> frequency
                    {
                        get { return Value.frequency; }
                        set { Value.frequency = value; }
                    }

                    // octaves
                    [ParserTarget("octaves")]
                    public NumericParser<Int32> octaves
                    {
                        get { return Value.octaves; }
                        set { Value.octaves = Mathf.Clamp(value, 1, 30); }
                    }

                    // persistance
                    [ParserTarget("persistance")]
                    public NumericParser<Double> persistance
                    {
                        get { return Value.persistance; }
                        set { Value.persistance = value; }
                    }

                    // seed
                    [ParserTarget("seed")]
                    public NumericParser<Int32> seedLoader
                    {
                        get { return Value.seed; }
                        set { Value.seed = value; }
                    }

                    // noise
                    [ParserTarget("Noise", AllowMerge = true, NameSignificance = NameSignificance.Type)]
                    [KittopiaHideOption]
                    public INoiseLoader noise
                    {
                        get
                        {
                            if (Value.noise != null)
                            {
                                Type noiseType = Value.noise.GetType();
                                foreach (Type loaderType in Parser.ModTypes)
                                {
                                    if (loaderType.BaseType == null)
                                        continue;
                                    if (loaderType.BaseType.Namespace != "Kopernicus.Configuration.NoiseLoader")
                                        continue;
                                    if (!loaderType.BaseType.Name.StartsWith("NoiseLoader"))
                                        continue;
                                    if (loaderType.BaseType.GetGenericArguments()[0] != noiseType)
                                        continue;
                        
                                    // We found our loader type
                                    INoiseLoader loader = (INoiseLoader) Activator.CreateInstance(loaderType);
                                    loader.Create(Value.noise);
                                    return loader;
                                }
                            }
                            return null;
                        }
                        set
                        {
                            Value.Setup(value.Noise);
                        }
                    }

                    // Default constructor
                    [KittopiaConstructor(KittopiaConstructor.Parameter.Empty)]
                    public NoiseModLoader()
                    {
                        Value = new PQSMod_VertexPlanet.NoiseModWrapper(0, 0, 0, 0);
                    }

                    // Runtime Constructor
                    public NoiseModLoader(PQSMod_VertexPlanet.NoiseModWrapper noise)
                    {
                        Value = noise;
                    }

                    /// <summary>
                    /// Convert Parser to Value
                    /// </summary>
                    public static implicit operator PQSMod_VertexPlanet.NoiseModWrapper(NoiseModLoader parser)
                    {
                        return parser.Value;
                    }
        
                    /// <summary>
                    /// Convert Value to Parser
                    /// </summary>
                    public static implicit operator NoiseModLoader(PQSMod_VertexPlanet.NoiseModWrapper value)
                    {
                        return new NoiseModLoader(value);
                    }
                }

                // Land class loader 
                [RequireConfigType(ConfigType.Node)]
                public class LandClassLoader : IPatchable, ITypeParser<PQSMod_VertexPlanet.LandClass>
                {
                    // Land class object
                    public PQSMod_VertexPlanet.LandClass Value { get; set; }

                    // Name of the class
                    [ParserTarget("name")]
                    public String name
                    {
                        get { return Value.name; }
                        set { Value.name = value; }
                    }

                    // Should we delete this
                    [ParserTarget("delete")]
                    public NumericParser<Boolean> delete = false;

                    // baseColor
                    [ParserTarget("baseColor")]
                    public ColorParser baseColor
                    {
                        get { return Value.baseColor; }
                        set { Value.baseColor = value; }
                    }

                    // colorNoise
                    [ParserTarget("colorNoise")]
                    public ColorParser colorNoise
                    {
                        get { return Value.colorNoise; }
                        set { Value.colorNoise = value; }
                    }

                    // colorNoiseAmount
                    [ParserTarget("colorNoiseAmount")]
                    public NumericParser<Double> colorNoiseAmount
                    {
                        get { return Value.colorNoiseAmount; }
                        set { Value.colorNoiseAmount = value; }
                    }

                    // colorNoiseMap
                    [ParserTarget("SimplexNoiseMap", AllowMerge = true)]
                    public SimplexLoader colorNoiseMap
                    {
                        get { return Value.colorNoiseMap; }
                        set { Value.colorNoiseMap = value; }
                    }

                    // fractalEnd
                    [ParserTarget("fractalEnd")]
                    public NumericParser<Double> fractalEnd
                    {
                        get { return Value.fractalEnd; }
                        set { Value.fractalEnd = value; }
                    }

                    // fractalStart
                    [ParserTarget("fractalStart")]
                    public NumericParser<Double> fractalStart
                    {
                        get { return Value.fractalStart; }
                        set { Value.fractalStart = value; }
                    }

                    // lerpToNext
                    [ParserTarget("lerpToNext")]
                    public NumericParser<Boolean> lerpToNext
                    {
                        get { return Value.lerpToNext; }
                        set { Value.lerpToNext = value; }
                    }

                    // fractalDelta
                    [ParserTarget("fractalDelta")]
                    public NumericParser<Double> fractalDelta
                    {
                        get { return Value.fractalDelta; }
                        set { Value.fractalDelta = value; }
                    }

                    // endHeight
                    [ParserTarget("endHeight")]
                    public NumericParser<Double> endHeight
                    {
                        get { return Value.endHeight; }
                        set { Value.endHeight = value; }
                    }

                    // startHeight
                    [ParserTarget("startHeight")]
                    public NumericParser<Double> startHeight
                    {
                        get { return Value.startHeight; }
                        set { Value.startHeight = value; }
                    }
                    
                    // Default constructor
                    [KittopiaConstructor(KittopiaConstructor.Parameter.Empty)]
                    public LandClassLoader()
                    {
                        Value = new PQSMod_VertexPlanet.LandClass("class", 0.0, 0.0, Color.white, Color.white, 0.0);
                    }

                    // Runtime constructor
                    public LandClassLoader(PQSMod_VertexPlanet.LandClass land)
                    {
                        Value = land;
                    }

                    /// <summary>
                    /// Convert Parser to Value
                    /// </summary>
                    public static implicit operator PQSMod_VertexPlanet.LandClass(LandClassLoader parser)
                    {
                        return parser?.Value;
                    }
        
                    /// <summary>
                    /// Convert Value to Parser
                    /// </summary>
                    public static implicit operator LandClassLoader(PQSMod_VertexPlanet.LandClass value)
                    {
                        return value != null ? new LandClassLoader(value) : null;
                    }
                }

                // buildHeightColors
                [ParserTarget("buildHeightColors")]
                public NumericParser<Boolean> buildHeightColors 
                {
                    get { return mod.buildHeightColors; }
                    set { mod.buildHeightColors = value; }
                }

                // colorDeformity
                [ParserTarget("colorDeformity")]
                public NumericParser<Double> colorDeformity
                {
                    get { return mod.colorDeformity; }
                    set { mod.colorDeformity = value; }
                }

                // continental
                [ParserTarget("ContinentalSimplex", AllowMerge = true)]
                public SimplexLoader continental
                {
                    get { return mod.continental; }
                    set { mod.continental = value; }
                }

                // continentalRuggedness
                [ParserTarget("RuggednessSimplex", AllowMerge = true)]
                public SimplexLoader continentalRuggedness
                {
                    get { return mod.continentalRuggedness; }
                    set { mod.continentalRuggedness = value; }
                }

                // continentalSharpness
                [ParserTarget("SharpnessNoise", AllowMerge = true)]
                public NoiseModLoader continentalSharpness
                {
                    get { return mod.continentalSharpness; }
                    set { mod.continentalSharpness = value; }
                }

                // continentalSharpnessMap
                [ParserTarget("SharpnessSimplexMap", AllowMerge = true)]
                public SimplexLoader continentalSharpnessMap
                {
                    get { return mod.continentalSharpnessMap; }
                    set { mod.continentalSharpnessMap = value; }
                }

                // deformity
                [ParserTarget("deformity")]
                public NumericParser<Double> deformity
                {
                    get { return mod.deformity; }
                    set { mod.deformity = value; }
                }

                // The land classes
                [ParserTargetCollection("LandClasses", AllowMerge = true)]
                public CallbackList<LandClassLoader> landClasses { get; set; }

                // oceanDepth
                [ParserTarget("oceanDepth")]
                public NumericParser<Double> oceanDepth
                {
                    get { return mod.oceanDepth; }
                    set { mod.oceanDepth = value; }
                }

                // oceanLevel
                [ParserTarget("oceanLevel")]
                public NumericParser<Double> oceanLevel
                {
                    get { return mod.oceanLevel; }
                    set { mod.oceanLevel = value; }
                }

                // oceanSnap
                [ParserTarget("oceanSnap")]
                public NumericParser<Boolean> oceanSnap
                {
                    get { return mod.oceanSnap; }
                    set { mod.oceanSnap = value; }
                }

                // oceanStep
                [ParserTarget("oceanStep")]
                public NumericParser<Double> oceanStep
                {
                    get { return mod.oceanStep; }
                    set { mod.oceanStep = value; }
                }

                // seed
                [ParserTarget("seed")]
                public NumericParser<Int32> seed
                {
                    get { return mod.seed; }
                    set { mod.seed = value; }
                }

                // terrainRidgeBalance
                [ParserTarget("terrainRidgeBalance")]
                public NumericParser<Double> terrainRidgeBalance
                {
                    get { return mod.terrainRidgeBalance; }
                    set { mod.terrainRidgeBalance = value; }
                }

                // terrainRidgesMax
                [ParserTarget("terrainRidgesMax")]
                public NumericParser<Double> terrainRidgesMax
                {
                    get { return mod.terrainRidgesMax; }
                    set { mod.terrainRidgesMax = value; }
                }

                // terrainRidgesMin
                [ParserTarget("terrainRidgesMin")]
                public NumericParser<Double> terrainRidgesMin
                {
                    get { return mod.terrainRidgesMin; }
                    set { mod.terrainRidgesMin = value; }
                }

                // terrainShapeEnd
                [ParserTarget("terrainShapeEnd")]
                public NumericParser<Double> terrainShapeEnd
                {
                    get { return mod.terrainShapeEnd; }
                    set { mod.terrainShapeEnd = value; }
                }

                // terrainShapeStart
                [ParserTarget("terrainShapeStart")]
                public NumericParser<Double> terrainShapeStart
                {
                    get { return mod.terrainShapeStart; }
                    set { mod.terrainShapeStart = value; }
                }

                // terrainSmoothing
                [ParserTarget("terrainSmoothing")]
                public NumericParser<Double> terrainSmoothing
                {
                    get { return mod.terrainSmoothing; }
                    set { mod.terrainSmoothing = value; }
                }

                // terrainType
                [ParserTarget("TerrainTypeSimplex", AllowMerge = true)]
                public SimplexLoader terrainType
                {
                    get { return mod.terrainType; }
                    set { mod.terrainType = value; }
                }

                // Creates the a PQSMod of type T with given PQS
                public override void Create(PQS pqsVersion)
                {
                    base.Create(pqsVersion);
                    
                    // Create mod components
                    continental = new SimplexLoader();
                    continentalRuggedness = new SimplexLoader();
                    continentalSharpness = new NoiseModLoader();
                    continentalSharpnessMap = new SimplexLoader();
                    terrainType = new SimplexLoader();
                        
                    // Create the callback list
                    landClasses = new CallbackList<LandClassLoader> ((e) =>
                    {
                        mod.landClasses = landClasses.Where(landClass => !landClass.delete)
                            .Select(landClass => landClass.Value).ToArray();
                    });
                    mod.landClasses = new PQSMod_VertexPlanet.LandClass[0];
                }

                // Grabs a PQSMod of type T from a parameter with a given PQS
                public override void Create(PQSMod_VertexPlanet _mod, PQS pqsVersion)
                {
                    base.Create(_mod, pqsVersion);
                    
                    // Create mod components
                    if (continental == null)
                    {
                        continental = new SimplexLoader();
                    }
                    else if (mod.continental.GetType() == typeof(PQSMod_VertexPlanet.SimplexWrapper))
                    {
                        continental = new SimplexLoader(mod.continental);
                    }
                    if (continentalRuggedness == null)
                    {
                        continentalRuggedness = new SimplexLoader();
                    }
                    else if (mod.continentalRuggedness.GetType() == typeof(PQSMod_VertexPlanet.SimplexWrapper))
                    {
                        continentalRuggedness = new SimplexLoader(mod.continentalRuggedness);
                    }
                    if (continentalSharpness == null)
                    {
                        continentalSharpness = new NoiseModLoader();
                    }
                    if (continentalSharpnessMap == null)
                    {
                        continentalSharpnessMap = new SimplexLoader();
                    }
                    else if (mod.continentalSharpnessMap.GetType() == typeof(PQSMod_VertexPlanet.SimplexWrapper))
                    {
                        continentalSharpnessMap = new SimplexLoader(mod.continentalSharpnessMap);
                    }
                    if (terrainType == null)
                    {
                        terrainType = new SimplexLoader();
                    }
                    else if (mod.terrainType.GetType() == typeof(PQSMod_VertexPlanet.SimplexWrapper))
                    {
                        terrainType = new SimplexLoader(mod.terrainType);
                    }
                    
                    // Create the callback list
                    landClasses = new CallbackList<LandClassLoader> ((e) =>
                    {
                        mod.landClasses = landClasses.Where(landClass => !landClass.delete)
                            .Select(landClass => landClass.Value).ToArray();
                    });
                    
                    // Load LandClasses
                    if (mod.landClasses != null)
                    {
                        for (Int32 i = 0; i < mod.landClasses.Length; i++)
                        {
                            // Only activate the callback if we are adding the last loader
                            landClasses.Add(new LandClassLoader(mod.landClasses[i]), i == mod.landClasses.Length - 1);
                        }
                    }
                    else
                    {
                        mod.landClasses = new PQSMod_VertexPlanet.LandClass[0];
                    }
                }
            }
        }
    }
}

