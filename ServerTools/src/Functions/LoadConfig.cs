﻿using System.IO;
using System.Xml;

namespace ServerTools
{
    public class Config
    {
        private const string configFile = "ServerToolsConfig.xml";
        private static string configFilePath = string.Format("{0}/{1}", API.ConfigPath, configFile);
        private static FileSystemWatcher fileWatcher = new FileSystemWatcher(API.ConfigPath, configFile);
        public const double version = 6.5;
        public static bool UpdateConfigs = false;
        public static string ChatResponseColor = "[00FF00]";

        public static void Load()
        {
            LoadXml();
            InitFileWatcher();
        }

        private static void LoadXml()
        {
            if (!Utils.FileExists(configFilePath))
            {
                UpdateXml();
                Phrases.Load();
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(configFilePath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", configFilePath, e.Message));
                return;
            }
            XmlNode _XmlNode = xmlDoc.DocumentElement;
            foreach (XmlNode childNode in _XmlNode.ChildNodes)
            {
                if (childNode.Name == "Version")
                {
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Tools' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("Version"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Version entry because of missing 'Version' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        double _oldversion;
                        if (!double.TryParse(_line.GetAttribute("Version"), out _oldversion))
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Ignoring Version entry because of invalid (non-numeric) value for 'Version' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (_oldversion != version)
                        {
                            UpdateConfigs = true;
                        }
                    }
                }
                if (childNode.Name == "Tools")
                {
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Tools' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("Name"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring tool entry because of missing 'Name' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        switch (_line.GetAttribute("Name"))
                        {
                            case "AdminChatCommands":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminChatCommands entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out AdminChat.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminChatCommands entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "AdminList":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out AdminList.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out AdminList.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ModLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of missing 'ModLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("ModLevel"), out AdminList.ModLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminList entry because of invalid (non-numeric) value for 'ModLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "AdminNameColoring":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.AdminNameColoring))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out ChatHook.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminPrefix"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'AdminPrefix' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminColor"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'AdminColor' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ModeratorLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'ModeratorLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("ModeratorLevel"), out ChatHook.ModLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of invalid (non-numeric) value for 'ModeratorLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ModeratorPrefix"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'ModeratorPrefix' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ModeratorColor"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AdminNameColoring entry because of missing 'ModeratorColor' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }                              
                                ChatHook.AdminPrefix = _line.GetAttribute("AdminPrefix");
                                ChatHook.AdminColor = _line.GetAttribute("AdminColor");
                                ChatHook.ModPrefix = _line.GetAttribute("ModeratorPrefix");
                                ChatHook.ModColor = _line.GetAttribute("ModeratorColor");
                                break;
                            case "AnimalTracking":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Animals.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenUses"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of missing 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AlwaysShowResponse"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of missing 'AlwaysShowResponse' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("AlwaysShowResponse"), out Animals.AlwaysShowResponse))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of invalid (true/false) value for 'AlwaysShowResponse' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenUses"), out Animals.DelayBetweenUses))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of invalid (non-numeric) value for 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("MinimumSpawnRadius"), out Animals.MinimumSpawnRadius))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of invalid (non-numeric) value for 'MinimumSpawnRadius' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("MaximumSpawnRadius"), out Animals.MaximumSpawnRadius))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of invalid (non-numeric) value for 'MaximumSpawnRadius' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("EntityId"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AnimalTracking entry because of missing 'EntityId' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                Animals.animalList = _line.GetAttribute("EntityId");
                                break;
                            case "AnnounceInvalidItemStack":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemStack entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out InventoryCheck.AnounceInvalidStack))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemStack entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break; 
                            case "AutoSaveWorld":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoSaveWorld entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out AutoSaveWorld.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoSaveWorld entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenWorldSaves"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoSaveWorld entry because of missing 'DelayBetweenWorldSaves' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenWorldSaves"), out AutoSaveWorld.DelayBetweenWorldSaves))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoSaveWorld entry because of invalid (non-numeric) value for 'DelayBetweenWorldSaves' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "AutoShutdown":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out AutoShutdown.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("CountdownTimer"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of missing 'CountdownTimer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("CountdownTimer"), out AutoShutdown.CountdownTimer))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of invalid (non-numeric) value for 'CountdownTimer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("TimeBeforeShutdown"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of missing 'TimeBeforeShutdown' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("TimeBeforeShutdown"), out AutoShutdown.TimeBeforeShutdown))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AutoShutdown entry because of invalid (non-numeric) value for 'TimeBeforeShutdown' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "BadWordFilter":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring BadWordFilter entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Badwords.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring BadWordFilter entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Bloodmoon":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Bloodmoon.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ShowOnSpawn"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of missing 'ShowOnSpawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ShowOnSpawn"), out Bloodmoon.ShowOnSpawn))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of invalid (true/false) value for 'ShowOnSpawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ShowOnRespawn"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of missing 'ShowOnRespawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ShowOnRespawn"), out Bloodmoon.ShowOnRespawn))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of invalid (true/false) value for 'ShowOnRespawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AutoShowBloodmoonDelay"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of missing 'AutoShowBloodmoonDelay' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AutoShowBloodmoonDelay"), out Bloodmoon.AutoShowBloodmoon))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of invalid (non-numeric) value for 'AutoShowBloodmoonDelay' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DaysUntilHorde"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of missing 'DaysUntilHorde' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DaysUntilHorde"), out Bloodmoon.DaysUntilHorde))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Bloodmoon entry because of invalid (non-numeric) value for 'DaysUntilHorde' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "ChatCommandPrivate":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatCommandPrivate entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.ChatCommandPrivateEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatCommandPrivate entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (ChatHook.ChatCommandPrivateEnabled)
                                {
                                    ChatHook.commandPrivate = _line.GetAttribute("Symbol");
                                }
                                break;
                            case "ChatCommandPublic":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatCommandPublic entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.ChatCommandPublicEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatCommandPublic entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (ChatHook.ChatCommandPublicEnabled)
                                {
                                    ChatHook.commandPublic = _line.GetAttribute("Symbol");
                                }
                                break;
                            case "ChatFloodProtection":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatFloodProtection entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.ChatFlood))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatFloodProtection entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "ChatLogger":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatLogger entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatLog.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatLogger entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "ClanManager":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ClanManager entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ClanManager.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ClanManager entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "ChatResponseColor":
                                if (!_line.HasAttribute("Color"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ChatColor entry because of missing 'Color' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                Config.ChatResponseColor = _line.GetAttribute("Color");
                                break;
                            case "CustomCommands":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring CustomCommands entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out CustomCommands.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring CustomCommands entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Day7":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Day7 entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Day7.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Day7 entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DaysUntilHorde"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Day7 entry because of missing 'DaysUntilHorde' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DaysUntilHorde"), out Day7.DaysUntilHorde))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Day7 entry because of invalid (non-numeric) value for 'DaysUntilHorde' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "DonatorNameColoring":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.DonatorNameColoring))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ReservedSlots.DonatorNameColoring))
                                {
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DonatorLevel1"), out ChatHook.DonLevel1))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of invalid (non-numeric) value for 'DonatorLevel1' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorPrefix1"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorPrefix1' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorColor1"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorColor1' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DonatorLevel2"), out ChatHook.DonLevel2))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of invalid (non-numeric) value for 'DonatorLevel2' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorPrefix2"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorPrefix2' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorColor2"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorColor2' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DonatorLevel3"), out ChatHook.DonLevel3))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of invalid (non-numeric) value for 'DonatorLevel3' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorPrefix3"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorPrefix3' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DonatorColor3"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring DonatorNameColoring entry because of missing 'DonatorColor3' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                ChatHook.DonPrefix1 = _line.GetAttribute("DonatorPrefix1");
                                ChatHook.DonPrefix2 = _line.GetAttribute("DonatorPrefix2");
                                ChatHook.DonPrefix3 = _line.GetAttribute("DonatorPrefix3");
                                ChatHook.DonColor1 = _line.GetAttribute("DonatorColor1");
                                ChatHook.DonColor2 = _line.GetAttribute("DonatorColor2");
                                ChatHook.DonColor3 = _line.GetAttribute("DonatorColor3");
                                break;
                            case "EntityUndergroundCheck":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring EntityUndergroundCheck entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out EntityUnderground.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring EntityUndergroundCheck entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AlertAdmin"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring AlertAdmin entry because of missing 'AlertAdmin' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("AlertAdmin"), out EntityUnderground.AlertAdmin))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring EntityUndergroundCheck entry because of invalid (true/false) value for 'AlertAdmin' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring EntityUndergroundCheck entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out EntityUnderground.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring EntityUndergroundCheck entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "FamilyShareAccount":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FamilyShareAccount entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out FamilyShareAccount.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FamilyShareAccount entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FamilyShareAccount entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out FamilyShareAccount.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FamilyShareAccount entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "FirstClaimBlock":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FirstClaimBlock entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out FirstClaimBlock.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FirstClaimBlock entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "FlightCheck":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out FlightCheck.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out FlightCheck.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("MaxPing"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'MaxPing' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("MaxPing"), out FlightCheck.MaxPing))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (non-numeric) value for 'MaxPing' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("MaxHeight"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'MaxHeight' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("MaxHeight"), out FlightCheck.MaxHeight))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (non-numeric) value for 'MaxHeight' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KillPlayer"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'KillPlayer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KillPlayer"), out FlightCheck.KillPlayer))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'KillPlayer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Announce"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'Announce' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Announce"), out FlightCheck.Announce))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'Announce' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("JailEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("JailEnabled"), out FlightCheck.JailEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KickEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KickEnabled"), out FlightCheck.KickEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("BanEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("BanEnabled"), out FlightCheck.BanEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (true/false) value for 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DaysBeforeLogDelete"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of missing 'DaysBeforeLogDelete' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DaysBeforeLogDelete"), out FlightCheck.DaysBeforeLogDelete))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring FlightCheck entry because of invalid (non-numeric) value for 'DaysBeforeLogDelete' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Gimme":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Gimme entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Gimme.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Gimme entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenUses"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring gimme entry because of missing 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenUses"), out Gimme.DelayBetweenUses))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring gimme entry because of invalid (non-numeric) value for 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AlwaysShowResponse"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Gimme entry because of missing 'AlwaysShowResponse' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("AlwaysShowResponse"), out Gimme.AlwaysShowResponse))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Gimme entry because of invalid (true/false) value for 'AlwaysShowResponse' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "HatchElevatorDetector":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring HatchElevator entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out HatchElevator.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring HatchElevator entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "HighPingKicker":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring HighPingKicker entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out HighPingKicker.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring HighPingKicker entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("SamplesNeeded"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ping entry because of missing 'SamplesNeeded' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("SamplesNeeded"), out HighPingKicker.SamplesNeeded))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ping entry because of invalid (non-numeric) value for 'SamplesNeeded' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Maxping"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ping entry because of missing 'Maxping' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("Maxping"), out HighPingKicker.MAXPING))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ping entry because of invalid (non-numeric) value for 'maxping' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "InfoTicker":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                bool _old = InfoTicker.IsEnabled;
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out InfoTicker.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenMessages"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of missing 'DelayBetweenMessages' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenMessages"), out InfoTicker.DelayBetweenMessages))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of invalid (non-numeric) value for 'DelayBetweenMessages' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Random"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of missing 'Random' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Random"), out InfoTicker.Random))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InfoTicker entry because of invalid (true/false) value for 'Random' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "InvalidItemKicker":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out InventoryCheck.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Ban"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of missing 'Ban' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Ban"), out InventoryCheck.BanPlayer))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of invalid (true/false) value for 'Ban' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("LevelToIgnore"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of missing 'LevelToIgnore' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("LevelToIgnore"), out InventoryCheck.LevelToIgnore))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring InvalidItemKicker entry because of invalid (true/false) value for 'LevelToIgnore' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "JailCommands":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring JailCommands entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Jail.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring JailCommands entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("JailSize"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring JailCommands entry because of missing 'JailSize' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("JailSize"), out Jail.JailSize))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring JailCommands entry because of invalid (non-numeric) value for 'JailSize' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("JailPosition"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring JailCommands entry because of missing 'JailPosition' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                Jail.JailPosition = _line.GetAttribute("JailPosition");
                                break;
                            case "Killme":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring killme entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out KillMe.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring killme entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenKillme"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring killme entry because of missing 'DelayBetweenKillme' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenKillme"), out KillMe.DelayBetweenKillMe))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring killme entry because of invalid (non-numeric) value for 'DelayBetweenKillme' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Motd":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Motd entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Motd.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Motd entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ShowOnRespawn"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Motd entry because of missing 'ShowOnRespawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ShowOnRespawn"), out Motd.ShowOnRespawn))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Motd entry because of invalid (true/false) value for 'ShowOnRespawn' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "NewSpawnTele":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NewSpawnTele entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out NewSpawnTele.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NewSpawnTele entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("NewSpawnTelePosition"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NewSpawnTele entry because of missing a NewSpawnTelePosition attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                NewSpawnTele.NewSpawnTelePosition = _line.GetAttribute("NewSpawnTelePosition");
                                break;
                            case "NormalPlayerNameColoring":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NormalPlayerNameColoring entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.NormalPlayerNameColoring))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NormalPlayerNameColoring entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("NormalPlayerPrefix"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NormalPlayerNameColoring entry because of missing 'NormalPlayerPrefix' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("NormalPlayerColor"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring NormalPlayerNameColoring entry because of missing 'NormalPlayerColor' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                ChatHook.NormalPlayerPrefix = _line.GetAttribute("NormalPlayerPrefix");
                                ChatHook.NormalPlayerColor = _line.GetAttribute("NormalPlayerColor");
                                break;
                            case "PlayerLogs":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out PlayerLogs.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Interval"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'Interval' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("Interval"), out PlayerLogs.Interval))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (non-numeric) value for 'Interval' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Position"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'Position' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Position"), out PlayerLogs.Position))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (true/false) value for 'Position' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Inventory"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'Inventory' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Inventory"), out PlayerLogs.Inventory))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (true/false) value for 'Inventory' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Extra"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'Extra' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Extra"), out PlayerLogs.P_Data))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (true/false) value for 'Extra' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DaysBeforeDeleted"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of missing 'DaysBeforeDeleted' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DaysBeforeDeleted"), out PlayerLogs.DaysBeforeDeleted))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerLogs entry because of invalid (non-numeric) value for 'DaysBeforeDeleted' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "PlayerStatCheck":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out PlayerStatCheck.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out PlayerStatCheck.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KickEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of missing 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KickEnabled"), out PlayerStatCheck.KickEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of invalid (true/false) value for 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("BanEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of missing 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("BanEnabled"), out PlayerStatCheck.BanEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring PlayerStatCheck entry because of invalid (true/false) value for 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "ReservedSlots":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ReservedSlots entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ReservedSlots.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ReservedSlots entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ReservedCheck"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ReservedSlots entry because of missing 'ReservedCheck' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ReservedCheck"), out ChatHook.ReservedCheck))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ReservedSlots entry because of invalid (true/false) value for 'ReservedCheck' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "SetHome":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out TeleportHome.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("SetHome2Enabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of missing 'SetHome2Enabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("SetHome2Enabled"), out TeleportHome.SetHome2Enabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of invalid (true/false) value for 'SetHome2Enabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("SetHome2DonorOnly"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of missing 'SetHome2DonorOnly' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("SetHome2DonorOnly"), out TeleportHome.SetHome2DonorOnly))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenHomeUses"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of missing 'DelayBetweenHomeUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenHomeUses"), out TeleportHome.DelayBetweenUses))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of invalid (non-numeric) value for 'DelayBetweenHomeUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "SpecialPlayerNameColoring":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SpecialPlayerNameColoring entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ChatHook.SpecialPlayerNameColoring))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SpecialPlayerNameColoring entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("SpecialPlayerPrefix"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SpecialPlayerNameColoring entry because of missing 'SpecialPlayerPrefix' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                ChatHook.SpecialPlayerPrefix = _line.GetAttribute("SpecialPlayerPrefix");
                                if (!_line.HasAttribute("SpecialPlayerColor"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SpecialPlayerNameColoring entry because of missing 'SpecialPlayerColor' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                ChatHook.SpecialPlayerColor = _line.GetAttribute("SpecialPlayerColor");
                                if (!_line.HasAttribute("SpecialPlayerSteamId"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SpecialPlayerNameColoring entry because of missing 'SpecialPlayerSteamId' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                ChatHook.SpecialPlayersList = _line.GetAttribute("SpecialPlayerSteamId");
                                break;
                            case "StartingItems":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out StartingItems.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring SetHome entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Stopserver":
                                if (!_line.HasAttribute("TenSecondCountdownEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Stopserver entry because of missing 'TenSecondCountdownEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("TenSecondCountdownEnabled"), out StopServer.TenSecondCountdownEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Stopserver entry because of invalid (true/false) value for 'TenSecondCountdownEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "TempBan":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring TempBan entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out TempBan.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring TempBan entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out TempBan.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Travel":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Travel.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("DelayBetweenUses"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenUses"), out Travel.DelayBetweenUses))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of invalid (non-numeric) value for 'DelayBetweenUses' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "UndergroundCheck":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out UndergroundCheck.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("AdminLevel"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("AdminLevel"), out UndergroundCheck.AdminLevel))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (non-numeric) value for 'AdminLevel' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("MaxPing"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'MaxPing' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("MaxPing"), out UndergroundCheck.MaxPing))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (non-numeric) value for 'MaxPing' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KillPlayer"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'KillPlayer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KillPlayer"), out UndergroundCheck.KillPlayer))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'KillPlayer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("Announce"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'Announce' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Announce"), out UndergroundCheck.Announce))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'Announce' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("JailEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("JailEnabled"), out UndergroundCheck.JailEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KickEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KickEnabled"), out UndergroundCheck.KickEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("BanEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of missing 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("BanEnabled"), out UndergroundCheck.BanEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring UndergroundCheck entry because of invalid (true/false) value for 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Voting":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out VoteReward.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("YourVotingSite"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of missing 'YourVotingSite' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                VoteReward.YourVotingSite = _line.GetAttribute("YourVotingSite");
                                if (!_line.HasAttribute("APIKey"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of missing 'APIKey' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                VoteReward.APIKey = _line.GetAttribute("APIKey");
                                if (!_line.HasAttribute("DelayBetweenRewards"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of missing 'DelayBetweenRewards' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("DelayBetweenRewards"), out VoteReward.DelayBetweenRewards))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of invalid (non-numeric) value for 'DelayBetweenRewards' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("RewardCount"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of missing 'RewardCount' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("RewardCount"), out VoteReward.RewardCount))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Voting entry because of invalid (non-numeric) value for 'RewardCount' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "Watchlist":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Watchlist entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out Watchlist.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring Watchlist entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            case "WeatherVote":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WeatherVote entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out WeatherVote.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WeatherVote entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue; 
                                }
                                if (!_line.HasAttribute("VoteDelay"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WeatherVote entry because of missing 'VoteDelay' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("VoteDelay"), out WeatherVote.VoteDelay))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WeatherVote entry because of invalid (non-numeric) value for 'VoteDelay' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                            /*case "WorldRadius":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WorldRadius entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out WorldRadius.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WorldRadius entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("WorldSize"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WorldRadius entry because of missing 'WorldSize' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!int.TryParse(_line.GetAttribute("WorldSize"), out WorldRadius.WorldSize))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WorldRadius entry because of invalid (non-numeric) value for 'WorldSize' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ZoneWarnings"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring WorldRadius entry because of missing 'ZoneWarnings' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ZoneWarnings"), out WorldRadius.ZoneWarnings))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;*/
                            case "ZoneProtection":
                                if (!_line.HasAttribute("Enable"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("Enable"), out ZoneProtection.IsEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KillMurderer"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'KillMurderer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KillMurderer"), out ZoneProtection.KillMurderer))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'KillMurderer' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("JailEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("JailEnabled"), out ZoneProtection.JailEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'JailEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("KickEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("KickEnabled"), out ZoneProtection.KickEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'KickEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("BanEnabled"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("BanEnabled"), out ZoneProtection.BanEnabled))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'BanEnabled' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("ZoneMessage"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'ZoneMessage' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("ZoneMessage"), out ZoneProtection.ZoneMessage))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'ZoneMessage' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!_line.HasAttribute("SetHome"))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of missing 'SetHome' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                if (!bool.TryParse(_line.GetAttribute("SetHome"), out ZoneProtection.SetHome))
                                {
                                    Log.Warning(string.Format("[SERVERTOOLS] Ignoring ZoneProtection entry because of invalid (true/false) value for 'SetHome' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                break;
                        }
                    }
                }
            }
            if (UpdateConfigs)
            {
                UpdateXml();
            }
            Phrases.Load();
            Mods.Load();
            UpdateConfigs = false;
        }

        public static void UpdateXml()
        {
            fileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(configFilePath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<ServerTools>");
                sw.WriteLine("    <Version>");
                sw.WriteLine(string.Format("        <Version Version=\"{0}\" />", version.ToString()));
                sw.WriteLine("    </Version>");
                sw.WriteLine("    <Tools>");
                sw.WriteLine(string.Format("        <Tool Name=\"AdminChatCommands\" Enable=\"{0}\" />", AdminChat.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"AdminList\" Enable=\"{0}\" AdminLevel=\"{1}\" ModLevel=\"{2}\" />", AdminList.IsEnabled, AdminList.AdminLevel, AdminList.ModLevel));
                sw.WriteLine(string.Format("        <Tool Name=\"AdminNameColoring\" Enable=\"{0}\" AdminLevel=\"{1}\" AdminPrefix=\"{2}\" AdminColor=\"{3}\" ModeratorLevel=\"{4}\" ModeratorPrefix=\"{5}\" ModeratorColor=\"{6}\" />", ChatHook.AdminNameColoring, ChatHook.AdminLevel, ChatHook.AdminPrefix, ChatHook.AdminColor, ChatHook.ModLevel, ChatHook.ModPrefix, ChatHook.ModColor));
                sw.WriteLine(string.Format("        <Tool Name=\"AnimalTracking\" Enable=\"{0}\" AlwaysShowResponse=\"{1}\" DelayBetweenUses=\"{2}\" MinimumSpawnRadius=\"{3}\" MaximumSpawnRadius=\"{4}\" EntityId=\"{5}\" />", Animals.IsEnabled, Animals.AlwaysShowResponse, Animals.DelayBetweenUses, Animals.MinimumSpawnRadius, Animals.MaximumSpawnRadius, Animals.animalList));
                sw.WriteLine(string.Format("        <Tool Name=\"AnnounceInvalidItemStack\" Enable=\"{0}\" />", InventoryCheck.AnounceInvalidStack));
                sw.WriteLine(string.Format("        <Tool Name=\"AutoSaveWorld\" Enable=\"{0}\" DelayBetweenWorldSaves=\"{1}\" />", AutoSaveWorld.IsEnabled, AutoSaveWorld.DelayBetweenWorldSaves));
                sw.WriteLine(string.Format("        <Tool Name=\"AutoShutdown\" Enable=\"{0}\" CountdownTimer=\"{1}\" TimeBeforeShutdown=\"{2}\" />", AutoShutdown.IsEnabled, AutoShutdown.CountdownTimer, AutoShutdown.TimeBeforeShutdown));
                sw.WriteLine(string.Format("        <Tool Name=\"BadWordFilter\" Enable=\"{0}\" />", Badwords.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"Bloodmoon\" Enable=\"{0}\" ShowOnSpawn=\"{1}\" ShowOnRespawn=\"{2}\" AutoShowBloodmoonDelay=\"{3}\" DaysUntilHorde=\"{4}\" />", Bloodmoon.IsEnabled, Bloodmoon.ShowOnSpawn, Bloodmoon.ShowOnRespawn, Bloodmoon.AutoShowBloodmoon, Bloodmoon.DaysUntilHorde));
                sw.WriteLine(string.Format("        <Tool Name=\"ChatResponseColor\" Color=\"{0}\" />", Config.ChatResponseColor));
                sw.WriteLine(string.Format("        <Tool Name=\"ChatCommandPrivate\" Enable=\"{0}\" Symbol=\"{1}\" />", ChatHook.ChatCommandPrivateEnabled, ChatHook.commandPrivate));
                sw.WriteLine(string.Format("        <Tool Name=\"ChatCommandPublic\" Enable=\"{0}\" Symbol=\"{1}\" />", ChatHook.ChatCommandPublicEnabled, ChatHook.commandPublic));
                sw.WriteLine(string.Format("        <Tool Name=\"ChatFloodProtection\" Enable=\"{0}\" />", ChatHook.ChatFlood));
                sw.WriteLine(string.Format("        <Tool Name=\"ChatLogger\" Enable=\"{0}\" />", ChatLog.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"ClanManager\" Enable=\"{0}\" />", ClanManager.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"CustomCommands\" Enable=\"{0}\" />", CustomCommands.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"Day7\" Enable=\"{0}\" DaysUntilHorde=\"{1}\" />", Day7.IsEnabled, Day7.DaysUntilHorde));
                sw.WriteLine(string.Format("        <Tool Name=\"DonatorNameColoring\" Enable=\"{0}\" DonatorLevel1=\"{1}\" DonatorLevel2=\"{2}\" DonatorLevel3=\"{3}\" DonatorPrefix1=\"{4}\" DonatorPrefix2=\"{5}\" DonatorPrefix3=\"{6}\" DonatorColor1=\"{7}\" DonatorColor2=\"{8}\" DonatorColor3=\"{9}\" />", ChatHook.DonatorNameColoring && ReservedSlots.DonatorNameColoring, ChatHook.DonLevel1, ChatHook.DonLevel2, ChatHook.DonLevel3, ChatHook.DonPrefix1, ChatHook.DonPrefix2, ChatHook.DonPrefix3, ChatHook.DonColor1, ChatHook.DonColor2, ChatHook.DonColor3));
                sw.WriteLine(string.Format("        <Tool Name=\"EntityUndergroundCheck\" Enable=\"{0}\" AlertAdmin=\"{1}\" AdminLevel=\"{2}\" />", EntityUnderground.IsEnabled, EntityUnderground.AlertAdmin, EntityUnderground.AdminLevel));
                sw.WriteLine(string.Format("        <Tool Name=\"FamilyShareAccount\" Enable=\"{0}\" AdminLevel=\"{1}\" />", FamilyShareAccount.IsEnabled, FamilyShareAccount.AdminLevel));
                sw.WriteLine(string.Format("        <Tool Name=\"FirstClaimBlock\" Enable=\"{0}\" />", FirstClaimBlock.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"FlightCheck\" Enable=\"{0}\" AdminLevel=\"{1}\" MaxPing=\"{2}\" MaxHeight=\"{3}\" KillPlayer=\"{4}\" Announce=\"{5}\" JailEnabled=\"{6}\" KickEnabled=\"{7}\" BanEnabled=\"{8}\" DaysBeforeLogDelete=\"{9}\" />", FlightCheck.IsEnabled, FlightCheck.AdminLevel, FlightCheck.MaxPing, FlightCheck.MaxHeight, FlightCheck.KillPlayer, FlightCheck.Announce, FlightCheck.JailEnabled, FlightCheck.KickEnabled, FlightCheck.BanEnabled, FlightCheck.DaysBeforeLogDelete));
                sw.WriteLine(string.Format("        <Tool Name=\"Gimme\" Enable=\"{0}\" DelayBetweenUses=\"{1}\" AlwaysShowResponse=\"{2}\" />", Gimme.IsEnabled, Gimme.DelayBetweenUses, Gimme.AlwaysShowResponse));
                sw.WriteLine(string.Format("        <Tool Name=\"HatchElevatorDetector\" Enable=\"{0}\" />", HatchElevator.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"HighPingKicker\" Enable=\"{0}\" Maxping=\"{1}\" SamplesNeeded=\"{2}\" />", HighPingKicker.IsEnabled, HighPingKicker.MAXPING, HighPingKicker.SamplesNeeded));
                sw.WriteLine(string.Format("        <Tool Name=\"InfoTicker\" Enable=\"{0}\" DelayBetweenMessages=\"{1}\" Random=\"{2}\" />", InfoTicker.IsEnabled, InfoTicker.DelayBetweenMessages, InfoTicker.Random));
                sw.WriteLine(string.Format("        <Tool Name=\"InvalidItemKicker\" Enable=\"{0}\" Ban=\"{1}\" LevelToIgnore=\"{2}\" />", InventoryCheck.IsEnabled, InventoryCheck.BanPlayer, InventoryCheck.LevelToIgnore));
                sw.WriteLine(string.Format("        <Tool Name=\"JailCommands\" Enable=\"{0}\" JailSize=\"{1}\" JailPosition=\"{2}\" />", Jail.IsEnabled, Jail.JailSize, Jail.JailPosition));
                sw.WriteLine(string.Format("        <Tool Name=\"Killme\" Enable=\"{0}\" DelayBetweenKillme=\"{1}\" />", KillMe.IsEnabled, KillMe.DelayBetweenKillMe));
                sw.WriteLine(string.Format("        <Tool Name=\"Motd\" Enable=\"{0}\" ShowOnRespawn=\"{1}\" />", Motd.IsEnabled, Motd.ShowOnRespawn));
                sw.WriteLine(string.Format("        <Tool Name=\"NewSpawnTele\" Enable=\"{0}\" NewSpawnTelePosition=\"{1}\" />", NewSpawnTele.IsEnabled, NewSpawnTele.NewSpawnTelePosition));
                sw.WriteLine(string.Format("        <Tool Name=\"NormalPlayerNameColoring\" Enable=\"{0}\" NormalPlayerPrefix=\"{1}\" NormalPlayerColor=\"{2}\" />", ChatHook.NormalPlayerNameColoring, ChatHook.NormalPlayerPrefix, ChatHook.NormalPlayerColor));
                sw.WriteLine(string.Format("        <Tool Name=\"PlayerLogs\" Enable=\"{0}\" Interval=\"{1}\" Position=\"{2}\" Inventory=\"{3}\" Extra=\"{4}\" DaysBeforeDeleted=\"{5}\" />", PlayerLogs.IsEnabled, PlayerLogs.Interval, PlayerLogs.Position, PlayerLogs.Inventory, PlayerLogs.P_Data, PlayerLogs.DaysBeforeDeleted));
                sw.WriteLine(string.Format("        <Tool Name=\"PlayerStatCheck\" Enable=\"{0}\" AdminLevel=\"{1}\" KickEnabled=\"{2}\" BanEnabled=\"{3}\" />", PlayerStatCheck.IsEnabled, PlayerStatCheck.AdminLevel, PlayerStatCheck.KickEnabled, PlayerStatCheck.BanEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"ReservedSlots\" Enable=\"{0}\" ReservedCheck=\"{1}\" />", ReservedSlots.IsEnabled, ChatHook.ReservedCheck));
                sw.WriteLine(string.Format("        <Tool Name=\"SetHome\" Enable=\"{0}\" SetHome2Enabled=\"{1}\" SetHome2DonorOnly=\"{2}\" DelayBetweenHomeUses=\"{3}\" />", TeleportHome.IsEnabled, TeleportHome.SetHome2Enabled, TeleportHome.SetHome2DonorOnly, TeleportHome.DelayBetweenUses));
                sw.WriteLine(string.Format("        <Tool Name=\"SpecialPlayerNameColoring\" Enable=\"{0}\" SpecialPlayerSteamId=\"{1}\" SpecialPlayerPrefix=\"{2}\" SpecialPlayerColor=\"{3}\" />", ChatHook.SpecialPlayerNameColoring, ChatHook.SpecialPlayersList, ChatHook.SpecialPlayerPrefix, ChatHook.SpecialPlayerColor));
                sw.WriteLine(string.Format("        <Tool Name=\"StartingItems\" Enable=\"{0}\" />", StartingItems.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"Stopserver\" TenSecondCountdownEnabled=\"{0}\" />", StopServer.TenSecondCountdownEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"TempBan\" Enable=\"{0}\" AdminLevel=\"{1}\" />", TempBan.IsEnabled, TempBan.AdminLevel));
                sw.WriteLine(string.Format("        <Tool Name=\"Travel\" Enable=\"{0}\" DelayBetweenUses=\"{1}\" />", Travel.IsEnabled, Travel.DelayBetweenUses));
                sw.WriteLine(string.Format("        <Tool Name=\"UndergroundCheck\" Enable=\"{0}\" AdminLevel=\"{1}\" MaxPing=\"{2}\" KillPlayer=\"{3}\" Announce=\"{4}\" JailEnabled=\"{5}\" KickEnabled=\"{6}\" BanEnabled=\"{7}\" DaysBeforeDeleted=\"{8}\" />", UndergroundCheck.IsEnabled, UndergroundCheck.AdminLevel, UndergroundCheck.MaxPing, UndergroundCheck.KillPlayer, UndergroundCheck.Announce, UndergroundCheck.JailEnabled, UndergroundCheck.KickEnabled, UndergroundCheck.BanEnabled, UndergroundCheck.DaysBeforeDeleted));
                sw.WriteLine(string.Format("        <Tool Name=\"Voting\" Enable=\"{0}\" YourVotingSite=\"{1}\" APIKey=\"{2}\" DelayBetweenRewards=\"{3}\" RewardCount=\"{4}\" />", VoteReward.IsEnabled, VoteReward.YourVotingSite, VoteReward.APIKey, VoteReward.DelayBetweenRewards, VoteReward.RewardCount));               
                sw.WriteLine(string.Format("        <Tool Name=\"Watchlist\" Enable=\"{0}\" />", Watchlist.IsEnabled));
                sw.WriteLine(string.Format("        <Tool Name=\"WeatherVote\" Enable=\"{0}\" VoteDelay=\"{1}\"/>", WeatherVote.IsEnabled, WeatherVote.VoteDelay));
                //sw.WriteLine(string.Format("        <Tool Name=\"WorldRadius\" Enable=\"{0}\" WorldSize=\"{1}\" ZoneWarnings=\"{2}\", WorldRadius.IsEnabled, WorldRadius.WorldSize, WorldRadius.ZoneWarnings));
                sw.WriteLine(string.Format("        <Tool Name=\"ZoneProtection\" Enable=\"{0}\" KillMurderer=\"{1}\" JailEnabled=\"{2}\" KickEnabled=\"{3}\" BanEnabled=\"{4}\" ZoneMessage=\"{5}\" SetHome=\"{6}\" />", ZoneProtection.IsEnabled, ZoneProtection.KillMurderer, ZoneProtection.JailEnabled, ZoneProtection.KickEnabled, ZoneProtection.BanEnabled, ZoneProtection.ZoneMessage, ZoneProtection.SetHome));
                sw.WriteLine("    </Tools>");
                sw.WriteLine("</ServerTools>");
                sw.Flush();
                sw.Close();
            }
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!Utils.FileExists(configFilePath))
            {
                UpdateXml();
            }
            LoadXml();
        }
    }
}