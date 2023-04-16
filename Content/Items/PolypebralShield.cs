using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using System.Linq;

namespace Polyphemalus.Content.Items
{
    public class PolypebralShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Polypebral Shield");
            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            base.Item.accessory = true;
            Item.defense = 8;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PolyPlayer pplayer = player.GetModPlayer<PolyPlayer>();
            pplayer.PolyShieldChargeEnabled = true;
            player.dashType = 0;

        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<string> hkList = PolyKeybinds.PolyDashKeybind.GetAssignedKeys();
            string hotkey = "";
            if (hkList.Count == 0 || hkList == null)
            {
                hotkey = "[NONE]";
            } else
            {
                hotkey = "["+hkList[0]+"]";
            }
            
            TooltipLine line = list.FirstOrDefault((TooltipLine x) => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (line != null)
            {
                line.Text = "Press " + hotkey + " to perform a omnidirectional dash that can be used to bonk enemies.\nHas a long cooldown. \nInitiating another dash right before hitting an enemy will allow you to control your ricochet";
            }
        }
    }
}