--[[
* @file:   TimedAnnouncementConfig
* @brief:  这个文件是通过工具自动生成的，建议不要手动修改
]]--

local TimedAnnouncementConfigTable = 
{
	[1] = {content = '欢迎来到GOM！本次开服时间为18:00-22:00，祝您游戏愉快！', id = 1, priority = 3, showTime = {{start = '11:00:00', stop = '11:10:59'}}, timeType = 'day'}, 
	[2] = {content = '游戏将于22:00点进行停机维护，届时将无法开始游戏，请注意时间哦！', id = 2, priority = 1, showTime = {{start = '14:45:00', stop = '15:14:59'}}, timeType = 'day'}, 
	[3] = {content = '现在是上午', id = 3, priority = 2, showTime = {{start = '2021-01-10 00:00:00', stop = '2021-01-30 05:59:58'}}, timeType = 'limit'}, 
	[4] = {content = '现在是下午', id = 4, priority = 2, showTime = {{start = '2021-01-10 07:00:00', stop = '2021-01-30 13:59:58'}}, timeType = 'limit'}
}
return TimedAnnouncementConfigTable