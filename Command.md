Help
---
These are the commands you can use with Miunie
<br/><br/>
[Admin](#admin)<br/>
[Announcements](#announcements)<br/>
[Welcome](#welcome)<br/>
[Leave](#leave)<br/>
[Basics](#basics)<br/>
[Blog](#blog)<br/>
[CommandInfoFileCreator](#commandinfofilecreator)<br/>
[Economy](#economy)<br/>
[Misc](#misc)<br/>
[Prefix](#prefix)<br/>
[Reminder](#reminder)<br/>
[Tasks](#tasks)<br/>
[Bots](#bots)<br/>
[ServerSetup](#serversetup)<br/>
[SetTimeZone](#settimezone)<br/>
[Tag](#tag)<br/>
[PersonalTags](#personaltags)<br/>
[Voice](#voice)<br/>
[RoleByPhrase](#rolebyphrase)<br/>
[Trivia](#trivia)<br/>
[account](#account)<br/>

<br/><br/>
### Admin




| Command | Description | Remarks |
| --- | --- | --- |
| `purge` |  | Purges An Amount Of Messages | |
| `kick` |  | Kick A User | |
| `mute` |  | Mutes A User | |
| `unmute` |  | Unmutes A User | |
| `ban` |  | Ban A User | |
| `unban` |  | Unban A User | |
| `nickname` |  | Set A User's Nickname | |
| `createtext` |  | Make A Text Channel | |
| `createvoice` |  | Make A Voice Channel | |
| `announce` |  | Make A Announcement | |
| `echo` |  | Make The Bot Say A Message | |
| `game` |  | Change what the bot is currently playing. | |
| `setavatar` |  | Sets the bots Avatar | |
<br/>

### Announcements
Settings for announcements<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `announcements setchannel` |  | Sets the channel where to post announcements | |
| `announcements unsetchannel` |  | Turns posting announcements to a channel off | |
<br/>

### Welcome

DM a joining user a random message out of the ones defined.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `welcome add` |  | Example: `welcome add <usermention>, welcome to **<guildname>**! Try using ```@<botname>#<botdiscriminator> help``` for all the commands of <botmention>!`<br/>Possible placeholders are: `<usermention>`, `<username>`, `<guildname>`, `<botname>`, `<botdiscriminator>`, `<botmention>`  | |
| `welcome remove` |  | Removes a Welcome Message from the ones availabe | |
| `welcome list` |  | Shows all currently set Welcome Messages | |
<br/>

### Leave

Announce a leaving user in the set announcement channelwith a random message out of the ones defined.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `leave add` |  | Example: `leave add Oh noo! <usermention>, left <guildname>...`<br/>Possible placeholders are: `<usermention>`, `<username>`, `<guildname>`, `<botname>`, `<botdiscriminator>`, `<botmention>` | |
| `leave remove` |  | Removes a Leave Message from the ones available | |
| `leave list` |  | Shows all currently set Leave Messages | |
<br/>

### Basics




| Command | Description | Remarks |
| --- | --- | --- |
| `hello` |  |  | |
<br/>

### Blog

Enables you to create a block that people can subscribe to so they don't miss out if you publish a new one<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `blog create` |  | Create a new named block | |
| `blog post` |  | Publish a new post to one of your named blocks | |
| `blog subscribe` |  | Subscribe to a named blog to receive a message when a new post gets published | |
| `blog unsubscribe` |  | Remove a subscription from a named block | |
<br/>

### CommandInfoFileCreator




| Command | Description | Remarks |
| --- | --- | --- |
| `commandinfo` |  |  | |
<br/>

### Economy




| Command | Description | Remarks |
| --- | --- | --- |
| `daily` |  | Gives you some Miunies but can only be used once a day | |
| `miunies` |  | Shows how many Miunies you have | |
| `richest` |  | Shows a user list of the sorted by Miunies. Pageable to see lower ranked users. | |
| `transfer` |  | Transferrs specified amount of your Minuies to the mentioned person. | |
| `newslot` |  | Creates a new slot machine if you feel the current one is unlucky | |
| `slots` |  | Play the slots! Win or lose some Miunies! | |
| `showslots` |  | Shows the configuration of the current slot machine | |
<br/>

### Misc




| Command | Description | Remarks |
| --- | --- | --- |
| `help` |  | DMs you a huge message if called without parameter - otherwise shows help to the provided command or module | |
| `version` |  | Returns the current version of the bot. | |
| `credits` | Shows everyone who has worked on and contributed to me |  | |
| `bug` | It sends users where to report bugs. |  | |
| `addition` | Adds 2 numbers together. |  | |
| `subtract` | Subtracts 2 numbers. |  | |
| `multiply` | Multiplys 2 Numbers. |  | |
| `divide` | Divides 2 Numbers. |  | |
| `math` | Computes mathematical operations. |  | |
| `list` | Manage lists with custom accessibility by role |  | |
<br/>

### Prefix

Setting for the Bots prefix on this server<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `prefix add` |  | Adds a prefix to the list of prefixes | |
| `prefix remove` |  | Removes a prefix from the list of prefixes | |
| `prefix list` |  | Show all possible prefixes for this server | |
<br/>

### Reminder

Tell the bot to remind you in some amount of time. The bot will send you a DM with the text you specified.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `reminder` |  | Add a reminder | |
| `reminder list` |  | List all your reminders | |
| `reminder delete` |  | Delete one of your reminders | |
<br/>

### Tasks

Settings for the repeated task that run in the background<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `tasks` |  |  | |
| `tasks start` |  |  | |
| `tasks interval` |  |  | |
| `tasks stop` |  |  | |
<br/>

### Bots

Allows access to pending and archived invite links to bots. This allows for you to submit your invite links for bots so that the guild's managers can add them.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `bots add` |  | Adds your bot's invite link to the invite queue where server managers can add your bot. Usage: bots add <bot's client id> <bot's name> "|" <description> | |
| `bots list` |  | Views all pending bots' invite links in the order requested at. Usage: bots list <page number> <archives/pending> | |
| `bots remove` |  | Removes a submission from the pending or archives list. Requires ManageGuild Permission. Usage: bots remove <bot's client id> <archives/pending> | |
| `bots archive` |  | Archives a submission from the pending list. Requires ManageGuild Permission. Usage: bots archive <bot's client id> | |
<br/>

### ServerSetup




| Command | Description | Remarks |
| --- | --- | --- |
| `offlog` |  |  | |
| `setlog` |  |  | |
| `setroleonjoin` |  |  | |
<br/>

### SetTimeZone




| Command | Description | Remarks |
| --- | --- | --- |
| `mycity` |  |  | |
<br/>

### Tag

Permanently assing a message to a keyword (for this server) which the bot will repeat if someone uses this command with that keyword.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `tag` |  | Let the bot send a message with the content of the named tag on the server | |
| `tag new` |  | Adds a new (not yet existing) tag to the server | |
| `tag update` |  | Updates the content of an existing tag of the server | |
| `tag remove` |  | Removes a tag off the server | |
| `tag list` |  | Show all tag on this server | |
<br/>

### PersonalTags

Permanently assing a message to a keyword (global for you) which the bot will repeat if you use this command with that keyword.<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `personaltags` |  | Lets the bot send a message with the content of your named tag | |
| `personaltags new` |  | Adds a new (not yet existing) tag to your collection | |
| `personaltags update` |  | Updates an existing tag of yours | |
| `personaltags remove` |  | Removes an existing tag of yours | |
| `personaltags list` |  | Show all your tags | |
<br/>

### Voice




| Command | Description | Remarks |
| --- | --- | --- |
| `voice` |  |  | |
<br/>

### RoleByPhrase

Settings for auto-assigning roles based on a sent Phrase<br/>


| Command | Description | Remarks |
| --- | --- | --- |
| `rolebyphrase status` |  | Returns the current state of RoleByPhrase lists and relations. | |
| `rolebyphrase addphrase` |  | Adds a new phrase to the guild's settings. (Phrase is a Remainder, so no double quotes are needed) | |
| `rolebyphrase addrole` |  | Adds a new phrase to the guild's settings. (Phrase is a Remainder, so no double quotes are needed) | |
| `rolebyphrase addrelation` |  | Adds a new relation between a phrase and a role. Relation are automatically enabled and used after you add them. | |
| `rolebyphrase removerelation` |  | Removes a relation between a phrase and a role. | |
| `rolebyphrase removephrase` |  | Removes a phrase and its relations. | |
| `rolebyphrase removerole` |  | Removes a role and its relations. | |
<br/>

### Trivia




| Command | Description | Remarks |
| --- | --- | --- |
| `trivia` |  |  | |
<br/>

### account




| Command | Description | Remarks |
| --- | --- | --- |
| `account info` |  |  | |
| `account showcommandhistory` |  |  | |
| `account getallmyaccountdata` |  |  | |
| `account deleteallmyaccountdata` |  |  | |
<br/>

