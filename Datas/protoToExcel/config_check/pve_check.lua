Json = require "config_check.json"
Util = require "config_check.util"
Text = require "config_check.text"

local print_t = print
local errcnt = 0
function Log(...)
	errcnt = errcnt + 1
	print_t(...)
end
print = Log

ConfigList = 
{
	"tbl_scene_path_config",
	"tbl_game_map_config",
	"tbl_rogue_like_random_stage_config",
	"tbl_rogue_like_stage_list_config",
	"tbl_rogue_like_monster_spawn_config",
	"tbl_zombie_monster_data",
	"tbl_monster_data",
	"tbl_monster_level_data",
	"tbl_game_task_config",
	"tbl_boss_monster_data",
}

EventEnum =
{
    SceneLoaded = 1,
    AreaTrigger = 2,
    FightAreaTrigger = 2,
    MissionComplete = 3,
    RoundStart = 4,
    ConfirmTeleport = 5,
    EventAreaTrigger = 6,
}

BehaviourEnum =
{
    OpenAirBox = 1,
    CloseAirBox = 2,
    SpawnMonster = 3,
    SetAiZombiePointData = 4,
    SetPlayerBornPoints = 5,
    SetActiveAirBoxPlan = 6,
    ActiveMission = 7,
    SetBattleArea = 8,
    SwitchToNextScene = 9,
    GenerateTreasureBoxes = 10,
    CreatePortal = 11,
    CreateTraps = 12,
    CreateNavLine = 13,
    DisableNavLine = 14,
    ShowGameTaskTips = 15,
    CreateDestinationPoint = 16,
    ClearDestinationPoint = 17,
    SpawnBoss = 18,
    SpawnProtectTarget = 19,
    ActiveBoss = 20,
    DestroyAllTraps = 21,
}


function LoadAllConfig()
	for k,v in pairs(ConfigList) do
		local path = "out/excel/lua_table_out/".. v .. ".lua"
		local code = Util.ReadFile(path)
		if code then
			local tb = loadstring(code)
			if tb then
				_G[v] = tb()
				--Util.PrintTable(_G[v])
			else
				print("load config tbl error " .. path)
			end
		else
			print("Load file Error :" .. path)
		end
	end
end

function GetSceneName(sceneId)
	if tbl_scene_path_config[sceneId] then
		return tbl_scene_path_config[sceneId].sceneName
	end
end

function GetMonsterSpawnConfig(id, level, playerCount)
	if tbl_rogue_like_monster_spawn_config[id] then
		return tbl_rogue_like_monster_spawn_config[id]
	end

	for k,v in pairs(tbl_rogue_like_monster_spawn_config) do
		if id == v.spawnId then
			if level == v.level or v.level == 0 then
				if playerCount == v.playerCount or v.playerCount == 0 then
					return v, v.spawnId * 10000 + v.level * 100 + v.playerCount
				end
			end
		end
	end
	return tbl_rogue_like_monster_spawn_config[id]
end

function GetZombieConfig(id)
	return tbl_zombie_monster_data[id]
end

function GetBossConfig(id)
	return tbl_boss_monster_data[id]
end

function GetMonsterDataConfig(id)
	return tbl_monster_data[id]
end

function GetMonsterLevelConfig(id)
	return tbl_monster_level_data[id]
end

function GetTaskConfig(id, level, playerCount)
	if tbl_game_task_config[id] then
		return tbl_game_task_config[id]
	end

	local castid = id * 1000 + level + 100 + playerCount

	if tbl_game_task_config[castid] then
		return tbl_game_task_config[castid]
	end

	return tbl_game_task_config[id]
end

function GetGameLevel(mapId)
	for k,v in pairs(tbl_game_map_config) do
		if mapId == v.idMS then
			return v.gameLevel
		end
	end
	return 0
end


EventBehaviourCheckList =
{
    --SceneLoaded = {"SetActiveAirBoxPlan", "SetPlayerBornPoints", },
    --AreaTrigger = {"SpawnMonster","SetAiZombiePointData","ActiveMission", "SetBattleArea"},
    SceneLoaded = {"SetPlayerBornPoints", },
    AreaTrigger = {"ActiveMission"},
}

function CheckEvent(eventList, stageId, scene, level, playerCount)
	local ev_counter = {}
	local monster_info = {}
	local tower_info = {}
	local task_info = {}
	local finish_task = {}
	for _,ev in pairs(eventList) do
		local d = ev_counter[ev.EventId]
		if not d then
			d = {cnt = 0, param_map = {}}
			ev_counter[ev.EventId] = d
		end
		d.cnt = d.cnt + 1
		if ev.EventId == EventEnum.AreaTrigger then
			local id = ev.EventParams[1]
			if id then
				d.param_map[id] = d.param_map[id] or 0
				d.param_map[id] = d.param_map[id] + 1
				if d.param_map[id] > 1 then
					print(string.format(Text.areatriggerrepeat , scene, stageId, id))
				end
			end
		end

		if ev.EventId == EventEnum.MissionComplete then
			local id = tonumber(ev.EventParams[1])
			finish_task[id] = finish_task[id] or 0
			finish_task[id] = finish_task[id] + 1
		end
			
		local bh_counter = {}
		
		for _,b in pairs(ev.Behaviors) do
			bh_counter[b.BehaviorId] = bh_counter[b.BehaviorId] or 0
			bh_counter[b.BehaviorId] = bh_counter[b.BehaviorId] + 1

			if b.BehaviorId == BehaviourEnum.OpenAirBox then
				local id = tonumber(b.BehaviorParams[1])
				if not id then
					print(string.format(Text.lostopenairboxid, scene, stageId , Text.EventName[ev.EventId] or "??" ))
				end
			end
			if b.BehaviorId == BehaviourEnum.CloseAirBox then
				local id = tonumber(b.BehaviorParams[1])
				if not id then
					print(string.format(Text.lostcloseairboxid, scene, stageId , Text.EventName[ev.EventId] or "??"))
				end
			end
			if b.BehaviorId == BehaviourEnum.SetActiveAirBoxPlan then
				local id = tonumber(b.BehaviorParams[1])
				if not id then
					print(string.format(Text.lostboxplanid, scene, stageId , Text.EventName[ev.EventId] or "??"))
				end
			end

			if b.BehaviorId == BehaviourEnum.SpawnProtectTarget then
				local id = tonumber(b.BehaviorParams[1])
				local poslist = Json.decode(b.BehaviorParams[2])
				local tcount = 1
				if not poslist.points or not poslist.points[1] then
					tcount = 0
					print(string.format(Text.losttowerposcfg, scene, stageId , id))
				end

				tower_info[ev.EventParams[1]] = tower_info[ev.EventParams[1]] or {cnt = 0, id_list={}}
				tower_info[ev.EventParams[1]].cnt = tower_info[ev.EventParams[1]].cnt + tcount
				if tcount > 0 then
					table.insert(tower_info[ev.EventParams[1]].id_list, id)
				end
			end
			if b.BehaviorId == BehaviourEnum.SpawnBoss then
				local bossid = tonumber(b.BehaviorParams[1])
				local poslist = Json.decode(b.BehaviorParams[2])
				if not poslist.points or not poslist.points[1] then
					print(string.format(Text.lostbossposcfg, scene, stageId , bossid))
				end
				local zf = GetBossConfig(bossid)
				if zf then
					local mf = GetMonsterDataConfig(zf.MonsterId)
					if mf then
						local lf = GetMonsterLevelConfig(zf.Level * 100000 + zf.MonsterId)
						if not lf then
							print(string.format(Text.lostbosslevelcfg , bossid))
						end
					else
						print(string.format(Text.lostbossmonstercfg  , bossid, zf.MonsterId))
					end

					monster_info[ev.EventParams[1]] = monster_info[ev.EventParams[1]] or {cnt = 0, dict = {}}
					local spawn_monster_info = monster_info[ev.EventParams[1]].dict
					spawn_monster_info[zf.MonsterId] = spawn_monster_info[zf.MonsterId] or 0
					spawn_monster_info[zf.MonsterId] = spawn_monster_info[zf.MonsterId] + 1
					--monster_info[ev.EventParams[1]] = {cnt = 1, dict = spawn_monster_info, boss = bossid}
				else
					print(string.format(Text.lostbosscfg , bossid))
				end
			end
			if b.BehaviorId == BehaviourEnum.SpawnMonster then
				monster_info[ev.EventParams[1]] = monster_info[ev.EventParams[1]] or {cnt = 0, dict = {}}
				local spawn_monster_info = monster_info[ev.EventParams[1]].dict
				local monster_count = 0
				local param = Json.decode(b.BehaviorParams[1])
				if param.m_Round ~= tonumber(ev.EventParams[1]) then
					print(string.format(Text.monsterrounderror, scene, stageId, tonumber(ev.EventParams[1]), param.m_Round))
				end
				for _,w in pairs(param.m_Wave) do
					for wid,a in pairs(w.m_Area) do
						local spawnid = a.m_SpawnListId
						local c, castid = GetMonsterSpawnConfig(spawnid, level, playerCount)
						if castid then
							spawnid = castid
						end
						if c then
							for _,s in pairs(c.crowd) do
								spawn_monster_info[s.id] = spawn_monster_info[s.id] or 0
								spawn_monster_info[s.id] = spawn_monster_info[s.id] + s.num

								monster_count = monster_count + s.num
								local zf = GetZombieConfig(s.id)
								if zf then
									local mf = GetMonsterDataConfig(zf.MonsterId)
									if mf then
										local lf = GetMonsterLevelConfig(zf.Level * 100000 + zf.MonsterId)
										if not lf then
											print(string.format(Text.lostmonsterlevelcfg , spawnid, s.id))
										end
									else
										print(string.format(Text.lostmonstercfg , spawnid, s.id, zf.MonsterId))
									end
								else
									print(string.format(Text.lostzombiecfg , spawnid, s.id))
								end
							end
						else
							print(string.format(Text.lostmonsterspawncfg ,scene,stageId, tonumber(ev.EventParams[1]), a.m_SpawnListId, level, playerCount))
						end
					end
				end
				monster_info[ev.EventParams[1]].cnt = monster_info[ev.EventParams[1]].cnt + monster_count
			end
			if b.BehaviorId == BehaviourEnum.ActiveMission then
				local taskid = tonumber(b.BehaviorParams[1])			
				local cfg, castid = GetTaskConfig(taskid, level, playerCount)
				if not cfg then
					print(string.format(Text.lostactivetaskcfg ,scene, stageId ,taskid))
				end
				if castid then
					taskid = castid
				end

				task_info[ev.EventParams[1]] = task_info[ev.EventParams[1]] or {}
				table.insert(task_info[ev.EventParams[1]], taskid)				
			end
		end

		local task_check = task_info[ev.EventParams[1]]
		if task_check then
			for _,t in pairs(task_check) do
				local cfg = GetTaskConfig(t, level, playerCount)
				if cfg then
					local monster_check = monster_info[ev.EventParams[1]]
					if cfg.contType == 1 then
						if monster_check then
							if monster_check.cnt < cfg.condParam[1] then
								print(string.format(Text.lostmonsternum, scene, stageId, tonumber(ev.EventParams[1]), monster_check.cnt, t, cfg.condParam[1]))
							end
						end
					elseif cfg.contType == 2 then
						if monster_check then
							for i=1, #cfg.condParam /2 do
								local id = cfg.condParam[2 *i -1]
								local num = cfg.condParam[2 *i]
								local cnt = monster_check.dict[id] or 0
								if num ~= cnt then
									print(string.format(Text.lostmonsteridnum , scene, stageId, tonumber(ev.EventParams[1]), id, cnt, t, num))
								end
							end
						end
					elseif cfg.contType == 3 then
						local tower_check = tower_info[ev.EventParams[1]] or {cnt = 0, id_list={}}
						if tower_check.cnt < cfg.condParam[1] then
							print(string.format(Text.losttowernum, scene, stageId, tonumber(ev.EventParams[1]), tower_check.cnt, t, cfg.condParam[1]))
						end
					end
				end
			end
		end

		for en,bnl in pairs(EventBehaviourCheckList) do
			if EventEnum[en] == ev.EventId then
				for _,bn in pairs(bnl) do
					if not bh_counter[BehaviourEnum[bn]] then
						print(string.format(Text.lostbehaviour, scene, stageId, en, bn))
					end
				end
			end 
		end
	end

	for a,tl in pairs(task_info) do
		for _,t in pairs(tl) do
			if not finish_task[t] then
				print(string.format(Text.losttaskcompleteevent ,scene, stageId, t))
			end
		end		
	end

	if not ev_counter[EventEnum.SceneLoaded] then
		print(string.format(Text.losteventsceneloaded, scene, stageId))
	end

	if not ev_counter[EventEnum.AreaTrigger] then
		print(string.format(Text.losteventareatrigger, scene, stageId))
	end

	if not ev_counter[EventEnum.MissionComplete] then
		print(string.format(Text.losteventmissioncomplete, scene, stageId))
	end
end

function CheckStageInJson(sceneId, stageId, fromeGroup, level, playerCount)
	local sceneName = GetSceneName(sceneId)
	if not sceneName then
		print(string.format(Text.sceneconfiglost, sceneId, fromeGroup))
		return
	end
	local path = string.format("../unlimited/project/Assets/ConfigAsset/ExportConfig/SceneConfig/%s/%s_1033.json",sceneName,sceneName)
	local data = Util.ReadFile(path)
	if not data then
		print(string.format(Text.scenejsonlost,sceneId, fromeGroup, path))
		return
	end
	local js_tb = Json.decode(data)

	local find_stage = false
	for k,v in pairs(js_tb.m_SceneEventConfigs) do
		if stageId == v.configId then

			CheckEvent(v.m_Events, stageId, sceneName, level, playerCount)
			find_stage = true
		end
	end
	if not find_stage then
		print(string.format(Text.loststageinjson, sceneName, stageId, fromeGroup))
		return
	end
end

function CheckPVE()
	LoadAllConfig()
	local pve_list = {}

	for k1,v1 in pairs(tbl_rogue_like_random_stage_config) do
		local level = GetGameLevel(1033*10000 + v1.stageGroup)
		for k2,v2 in pairs(v1.stageList) do
			local stage_list_cfg = tbl_rogue_like_stage_list_config[v2.id]			
			if not stage_list_cfg then
				print(string.format(Text.randstagelost, v2.id, v1.playerCount, v1.stageGroup))
			else
				for i, stage_id in pairs(stage_list_cfg.stage) do
					local scene = (stage_id - stage_id % 10000)/10000
					CheckStageInJson(scene, stage_id%10000, v1.stageGroup, level, v1.playerCount)
				end
			end			
		end
	end

	-- local path = "share_json_config/GameModeConfig/PveMode/RogueLikeAiConfig.json"
	-- local data = Util.ReadFile(path)
	-- if data then
	-- 	local aiConfig = Json.decode(data)
	-- 	local map = {}
	-- 	for k,v in pairs(aiConfig.SniperList) do
	-- 		if map[v.ZombieId] then
	-- 			print(string.format("RogueLikeAiConfig.json SniperList repeated ZombieId %d", v.ZombieId))
	-- 		else
	-- 			map[v.ZombieId] = v
	-- 		end			
	-- 	end
	-- else
	-- 	print("share_json_config/GameModeConfig/PveMode/RogueLikeAiConfig.json not find")
	-- end

end

CheckPVE()
print_t("\n")
print_t(string.format(Text.finish, errcnt))
io.read()
