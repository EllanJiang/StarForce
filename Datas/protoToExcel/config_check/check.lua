
Util = require "config_check.util"
Text = require "config_check.text"
Json = require "config_check.json"

ClientPath = ... or "../unlimited/project"

ERelyType = 
{
	VAL = 1, --值依赖
	LINEAR = 2, --线性依赖
	ARRAY = 3, --数组依赖
	STRUCTARRAY = 4, --结构数组依赖
	FILEPATH = 5, --文件依赖
	MULTIREPEAT = 6, --多列查重
	LANGUAGE = 7, --LANGUAGE.assest
}

local print_t = print
local errcnt = 0
function Log(...)
	errcnt = errcnt + 1
	print_t(...)
end
print = Log

print_t("ClientPath:" .. ClientPath)

function ConfigNameToLuaName(name)
    local str = ""
    for i=1,string.len(name) do
        local char = string.sub(name,i,i)
        if char ~= string.lower(char) then 
            if i ~= 1 then 
                char = "_"..char
            end
        end
        str = str..string.lower(char)
    end
    return "tbl_"..str
end


function LoadConfig(name)
	local tn = ConfigNameToLuaName(name)
	local path = "out/excel/lua_table_out/".. tn .. ".lua"
	local code = Util.ReadFile(path)
	if code then
		local tb = loadstring(code)
		if tb then
			return tb()
		else
			print("load config tbl error " .. path)
			return {}
		end
	else
		print("Load file Error :" .. path)
		return {}
	end
end


function ValRelyCheck(valueTable, relyTable, cfg)
	for k,v in pairs(relyTable) do
		if valueTable[cfg.fieldName] == v[cfg.relyFieldName] or CanSkip(valueTable[cfg.fieldName], cfg) then
			return true
		end
	end
	return false 
end

function LinearRelyCheck(valueTable, relyTable, cfg)	
	local val = 0
	local f = "("
	for i=1, #cfg.relyParam/2 do
		local kn = cfg.relyParam[2*i - 1]
		f = f .. kn .. " * " .. cfg.relyParam[2*i]
		if i ~= #cfg.relyParam/2 then
			f = f .. " + "
		end
		val = val + valueTable[kn]*cfg.relyParam[2*i]
	end
	f = f .. ")"
	for k,v in pairs(relyTable) do		
		if val == v[cfg.relyFieldName] or CanSkip(val, cfg) then
			return true
		end
	end
	return false, {val, f}
end

function ArrayRelyCheck(valueTable, relyTable, cfg)
	for ck,cv in pairs(valueTable[cfg.fieldName]) do
		local f = false
		for tk,tv in pairs(relyTable) do
			if cv == tv[cfg.relyFieldName] or CanSkip(cv, cfg) then
				f = true
				break;
			end
		end
		if not f then
			return false, {ck, cv}
		end
	end
	
	return true 
end

function StructArrayRelyCheck(valueTable, relyTable, cfg)
	for ck,cv in pairs(valueTable[cfg.fieldName]) do
		local f = false
		for tk,tv in pairs(relyTable) do
			if cv[cfg.relyParam[1]] == tv[cfg.relyFieldName] or CanSkip(cv[cfg.relyParam[1]], cfg) then
				f = true
				break;
			end
		end
		if not f then
			return false, {ck, cv[cfg.relyParam[1]]}
		end
	end
	
	return true 
end


function CheckLaser(str,valueTable, path)
	--print(str, path)
	if string.find(str, "Laser") then
		print(" =====================",GetLineID(valueTable), str, path)
		io.read()
	end
end

function FilePathRelyCheck(valueTable, relyTable, cfg)
	local value = valueTable[cfg.fieldName]

	local cpath = cfg.relyParam[1]
	if  string.find(cfg.relyParam[1], "CLIENTPATH") then
		cfg.relyParam[1] = string.gsub(cfg.relyParam[1], "CLIENTPATH", ClientPath)
	end
	local path = cfg.relyParam[1] .. value .. (cfg.relyParam[2] or "")
	local data = Util.ReadFile(path)
	if data then		
		if cfg.relyParam[3] == "weapon" then
			local t = Json.decode(data)
			if not t then
				return false , {path, 2}
			end

			local fcn = t.FireLogicClassConfigFileName
			local fccn = t.FireLogicClassName
			if not fcn then
				if t.parent then
					fcn = t.parent.FireLogicClassConfigFileName
					fccn = t.parent.FireLogicClassName
				end
			end
			if fcn and t.FireLogicClassConfigFileName ~= "" then
				local p2 = cfg.relyParam[1] .. fcn .. ".json"
				local d2 = Util.ReadFile(p2)
				if not d2 then
					return false , {path, 4, "FireLogicClassConfigFileName",fcn}
				end
				local t2 = Json.decode(d2)
				if not t2 then
					return false , {path, 5, "FireLogicClassConfigFileName",fcn}
				else
					local acf = t2.AdditionRecoilConfigFileName
					if not acf then
						if t2.parent then
							acf = t2.parent.AdditionRecoilConfigFileName
						end
					end
					if acf and acf ~= "" then
						local p3 = cfg.relyParam[1] .. acf .. ".json"
						local d3 = Util.ReadFile(p3)
						if not d3 then
							return false , {path, 6, "FireLogicClassConfigFileName",fcn,acf}
						end
						local t3 = Json.decode(d3)
						if not t3 then
							return false , {path, 7, "FireLogicClassConfigFileName",fcn,acf}							
						end
					end
				end
			else
				if fccn and fccn ~= "" then
					return false , {path, 3, "FireLogicClassConfigFileName"}
				end
			end

			local scn = t.SecondaryFireLogicConfigFileName
			local sccn = t.SecondaryFireLogicClassName
			if not scn then
				if t.parent then
					scn = t.parent.SecondaryFireLogicConfigFileName
					sccn = t.parent.SecondaryFireLogicClassName
				end
			end
			if scn and scn ~= "" then
				local p2 = cfg.relyParam[1] .. scn .. ".json"
				local d2 = Util.ReadFile(p2)
				if not d2 then
					return false , {path, 4, "SecondaryFireLogicConfigFileName", scn}
				end
				local t2 = Json.decode(d2)
				if not t2 then
					return false , {path, 5, "SecondaryFireLogicConfigFileName", scn}
				else
					local acf = t2.AdditionRecoilConfigFileName
					if not acf then
						if t2.parent then
							acf = t2.parent.AdditionRecoilConfigFileName
						end
					end
					if acf and acf ~= "" then
						local p3 = cfg.relyParam[1] .. acf .. ".json"
						local d3 = Util.ReadFile(p3)
						if not d3 then
							return false , {path, 6, "SecondaryFireLogicConfigFileName",fcn,acf}
						end
						local t3 = Json.decode(d3)
						if not t3 then
							return false , {path, 7, "SecondaryFireLogicConfigFileName",fcn,acf}							
						end
					end
				end
			else
				if sccn and sccn ~= "" then
					return false , {path, 3, "SecondaryFireLogicConfigFileName"}
				end
			end
		end
		return true
	end
	return false , {path, 1}
end

function MultiRepeatCheck(valueTable, relyTable, cfg)
	local lines = {}
	
	for tk,tv in pairs(relyTable) do
		local alleq = true
		for i,v in ipairs(cfg.relyParam) do
			if valueTable[v] ~= tv[v] then
				alleq = false
			end
		end

		if alleq then
			table.insert(lines, GetLineID(tv))
		end
	end
	
	return #lines == 1, lines
end

local LANGUAGE_FILE_STRING = nil
function LanguageCheck(valueTable, relyTable, cfg)
	if LANGUAGE_FILE_STRING == nil then
		local path = "out/excel/language_asset_out/tbl_config_language.asset" 
		LANGUAGE_FILE_STRING = Util.ReadFile(path)
	end

	local key = valueTable[cfg.fieldName]
	if key ~= "" and key ~= 0 then
		if not string.find(LANGUAGE_FILE_STRING, key .. "") then
			return false
		end
	end

	return true 
end

function CanSkip(value, cfg)
	if value == 0 and cfg.skipZero then
		return true
	end
	return false
end

CheckFunMap = {
	[ERelyType.VAL] = ValRelyCheck,
	[ERelyType.LINEAR] = LinearRelyCheck,
	[ERelyType.ARRAY] = ArrayRelyCheck,
	[ERelyType.STRUCTARRAY] = StructArrayRelyCheck,
	[ERelyType.FILEPATH] = FilePathRelyCheck,	
	[ERelyType.MULTIREPEAT] = MultiRepeatCheck,
	[ERelyType.LANGUAGE] = LanguageCheck,

}

function GetLineID(valueTable)
	if valueTable.id then
		return valueTable.id
	end

	if valueTable.lineNumber then
		return valueTable.lineNumber + 3
	end
	return 0
end

function ArrayToString(t)
	local s = ""
	for k,v in pairs(t) do
		s = s .. v .. ","
	end
	return s
end

function KeyArrayToString(t, kt)
	local s = ""
	for k,v in pairs(kt) do
		s = s .. v .. ":" .. t[v] .. ","
	end
	return s
end

function ReportError(valueTable, cfg, info)
	if cfg.relyType == ERelyType.VAL then
		local line = GetLineID(valueTable)
		local value = valueTable[cfg.fieldName]
		print(string.format(Text.valrelyerror,cfg.configName,line,cfg.fieldName,value,cfg.relyConfigName,cfg.relyFieldName))
		return 
	end

	if cfg.relyType == ERelyType.LINEAR then
		local line = GetLineID(valueTable)
		local value = info[1]
		local f = info[2]
		print(string.format(Text.linnearrelyerror ,cfg.configName,line,f,value,cfg.relyConfigName,cfg.relyFieldName))
		return 
	end

	if cfg.relyType == ERelyType.ARRAY then
		local line = GetLineID(valueTable)
		local index = info[1]
		local value = info[2]
		print(string.format(Text.arrayrelyerror ,cfg.configName,line,cfg.fieldName,index, value,cfg.relyConfigName,cfg.relyFieldName))
		return 
	end

	if cfg.relyType == ERelyType.STRUCTARRAY then
		local line = GetLineID(valueTable)
		local index = info[1]
		local value = info[2]
		print(string.format(Text.structarrayrelyerror ,cfg.configName,line,cfg.fieldName,index,cfg.relyParam[1],value,cfg.relyConfigName,cfg.relyFieldName))
		return 
	end

	if cfg.relyType == ERelyType.FILEPATH then
		local line = GetLineID(valueTable)
		local value = valueTable[cfg.fieldName]
		local p = info[1]
		local t = info[2]
		if t == 1 then
			print(string.format(Text.filerelyerror ,cfg.configName,line,cfg.fieldName,value,p))
		elseif t == 2 then
			print(string.format(Text.filerelyjsonerror ,cfg.configName,line,cfg.fieldName,value,p))
		else
			local fn = info[3]
			local estr_t = {Text.firemodenull ,Text.firemodelost , Text.firemodeError, Text.fireaddmodeError , Text.fireaddmodejsonError }
			local str = string.format(estr_t[t - 2], info[4] or "", info[5] or "")

			print(string.format(Text.fileweaponrelyerror ,cfg.configName,line,cfg.fieldName,value,fn,str))
		end
		return 
	end

	if cfg.relyType == ERelyType.MULTIREPEAT then
		local line = GetLineID(valueTable)
		print(string.format(Text.multirepeaterror,cfg.configName, KeyArrayToString(valueTable, cfg.relyParam), ArrayToString(info)))
	end

	
	if cfg.relyType == ERelyType.LANGUAGE then
		local line = GetLineID(valueTable)
		local value = valueTable[cfg.fieldName]
		print(string.format("config : %s, line %s, fieldName %s, value %s ",cfg.configName,line,cfg.fieldName,value))
	end
	
end

function CheckTable(cfg)
	local t1 = LoadConfig(cfg.configName)
	local t2 = LoadConfig(cfg.relyConfigName)

	local fun = CheckFunMap[cfg.relyType]
	if not fun then
		print(string.format("ZZConfigCheck Line %d relyType %d not suported", GetLineID(cfg), cfg.relyType))
		return
	end
	for k1,v1 in pairs(t1) do
		local result, info = fun(v1, t2, cfg)
		if not result then
			ReportError(v1, cfg, info)			
		end
	end
end


BUFF_CHECK_TB = LoadConfig("ZZBuffTypeCheck")
BUFF_TB = LoadConfig("BuffDataV2Config")
function GetBuffTypeCfg(buffType)
	for k,v in pairs(BUFF_CHECK_TB) do
		if v.buffType == buffType then
			return v
		end
	end
	return nil
end

function CheckBuff(buff)
	local cfg = GetBuffTypeCfg(buff.buffType)
	if not cfg then
		print(string.format(Text.lostbufftypecfg ,buff.id, buff.buffType))
	else
		if cfg.minParam > #buff.extraParam then
			print(string.format(Text.lostbuffparam ,buff.id, buff.buffType, cfg.minParam, #buff.extraParam))
		end		

		for k,index in pairs(cfg.nonZeroIndex) do
			if index > 0 then
				if buff.extraParam[index] then
					if buff.extraParam[index] == 0 then						
						print(string.format(Text.buffparamzero ,buff.id, buff.buffType, index))
					end
				else
					print(string.format(Text.buffparamlostnonzero ,buff.id, buff.buffType, index))
				end
			elseif index == -1 then
				for k,v in pairs(buff.extraParam) do
					if v == 0 then
						print(string.format(Text.buffparamnonzerolist ,buff.id, buff.buffType, k))
					end
				end
			end
		end

		for k,index in pairs(cfg.buffIndex) do
			if index > 0 then
				if buff.extraParam[index] then
					if not BUFF_TB[buff.extraParam[index]] then						
						print(string.format(Text.buffparamnotbuff ,buff.id, buff.buffType, index, buff.extraParam[index]))
					end
				else
					print(string.format(Text.buffparambufflost ,buff.id, buff.buffType, index))
				end
			elseif index == -1 then
				for k,v in pairs(buff.extraParam) do
					if not BUFF_TB[v] then
						print(string.format(Text.buffparamnotbuff ,buff.id, buff.buffType, k, v))
					end
				end
			end
		end
	end
end

function CheckAll()
	local check_tb = LoadConfig("ZZConfigCheck")
	for k,v in pairs(check_tb) do		
		CheckTable(v)
	end
	
	for k,v in pairs(BUFF_TB) do
		CheckBuff(v)
	end
end



CheckAll()

print_t("\n")
print_t(string.format(Text.excelfinish, errcnt))

io.read()
