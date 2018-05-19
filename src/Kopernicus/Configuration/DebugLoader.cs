/**
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

using System;
using Kopernicus.Components;
using Kopernicus.UI;
using UnityEngine;

namespace Kopernicus
{
    namespace Configuration
    {
        // Loads Debugging properties for a Body
        [RequireConfigType(ConfigType.Node)]
        public class DebugLoader : BaseLoader, IParserEventSubscriber, ITypeParser<CelestialBody>
        {
            // The Body we're editing
            public CelestialBody Value { get; set; }
            
            // If this is set to false, Kopernicus wont save a .bin file with the scaledSpace mesh 
            [ParserTarget("exportMesh")]
            [KittopiaDescription("Whether Kopernicus should save a .bin file with the ScaledSpace mesh.")]
            public NumericParser<Boolean> exportMesh
            {
                get { return Value.Get("exportMesh", true); }
                set { Value.Set("exportMesh", value.Value); }
            }

            // If this is set to true, Kopernicus will update the ScaledSpace mesh, even if the original conditions aren't matched
            [ParserTarget("update")]
            [KittopiaDescription("Setting this to true will force Kopernicus to update the ScaledSpace mesh.")]
            public NumericParser<Boolean> update
            {
                get { return Value.Get("update", false); }
                set { Value.Set("update", value.Value); }
            }

            // If this is set to true, a wireframe will appear to visualize the SOI
            [ParserTarget("showSOI")]
            [KittopiaHideOption]
            public NumericParser<Boolean> showSOI
            {
                get { return Value.Get("showSOI", false); }
                set
                {
                    Value.Set("showSOI", value.Value);
                    if (value)
                    {
                        Value.gameObject.AddComponent<Wiresphere>();
                    }
                }
            }

            /// <summary>
            /// Toggles the wireframe that gets added to the planet to show it's sphere of influence
            /// </summary>
            [KittopiaAction("Toggle SOI Visibility")]
            [KittopiaDescription("Visualizes the SOI of the planet.")]
            public void ShowSOI()
            {
                Value.Set("showSOI", !Value.Get("showSOI", false));
                if (Value.Get("showSOI", false))
                {
                    Value.gameObject.AddComponent<Wiresphere>();
                }
                else
                {
                    UnityEngine.Object.Destroy(Value.GetComponent<Wiresphere>());
                }
            }

            // Parser apply event
            void IParserEventSubscriber.Apply(ConfigNode node)
            {
                Events.OnDebugLoaderApply.Fire(this, node);
            }

            // Parser post apply event
            void IParserEventSubscriber.PostApply(ConfigNode node)
            {
                Events.OnDebugLoaderPostApply.Fire(this, node);
            }

            /// <summary>
            /// Creates a new Debug Loader from the Injector context.
            /// </summary>
            public DebugLoader()
            {
                // Is this the parser context?
                if (!Injector.IsInPrefab)
                {
                    throw new InvalidOperationException("Must be executed in Injector context.");
                }

                // Store values
                Value = generatedBody.celestialBody;
            }

            /// <summary>
            /// Creates a new Debug Loader from a spawned CelestialBody.
            /// </summary>
            [KittopiaConstructor(KittopiaConstructor.Parameter.CelestialBody)]
            public DebugLoader(CelestialBody body)
            {
                // Is this a spawned body?
                if (body?.scaledBody == null || Injector.IsInPrefab)
                {
                    throw new InvalidOperationException("The body must be already spawned by the PSystemManager.");
                }

                // Store values
                Value = body;
            }
        }
    }
}

