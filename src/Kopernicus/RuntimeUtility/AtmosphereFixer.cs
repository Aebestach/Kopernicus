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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kopernicus
{
    public class AFGInfo
    {
        public static Dictionary<String, AFGInfo> atmospheres = new Dictionary<String, AFGInfo>();
        
        Boolean DEBUG_alwaysUpdateAll;
        Boolean doScale;
        Single outerRadius;
        Single innerRadius;
        Single ESun;
        Single Kr;
        Single Km;
        Vector3 transformScale;
        Single scaleDepth;
        Single samples;
        Single g;
        Color waveLength;
        Color invWaveLength;

        public static Boolean StoreAFG(AtmosphereFromGround afg)
        {
            if (afg.planet == null)
            {
                Debug.Log("[Kopernicus] Trying to store AFG, but planet null!");
                return false;
            }
            atmospheres[afg.planet.transform.name] = new AFGInfo(afg);
            return true;
        }

        public static Boolean UpdateAFGName(String oName, String nName)
        {
            AFGInfo info = null;
            if (atmospheres.TryGetValue(oName, out info))
            {
                atmospheres.Remove(oName);
                atmospheres[nName] = info;
                return true;
            }
            return false;
        }

        public static Boolean PatchAFG(AtmosphereFromGround afg)
        {
            AFGInfo info = null;
            if (atmospheres.TryGetValue(afg.planet.transform.name, out info))
            {
                try
                {
                    info.Apply(afg);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public AFGInfo(AtmosphereFromGround afg)
        {
            DEBUG_alwaysUpdateAll = afg.DEBUG_alwaysUpdateAll;
            doScale = afg.doScale;
            ESun = afg.ESun;
            Kr = afg.Kr;
            Km = afg.Km;
            transformScale = afg.transform.localScale;
            scaleDepth = afg.scaleDepth;
            samples = afg.samples;
            g = afg.g;
            waveLength = afg.waveLength;
            invWaveLength = afg.invWaveLength;
            outerRadius = afg.outerRadius;
            innerRadius = afg.innerRadius;
        }

        public void Apply(AtmosphereFromGround afg)
        {
            afg.DEBUG_alwaysUpdateAll = DEBUG_alwaysUpdateAll;
            afg.doScale = doScale;
            afg.ESun = ESun;
            afg.Kr = Kr;
            afg.Km = Km;
            afg.transform.localScale = transformScale;
            afg.scaleDepth = scaleDepth;
            afg.samples = samples;
            afg.g = g;
            afg.waveLength = waveLength;
            afg.invWaveLength = invWaveLength;
            afg.outerRadius = outerRadius;
            afg.innerRadius = innerRadius;
            afg.transform.localPosition = Vector3.zero;

            Configuration.AtmosphereFromGroundLoader.CalculatedMembers(afg);
            afg.SetMaterial(true);

            Events.OnRuntimeUtilityPatchAFG.Fire(afg);
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class AtmosphereFixer : MonoBehaviour
    {
        Double timeCounter = 0d;
        void Awake()
        {
            if (!CompatibilityChecker.IsCompatible())
            {
                Destroy(this);
                return;
            }

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                if (FlightGlobals.GetHomeBody()?.atmosphericAmbientColor != null)
                    RenderSettings.ambientLight = FlightGlobals.GetHomeBody().atmosphericAmbientColor;
            }
        }

        void Start()
        {
            if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                return;
            }
            Destroy(this); // don't hang around.
        }

        void Update()
        {
            if (timeCounter < 0.5d)
            {
                timeCounter += Time.deltaTime;
                return;
            }
            foreach (AtmosphereFromGround afg in Resources.FindObjectsOfTypeAll<AtmosphereFromGround>())
            {
                if (afg.planet != null)
                {
                    //Debug.Log("[Kopernicus]: Patching AFG " + afg.planet.bodyName);
                    //if (!AFGInfo.PatchAFG(afg))
                    //    Debug.Log("[Kopernicus]: ERROR AtmosphereFixer => Couldn't patch AtmosphereFromGround for " + afg.planet.bodyName + "!");
                    if (AFGInfo.PatchAFG(afg))
                        Debug.Log("[Kopernicus] AtmosphereFixer => Patched AtmosphereFromGround for " + afg.planet.bodyName);
                }
            }
            UnityEngine.Object.Destroy(this); // don't hang around.
        }
    }
}
