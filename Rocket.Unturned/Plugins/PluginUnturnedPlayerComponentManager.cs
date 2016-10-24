﻿using Rocket.API;
using Rocket.API.Extensions;
using Rocket.API.Plugins;
using Rocket.Core.Extensions;
using Logger = Rocket.API.Logging.Logger;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Rocket.Unturned.Plugins
{
    public sealed class PluginUnturnedPlayerComponentManager : MonoBehaviour
    {
        private Assembly assembly;
        private List<Type> unturnedPlayerComponents = new List<Type>();
        
        private void OnDisable()
        {
            try
            {
                U.Instance.OnPlayerConnected -= addPlayerComponents;
                unturnedPlayerComponents = unturnedPlayerComponents.Where(p => p.Assembly != assembly).ToList();
                List<Type> playerComponents = assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent));
                foreach (Type playerComponent in playerComponents)
                {
                    //Provider.Players.ForEach(p => p.Player.gameObject.TryRemoveComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnEnable()
        {
            try
            {  
                IRocketPlugin plugin = GetComponent<IRocketPlugin>();
                assembly = plugin.GetType().Assembly;

                U.Events.OnBeforePlayerConnected += addPlayerComponents;
                unturnedPlayerComponents.AddRange(assembly.GetTypesFromParentClass(typeof(UnturnedPlayerComponent)));

                foreach (Type playerComponent in unturnedPlayerComponents)
                {
                    Logger.Info("Adding UnturnedPlayerComponent: "+playerComponent.Name);
                    //Provider.Players.ForEach(p => p.Player.gameObject.TryAddComponent(playerComponent.GetType()));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void addPlayerComponents(IRocketPlayer p)
        {
            foreach (Type component in unturnedPlayerComponents)
            {
                ((UnturnedPlayer)p).Player.gameObject.AddComponent(component);
            }
        }
    }
}