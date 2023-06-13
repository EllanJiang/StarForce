--[[
* @file:   ZZConfigCheck
* @brief:  这个文件是通过工具自动生成的，建议不要手动修改
]]--

local ZZConfigCheckTable = {
	{lineNumber = 1, configName = 'ZombieMonsterData', fieldName = 'MonsterId', relyConfigName = 'MonsterData', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 2, configName = 'ChickenMonsterData', fieldName = 'MonsterDataID', relyConfigName = 'MonsterData', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 3, configName = 'ZombieMonsterData', fieldName = 'Level', relyConfigName = 'MonsterLevelData', relyFieldName = 'id', relyParam = {'Level', '100000', 'MonsterId', '1'}, relyType = 2, skipZero = false}, 
	{lineNumber = 4, configName = 'ChickenMonsterData', fieldName = 'Level', relyConfigName = 'MonsterLevelData', relyFieldName = 'id', relyParam = {'Level', '100000', 'MonsterDataID', '1'}, relyType = 2, skipZero = false}, 
	{lineNumber = 5, configName = 'ChickenMonsterData', fieldName = 'AddChickenIDList', relyConfigName = 'ChickenMonsterData', relyFieldName = 'id', relyParam = {}, relyType = 3, skipZero = false}, 
	{lineNumber = 6, configName = 'MonsterLevelData', fieldName = 'MonsterId', relyConfigName = 'MonsterData', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 7, configName = 'MonsterData', fieldName = 'id', relyConfigName = 'MonsterLevelData', relyFieldName = 'MonsterId', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 8, configName = 'RogueLikeMonsterSpawnConfig', fieldName = 'crowd', relyConfigName = 'ZombieMonsterData', relyFieldName = 'id', relyParam = {'id'}, relyType = 4, skipZero = false}, 
	{lineNumber = 9, configName = 'RogueLikeShopItemsGroup', fieldName = 'shopId', relyConfigName = 'RogueLikeShopItems', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 10, configName = 'RogueLikeTalent', fieldName = 'buffId', relyConfigName = 'BuffDataV2Config', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 11, configName = 'BuffDataV2Config', fieldName = 'attributeModifier', relyConfigName = 'AttributeConfig', relyFieldName = 'id', relyParam = {'attributeId'}, relyType = 4, skipZero = true}, 
	{lineNumber = 12, configName = 'WeaponDataConfig', fieldName = 'weaponConfigName', relyConfigName = 'WeaponDataConfig', relyFieldName = 'weaponConfigName', relyParam = {'define/json_config/', '.json', 'weapon'}, relyType = 5, skipZero = false}, 
	{lineNumber = 13, configName = 'WeaponDataConfig', fieldName = 'icon', relyConfigName = 'ItemResource', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 14, configName = 'GunFunctionQualityGropConfig', fieldName = 'ID', relyConfigName = 'GunFunctionQualityGropConfig', relyFieldName = 'ID', relyParam = {'ID', 'qualityLevel'}, relyType = 6, skipZero = false}, 
	{lineNumber = 15, configName = 'ShopToModeMapConfig', fieldName = 'shopTabId', relyConfigName = 'ShopShowItems', relyFieldName = 'shopTabId', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 16, configName = 'SkillGroupChooseConfig', fieldName = 'skillList', relyConfigName = 'PlayerSkillConfig', relyFieldName = 'id', relyParam = {}, relyType = 3, skipZero = false}, 
	{lineNumber = 17, configName = 'SkillGroupChooseConfig', fieldName = 'DefaultInit', relyConfigName = 'PlayerSkillConfig', relyFieldName = 'id', relyParam = {'skillId'}, relyType = 4, skipZero = true}, 
	{lineNumber = 18, configName = 'ProfessionModeConfig', fieldName = 'skillGroup', relyConfigName = 'SkillGroupChooseConfig', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 19, configName = 'GunBuffListConfig', fieldName = 'BuffDes', relyConfigName = 'GunBuffListConfig', relyFieldName = 'BuffDes', relyParam = {}, relyType = 7, skipZero = true}, 
	{lineNumber = 20, configName = 'ArmsRaceConfig', fieldName = 'id', relyConfigName = 'ArmsRaceConfig', relyFieldName = 'id', relyParam = {'Grade', 'WeaponId'}, relyType = 6, skipZero = false}, 
	{lineNumber = 21, configName = 'BRShopItemGroupConfig', fieldName = 'itemId', relyConfigName = 'BRItemConfig', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 22, configName = 'BRShopLotteryConfig', fieldName = 'itemId', relyConfigName = 'BRItemConfig', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 23, configName = 'BRShopConfig', fieldName = 'ItemGroupId', relyConfigName = 'BRShopItemGroupConfig', relyFieldName = 'itemGroupId', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 24, configName = 'BRShopConfig', fieldName = 'lotteryGroupId', relyConfigName = 'BRShopLotteryConfig', relyFieldName = 'itemGroupId', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 25, configName = 'BRShopConfig', fieldName = 'flashSale', relyConfigName = 'BRShopItemGroupConfig', relyFieldName = 'itemGroupId', relyParam = {'groupId'}, relyType = 4, skipZero = false}, 
	{lineNumber = 26, configName = 'BRSkillGroupConfig', fieldName = 'skillId', relyConfigName = 'PlayerSkillConfig', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 27, configName = 'BRSkillCardConfig', fieldName = 'initSkill', relyConfigName = 'PlayerSkillConfig', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = false}, 
	{lineNumber = 28, configName = 'BRSkillCardConfig', fieldName = 'initItem', relyConfigName = 'BRItemConfig', relyFieldName = 'id', relyParam = {'id'}, relyType = 4, skipZero = false}, 
	{lineNumber = 29, configName = 'WeaponGroupCard', fieldName = 'shopShowItem', relyConfigName = 'ShopShowItems', relyFieldName = 'id', relyParam = {}, relyType = 1, skipZero = true}
}
return ZZConfigCheckTable