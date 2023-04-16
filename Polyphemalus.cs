using Polyphemalus.Content.NPCs;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;

namespace Polyphemalus
{
	public class Polyphemalus : Mod
	{
	}

	public class LastPolyBeaten : IItemDropRuleCondition, IProvideItemConditionDescription
	{
        public bool CanDrop(DropAttemptInfo info)
        {

            List<int> type =  new List<int>() { 
                ModContent.NPCType<Cataractacomb>(),
                ModContent.NPCType<Exotrexia>(),
                ModContent.NPCType<Conjunctivirus>(),
                ModContent.NPCType<Astigmageddon>()
            };

            type.Remove(info.npc.type);
            foreach (var item in type)
            {
                if (NPC.AnyNPCs(item))
                {
                    return false;
                }
            }
            return true;
        }
            public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
}