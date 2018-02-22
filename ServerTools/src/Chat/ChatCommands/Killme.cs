﻿using System;

namespace ServerTools
{
    public class KillMe
    {
        public static bool IsEnabled = false;
        public static int DelayBetweenKillMe = 60;

        public static void CheckPlayer(ClientInfo _cInfo, bool _announce)
        {
            if (DelayBetweenKillMe < 1)
            {
                KillPlayer(_cInfo);
            }
            else
            {
                Player p = PersistentContainer.Instance.Players[_cInfo.playerId, false];
                if (p == null || p.LastKillme == null)
                {
                    KillPlayer(_cInfo);
                }
                else
                {
                    TimeSpan varTime = DateTime.Now - p.LastKillme;
                    double fractionalMinutes = varTime.TotalMinutes;
                    int _timepassed = (int)fractionalMinutes;
                    if (_timepassed >= DelayBetweenKillMe)
                    {
                        KillPlayer(_cInfo);
                    }
                    else
                    {
                        int _timeremaining = DelayBetweenKillMe - _timepassed;
                        string _phrase8;
                        if (!Phrases.Dict.TryGetValue(8, out _phrase8))
                        {
                            _phrase8 = "{PlayerName} you can only use /killme once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                        }
                        _phrase8 = _phrase8.Replace("{PlayerName}", _cInfo.playerName);
                        _phrase8 = _phrase8.Replace("{DelayBetweenUses}", DelayBetweenKillMe.ToString());
                        _phrase8 = _phrase8.Replace("{TimeRemaining}", _timeremaining.ToString());
                        if (_announce)
                        {
                            GameManager.Instance.GameMessageServer((ClientInfo)null, EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.ChatResponseColor, _phrase8), "Server", false, "", false);
                        }
                        else
                        {
                            _cInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, string.Format("{0}{1}[-]", Config.ChatResponseColor, _phrase8), "Server", false, "", false));
                        }
                    }
                }
            }
        }

        private static void KillPlayer(ClientInfo _cInfo)
        {
            SdtdConsole.Instance.ExecuteSync(string.Format("kill {0}", _cInfo.entityId), (ClientInfo)null);
            PersistentContainer.Instance.Players[_cInfo.playerId, true].LastKillme = DateTime.Now;
            PersistentContainer.Instance.Save();
        }
    }
}