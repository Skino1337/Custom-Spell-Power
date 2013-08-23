/* Custom Spell Power by Skino */

//[CSP] Custom Spell Power by Skino
//[CSP] Found bug? Do not be silent! All info in plugin page.

game.hook("Dota_OnGetAbilityValue", onGetAbilityValue);
//game.hookEvent("entity_hurt", onEntityHurt, true);

var DEBUG = false;

var g_Mult = 3.0;
var g_Cap = 80.0;
var g_Spells = true, g_Items = true;

var replacedMap = [];

plugin.get("LobbyManager", function(lobbyManager)
{
	var m = lobbyManager.getOptionsForPlugin("CustomSpellPower")["Multiplier"];
	switch(m)
	{
	case "x1.5":
		g_Mult = 1.5;
		break;
	case "x2.0":
		g_Mult = 2.0;
		break;
	default:
	case "x3.0":
		g_Mult = 3.0;
		break;
	}
	
	var c = lobbyManager.getOptionsForPlugin("CustomSpellPower")["Cap"];
	switch(c)
	{
	case "60%":
		g_Cap = 60.0;
		break;
	default:
	case "80%":
		g_Cap = 80.0;
		break;
	case "100%":
		g_Cap = 100.0;
		break;
	}
	
	var s = lobbyManager.getOptionsForPlugin("CustomSpellPower")["Spec"];
	switch(s)
	{
	default:
	case "Spells and Items":
		g_Spells = true;
		g_Items = true;
		break;
	case "Spells":
		g_Spells = true;
		g_Items = false;
		break;
	case "Items":
		g_Spells = false;
		g_Items = true;
		break;
	}
});

var ignoreSpecific =
[
	"backdoor_protection.radius",
	"antimage_blink.min_blink_range",
	"drow_ranger_marksmanship.radius",
	"nevermore_shadowraze1.shadowraze_range",
	"nevermore_shadowraze2.shadowraze_range",
	"nevermore_shadowraze3.shadowraze_range",
	"windrunner_powershot.damage_reduction",
	"windrunner_powershot.speed_reduction",
	"item_tpscroll"
];

var ignoreParam =
[
	"interval",
	"cooldown",
	"damage_delay",
	"invuln",
	"illusion_damage_in_pct",
	"illusion_damage_out_pct",
	"incoming_damage",
	"outgoing_damage",
	"images_take",
	"images_do",
	"fow",
	"vision"
];

var increaseParam =
[
	"damage",
	"radius",
	"distance",
	"range",
	"aoe",
	"area",
	"duration",
	"amount",
	"count",
	"bonus",
	"resistance",
	"reduction",
	"heal",
	"hp",
	"health",
	"armor",
	"regen",
	"ministun",
	"slow",
	"threshold",
	"min",
	"max",
	// Special
	"mana_void_ministun",
	"points_per_tick",
	"per_stack",
	"explosion",
	"spirits"
];

var decreaseParam =
[
	"fade_delay",
	"fade_time",
	"land_time",
	// Special
	"focusfire_damage_reduction"
];

var fixedCapParam =
[
	"movement_speed",
	"movespeed",
	"move_slow",
	"ms",
	"chance",
	"percent",
	"evasion",
	"magical_armor",
	"spell_resist",
	"magic_damage_reduction",
	"attack_speed_pct",
	"slow_attack_speed"
];

function onGetAbilityValue(ability, abilityName, field, values)
{
	var fullName = abilityName + "." + field;
	
	if (typeof replacedMap[fullName] != "undefined") return;
	replacedMap[fullName] = true;
	
	if (abilityName.indexOf("item_") != -1)
	{
		if (!g_Items)
			return;
	}
	else
	{
		if (!g_Spells)
			return;
	}
	
	for (var i in ignoreSpecific)
	{
		if (ignoreSpecific[i].indexOf(".") != -1)
		{
			if (ignoreSpecific.indexOf(fullName) != -1)
			{
				if (DEBUG) printToAll("ignoreSpecific : " + fullName);
				return;
			}
		}
		else
		{
			if (ignoreSpecific.indexOf(abilityName) != -1)
			{
				if (DEBUG) printToAll("ignoreSpecific : " + fullName);
				return;
			}
		}
	}
	
	for (var i in ignoreParam)
	{
		if (field.indexOf(ignoreParam[i]) != -1)
		{
			if (DEBUG) printToAll("ignoreParam : " + fullName);
			return;
		}
	}
	
	for (var i in fixedCapParam)
	{
		if (field.indexOf(fixedCapParam[i]) != -1)
		{
			if (DEBUG) printToAll("fixedCapParam : " + fullName);
			return changePower(values, true, true);
		}
	}
	
	for (var i in decreaseParam)
	{
		if (field.indexOf(decreaseParam[i]) != -1)
		{
			if (DEBUG) printToAll("decreaseParam : " + fullName);
			return changePower(values, false, false);
		}
	}

	for (var i in increaseParam)
	{
		if (field.indexOf(increaseParam[i]) != -1)
		{
			if (DEBUG) printToAll("increaseParam : " + fullName);
			return changePower(values, true, false);
		}
	}
	
	if (DEBUG) printToAll("justSkipped : " + fullName + " : " + values);
	
	return;
}

function changePower(values, inc, g_Caped)
{
	var oldValues = values;
	
	if (inc)
		values = values.map(function(v) {return v * g_Mult;});
	else
		values = values.map(function(v) {return v / g_Mult;});
		
	if (g_Caped)
	{
		values = values.map(function(v) {return Math.min(v, g_Cap);});
		values = values.map(function(v) {return Math.max(v, -g_Cap);});
	}
	
	if (DEBUG) printToAll("Power changed: " + oldValues + " => " + values);
	
	return values;
}

var nonBoostDamageSpells =
[
	["lina_dragon_slave",			100,	170,	230,	280],
	["lina_dragon_slave",			100,	170,	230,	280]
];

function onEntityHurt(event)
{
	var attacked = game.getEntityByIndex(event.getInt("entindex_killed"));
	var attacker = game.getEntityByIndex(event.getInt("entindex_attacker"));
	var inflictor = game.getEntityByIndex(event.getInt("entindex_inflictor"));
	var damageType = event.getInt("damagebits");
	
	var inflictorName = inflictor.getClassname();
	var takenDamage = attacked.netprops.m_iRecentDamage;
	
	for (var i in nonBoostDamageSpells)
	{
		if (nonBoostDamageSpells[i][0].indexOf(inflictorName) != -1)
		{
			//var skillLevel = inflictor.netprops.m_iLevel;
			//var dmg = nonBoostDamageSpells[i][skillLevel-1] * g_Mult;	
			//var dmg = takenDamage * (g_Mult - 1.0);
			//var currhp = attacked.netprops.m_iHealth - 1;
			//dmg = Math.min(currhp, dmg);
			
			//if (DEBUG) printToAll(takenDamage);
			//if (DEBUG) printToAll("Unit " + attacker.getClassname() + " attack unit " + attacked.getClassname() + " with " + inflictorName + " lvl " + skillLevel + " dmgtype " + damageType);
			//attacked.netprops.m_iHealth -= dmg;
			//if (attacked.netprops.m_iHealth < 1)
			//	attacked.netprops.m_iHealth = 1;
		}
	}
}

function printToAll(string)
{
	var clients = getConnectedPlayingClients();
	for (var i in clients)
		clients[i].printToChat(string);
}

function getConnectedPlayingClients()
{
	var client, playing = [];
	for (var i = 0; i < server.clients.length; ++i)
	{
		client = server.clients[i];

		if (client === null)
			continue;

		if (!client.isInGame())
			continue;

		playerID = client.netprops.m_iplayerID;
		if (playerID == -1)
			continue;

		// if (getPlayerResource(playerID, "m_iConnectionState") !== 2)
		// 	continue;

		playing.push(client);
	}
	return playing;
}
