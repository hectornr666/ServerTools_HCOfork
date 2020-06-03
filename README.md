# 7dtd-ServerTools
This version is based on OC's well-known Server tools for 7 days to die dedicated servers<br>
<br>
# Installation
<br>
Since this is only a modifcation of ServerTools, first install latest files of ServerTools from OC's publib GitHub repo: https://github.com/dmustanger/7dtd-ServerTools/releases <br>
<br>The mod will keep all existing config files and will work with ServerTools 18.3.1.<br>
<br>
Replace the ServerTools.dll with my files from:
https://github.com/hectornr666/ServerTools_HCOfork/releases
<br>
<h1>Editing the ServerToolsConfig.xml</h1>
<b>AGAIN: The mod will keep all existing config files and will work with ServerTools 18.3.1.</b>
<br><b>Update</b> following line:
<br><b>Tool Name="Damage_Detector" Enable="True"</b><br>
Tool Name="Damage_Detector" Enable="True" Entity_Damage_Limit="500" Block_Damage_Limit="3000" Admin_Level="0" ReportOnHit="False" ReportOnKill="False" IngamePrefix="[00AE00][DMG-RPRT]:" SendToGlobalChat="True" SendToDiscordIfDCenabled="True" Discord_PostingFormat="2"/>

<br><br><b>Add</b> following lines to your existing 
<br>
Tool Name="DiscordConnect" Enable="True" Webhook_Key="https://discordapp.com/api/webhooks/<KEY>" SpamChat="True" Discord_PostingFormat="4" SendServerToolsMSG="True" />

<br><br><b>Remove or disable</b>:
<br>Tool Name="Kill_Notice" Enable="False" />
<br>
<b>Invite the Discord Bot -> HectoBOT
https://discordapp.com/oauth2/authorize?client_id=372686901972303872&permissions=0&scope=bot
<br>
Start the server as you did already 10.000 times before.<br>
<br>
<br>
<br>
# Current-Tools-and-Features
<br>
<br>
<li>Discord messaging including 2-way chat & admins command bridge, HectoBot Discord's Bot Support<br>
<li>telegram messaging<br>
<li>reworked country ban<br>
<li>Added: Support API for GUI<br>
<li>Added (beta): telemetry & server benchmark data (hardwarestatistics)<br>
<br> 
<br>
<hr>
 Need support or got some advices ?-> Come to my Discord: https://discord.gg/tTaGtCW
