local Text = 
{
	finish = "PVE配置检查完成 发现 %d 处错误",
	randstagelost = "找不到关卡列表ID %d PlayerCount: %d stageGroup: %d  ---检查RogueLikeRandomStageConfig 和 RogueLikeStageListConfig ",
	sceneconfiglost = "找不到场景配置 SceneId: %d sceneGroupId: %d  ----检查RogueLikeStageListConfig 和 SceneConfig",
	scenejsonlost = "找不到场景json SceneId: %d sceneGroupId: %d  ----检查确认存在文件 %s",
	loststageinjson = "%s 场景json配置中找不到关卡 stageId: %d sceneGroupId: %d ---检查RogueLikeStageListConfig 和 场景导出",
	areatriggerrepeat = "%s 场景中 关卡 %d 触发器区域下标 %d 重复  ---检查场景导出",
	monsterrounderror = "%s 场景中 关卡 %d 触发器区域下标 %d 和 怪物刷新回合 %d 不匹配  ---检查场景导出",
	losteventsceneloaded = "%s 场景中 关卡 %d 没有场景加载事件  ---检查场景导出",
	losteventareatrigger = "%s 场景中 关卡 %d 没有区域触发器事件  ---检查场景导出",
	losteventmissioncomplete = "%s 场景中 关卡 %d 没有任务完成事件  ---检查场景导出",
	losttaskcompleteevent = "%s 场景中 关卡 %d  任务 %d 缺少完成事件  ---检查场景导出",
	lostbehaviour = "%s 场景中 关卡 %d  事件 %s 缺少行为 %s ---检查场景导出",
	activetaskcfgtypeerr = "%s 场景中 关卡 %d  接受任务 %d 类型 %d 错误 ---检查GameTaskConfig",
	lostactivetaskcfg = "%s 场景中 关卡 %d  接受任务 %d 不在任务表中 ---检查GameTaskConfig 和 场景导出",
	lostmonsternum = "%s 场景中 关卡 %d  区域 %d 刷怪数 %d != 任务 %d 完成条件 %d ---检查GameTaskConfig 和 场景导出",
	lostmonsteridnum = "%s 场景中 关卡 %d  区域  %d 刷怪 id %d 数量 %d != 任务 %d 完成条件 %d ---检查GameTaskConfig 和 场景导出",
	lostmonsterlevelcfg = "刷怪配置id %d 僵尸 %d 缺少怪物等级配置  ---检查RogueLikeMonsterSpawnConfig 和 MonsterLevelData",
	lostmonstercfg = "刷怪配置id %d 僵尸 %d  monsterId %d 缺少怪物配置  ---检查RogueLikeMonsterSpawnConfig 和 MonsterData，MonsterLevelData",
	lostzombiecfg = "刷怪配置id %d 僵尸 %d   缺少僵尸配置  ---检查RogueLikeMonsterSpawnConfig 和 ZombieMonsterData，MonsterData，MonsterLevelData",
	lostmonsterspawncfg = "%s 场景中 关卡 %d  区域 %s 缺少刷怪配置 %d 等级%d 人数%d---检查RogueLikeMonsterSpawnConfig  和 场景导出 ",
	lostbosslevelcfg = "刷Boss %d 缺少怪物等级配置  ---检查BossMonsterConfig 和 MonsterLevelData",
	lostbossmonstercfg= "刷Boss %d  monsterId %d 缺少怪物配置  ---检查 BossMonsterConfig，MonsterLevelData",
	lostbosscfg= "刷Boss %d   缺少boss配置  ---检查 BossMonsterConfig 和 场景导出",

	lostbossposcfg= "%s 场景中 关卡 %d 刷Boss %d   缺少坐标配置  ---检查 场景导出",
	losttowerposcfg= "%s 场景中 关卡 %d 刷水晶塔 %d   缺少坐标配置  ---检查场景导出",

	excelfinish = "Excel配置依赖检查完成 发现 %d 处错误",
	valrelyerror = "配置表: %s 行: %s 列: %s 值: %s 在依赖表: %s 列: %s 中找不到",
	linnearrelyerror = "配置表: %s 行: %s 列组合: %s 值: %s 在依赖表: %s 列: %s 中找不到",

	arrayrelyerror = "配置表: %s 行: %s 数组列: %s 第 %s 位 值: %s 在依赖表: %s 列: %s 中找不到",
	structarrayrelyerror = "配置表: %s 行: %s 结构数组列: %s 第 %s 位 成员 %s 值: %s 在依赖表: %s 列: %s 中找不到",
	filerelyerror = "配置表: %s 行: %s 列: %s 值(文件名): %s 找不到文件: %s",
	filerelyjsonerror = "配置表: %s 行: %s 列: %s 值(文件名): %s json文件格式错误: %s",
	fileweaponrelyerror = "配置表: %s 行: %s 列: %s 值(文件名): %s 武器开火模式 %s %s",
	firemodenull = "未配置 %s %s",
	firemodelost = "文件 %s 丢失 %s",
	firemodeError = "文件 %s json格式错误 %s",
	fireaddmodeError = "文件 %s AdditionRecoilConfigFileName %s 未配置 json",
	fireaddmodejsonError = "文件 %s AdditionRecoilConfigFileName %s  json 格式错误",
	multirepeaterror = "配置表: %s  存在多列 %s 值重复 数据行 %s",
	losttowernum = "%s 场景中 关卡 %d  区域 %d 刷塔数 %d < 任务 %d 完成条件 %d ---检查GameTaskConfig 和 场景导出",

	lostopenairboxid = "%s 场景中 关卡 %d  %s事件 开启空气墙 缺少id ---检查场景导出 ",
	lostcloseairboxid = "%s 场景中 关卡 %d  %s事件 关闭空气墙 缺少id ---检查场景导出 ",
	lostboxplanid = "%s 场景中 关卡 %d  %s事件 设置空气墙组 缺少id ---检查场景导出 ",

	lostbufftypecfg = "BuffDataV2Config buff id %s 类型 %s 在 ZZBuffTypeCheck 中找不到配置",
	lostbuffparam = "BuffDataV2Config buff id %s 类型 %s 最少参数 %s > 当前extraParam参数数量 %s ",
	buffparamzero = "BuffDataV2Config buff id %s 类型 %s 第 %s 个参数不能为0 ",
	buffparamlostnonzero = "BuffDataV2Config buff id %s 类型 %s 第 %s 个参数不能为0 （当前没填默认0） ",
	buffparamnonzerolist= "BuffDataV2Config buff id %s 类型 %s 所有参数不能为0  检测到第 %s 个参数为 0 ",

	buffparamnotbuff = "BuffDataV2Config buff id %s 类型 %s 第 %s 个参数buffid %s 不在buff表 ",
	buffparambufflost = "BuffDataV2Config buff id %s 类型 %s 第 %s 个参数buffid 缺失 ",
}

Text.EventName = 
{
"场景加载完成",
"区域触发器",
"完成任务",
"回合开始",
"确认传送",
"事件触发器",
}

return Text;