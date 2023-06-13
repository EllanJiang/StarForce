local Util = {}

function Util.Split(szFullString, szSeparator)
    local nFindStartIndex = 1
    local nSplitIndex = 1
    local nSplitArray = {}
    while true do
       local nFindLastIndex = string.find(szFullString, szSeparator, nFindStartIndex)
       if not nFindLastIndex then
        nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, string.len(szFullString))
        break
       end
       nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, nFindLastIndex - 1)
       nFindStartIndex = nFindLastIndex + string.len(szSeparator)
       nSplitIndex = nSplitIndex + 1
    end
    return nSplitArray
end

local function GetBlankStr(num)
	local str = ''
	for i=1,num do
		str = str ..'\t'
	end
	return str
end

function Util.Serialize(obj, dep)
    local lua = ""
    local t = type(obj)
    if t == "number" then
        lua = lua .. obj
    elseif t == "boolean" then
        lua = lua .. tostring(obj)
    elseif t == "string" then
        lua = lua .. string.format("%q", obj)
    elseif t == "table" then
        lua = lua .. "\n" .. GetBlankStr(dep) .."{"
        for k, v in pairs(obj) do
            lua = lua .. "\n" .. GetBlankStr(dep + 1) .."[" .. Util.Serialize(k, dep + 1) .. "]=" .. Util.Serialize(v, dep + 1) .. ","
        end
        local metatable = getmetatable(obj)
        if metatable ~= nil and type(metatable.__index) == "table" then
            for k, v in pairs(metatable.__index) do  
                lua = lua .. "\n" .. GetBlankStr(dep + 1) .."[" .. Util.Serialize(k, dep + 1) .. "]=" .. Util.Serialize(v, dep + 1) .. ","
            end
        end
        lua = lua .. "\n" ..GetBlankStr(dep) .. "}"
    elseif t == "nil" then
        return "nil"
    elseif t == "userdata" then
        return "userdata"
    elseif t == "function" then
        return "function"
    elseif t == "thread" then
        return "thread"
    else
        error("can not Serialize a " .. t .. " type.")
    end
    --lua = lua .. '\n'
    return lua
end

function Util.PrintTable(o, noprint)
    -- local str = "\n{"
    -- for k,v in pairs(o) do
    --     str = str .. Util.Serialize(v, 1) .. ","
    -- end
    -- str = str .. "\n}"
    local str = Util.Serialize(o, 1)
    if not noprint then
        print(str)
    end
   return str
end

function Util.PrintArray(t)
	for k,v in pairs(t) do
		print(k, v)
	end
end

function Util.CopyArray(t)
    local list = {}
    for k,v in pairs(t) do
        list[k] = v
    end

    return list
end

function Util.TraceTable(obj, fun, dep, route)
    local t = type(obj)
    if t == "table" then
        for k, v in pairs(obj) do
            route[dep] = k
            if fun(k, v) then
                return obj, k, v, dep
            else
                local ft,fk,fv,fdep = Util.TraceTable(v, fun, dep + 1, route)
                if ft then
                    return ft,fk,fv,fdep
                end
            end
        end
        local metatable = getmetatable(obj)
        if metatable ~= nil and type(metatable.__index) == "table" then
            for k, v in pairs(metatable.__index) do
                route[dep] = k
                if fun(k, v) then
                    return metatable.__index, k, v, dep
                else
                    local ft,fk,fv, fdep = Util.TraceTable(v, fun, dep + 1, route)
                    if ft then
                        return ft,fk,fv,fdep
                    end
                end
            end
        end
    end
end

function Util.TraceToFindTable(obj, fun)
    local route = {}
    local ft, fk,fv, dep = Util.TraceTable(obj, fun, 1, route)
    if dep then
        for k,v in pairs(route) do
            if k > dep then
                route[k] = nil
            end
        end
    end
    return ft, fk, fv, route
end

function Util.SaveToFile(name, data)
	local save_f = io.open(name, 'w')
    if save_f then
        save_f:write(data)
        save_f:close()
        return true 
    end

    return false
end

function Util.ReadFile(name)
    local data
    local file = io.open(name, 'r')
    if file then
        data = file:read("*a")
        file:close()
    end
    return data
end

function Util.ReadLines(name)
    local data = {}
    local file = io.open(name, 'r')
    if file then        
        file:close()
        for line in io.lines(name) do
            table.insert(data, line)
        end    
    end
    return data
end

function Util.TryGetValue(t, default, ...)
	local arg = {...}
	local cur_t = t
	for i=1,#arg do
		if cur_t[arg[i]] then
			 cur_t = cur_t[arg[i]]
		else
			return default
		end
	end

	return cur_t
end

function Util.CheckTableValueType(t, v_type, ...)
    local arg = {...}
    local cur_t = t
    for i=1,#arg do
        if cur_t[arg[i]] then
             cur_t = cur_t[arg[i]]
        else
            return false
        end
    end

    return type(cur_t) == v_type
end

function Util.TryGetValueByIndexList(t, default, index_list)
    local cur_t = t
    for i=1,#index_list do
        if cur_t[index_list[i]] then
             cur_t = cur_t[index_list[i]]
        else
            return default
        end
    end

    return cur_t
end

function Util.FillString(str, to_len, char)
    local new_str = str or ''
    char = char or ' '
    local str_len = string.len(new_str)
    if str_len >= to_len then
        return new_str
    end

    for i=1,to_len - str_len  do
        new_str = new_str .. char
    end

    return new_str
end

function Util.GetDateStrByTime(t)
    --return '20170101'
    return os.date("%Y%m%d",t)
end

function Util.GetTimeByDateTable(date_tab)
    --date_tab = { year=2012, day=17, month=5, hour=0, minute=0, second=0}
    return os.time(date_tab)
end

function Util.GetTime()
    return os.time()
end

function Util.GetLastDayNowTime()
    return Util.GetTime() - 3600 * 24
end

function Util.TryToNum(data, default)
    local num = tonumber(data)
    if num then 
        return num
    end

    if type(data) == 'string' then
        local new_str = string.gsub(data, "%%", "")
        num = tonumber(new_str)
        if num then 
            return num
        end
    end

    return default or 0
end

function Util.GetPreciseDecimal(num, dec_num)
    return math.floor(num * 10 ^ dec_num) / (10 ^ dec_num)
end

--纬度 经度
function Util.LLDistance(lat1, lon1, lat2, lon2)
    -- body
    lat1 = lat1 * math.pi / 180
    lon1 = lon1 * math.pi / 180
    lat2 = lat2 * math.pi / 180
    lon2 = lon2 * math.pi / 180

    local function haver_sin(theta)
        local r = math.sin(theta / 2);
        return r * r;
    end

    local d_lat = math.abs(lat1-lat2)
    local d_lon = math.abs(lon1-lon2)
    local h = haver_sin(d_lat) + math.cos(lat1) * math.cos(lat2) * haver_sin(d_lon);
    local distance = 2 * 6371.0 * 1000 * math.asin(math.sqrt(h))

    return distance
end

function Util.CheckString(str, pattan)
    pattan = pattan or "[%w_]+"
    if type(str) ~= 'string' then
        return false
    end
    local temp = string.match(str, pattan)
    
    if not temp then
        return false
    end

    if string.len(temp) ~= string.len(str) then
        return false
    end

    return true
end

function Util.PrintDataTable(tab)
    --tab = { title = {{len = 15, str = 'row1'}, {align = 'right',len = 15, str = 'row1'}, {align = 'center', len = 15, str = 'row1'},}, data = {{1,2,3},{44,55,66},{777,888,999}} }

    local result_str = ''   
    local row = #tab.title

    local length = 0
    for i,v in pairs(tab.title) do
        length = length + v.len
    end

    length = length + (#tab.title - 1) * 3 + 4

    local line_char_map = {[1] = '+', [length] = '+\n', [0] = '-'}

    local f_line = ''
    for i=1,length do
        f_line = f_line .. (line_char_map[i] or line_char_map[0])
    end

    local tatle_row = '| '
    for i,v in pairs(tab.title) do
        local content = v.str
        local strlen = string.len(content)
        if strlen > v.len then
            content = string.sub(content, 1, v.len)
        elseif strlen < v.len then
            local left_str = ''
            local right_str = ''
            local left_blank = 0
            local right_blank = 0
            if tab.title[i].align == 'center' then
                left_blank = math.floor((tab.title[i].len - strlen)/2)
                right_blank = tab.title[i].len - strlen - left_blank
                
            elseif tab.title[i].align == 'right' then
                left_blank = tab.title[i].len - strlen
            else
                right_blank = tab.title[i].len - strlen
            end

            for i=1,left_blank do
                left_str = left_str .. ' '
            end
            for i=1,right_blank do
                right_str = right_str .. ' '
            end
            content = left_str .. content .. right_str
        end
        tatle_row = tatle_row .. content .. ' | '
    end
    tatle_row = tatle_row .. '\n'
    result_str = result_str .. f_line .. tatle_row .. f_line

    for row ,data in ipairs(tab.data) do
        local data_str = '| '
        for i,v in pairs(data) do
            if i <= #tab.title then
                local content = tostring(v)
                local strlen = string.len(content)
                if strlen > tab.title[i].len then
                    content = string.sub(content, 1, tab.title[i].len)
                elseif strlen < tab.title[i].len then
                    local left_str = ''
                    local right_str = ''
                    local left_blank = 0
                    local right_blank = 0
                    if tab.title[i].align == 'center' then
                        left_blank = math.floor((tab.title[i].len - strlen)/2)
                        right_blank = tab.title[i].len - strlen - left_blank
                        
                    elseif tab.title[i].align == 'right' then
                        left_blank = tab.title[i].len - strlen
                    else
                        right_blank = tab.title[i].len - strlen
                    end

                    for i=1,left_blank do
                        left_str = left_str .. ' '
                    end
                    for i=1,right_blank do
                        right_str = right_str .. ' '
                    end
                    content = left_str .. content .. right_str
                    
                end
                data_str = data_str .. content .. ' | '
            end
        end
        data_str = data_str .. '\n'
        result_str = result_str .. data_str .. f_line
    end

    print(result_str)
    return result_str
end

return Util
