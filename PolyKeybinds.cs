using Terraria.ModLoader;
using Terraria;
using Polyphemalus.Content.NPCs;

namespace Polyphemalus;
public class PolyKeybinds : ModSystem
{
    public static ModKeybind PolyDashKeybind { get; private set; }
    public override void Load()
    {
        PolyKeybinds.PolyDashKeybind = KeybindLoader.RegisterKeybind(base.Mod, "Polypebral Dash", "Mouse4");
    }

    public override void Unload()
    {
        PolyKeybinds.PolyDashKeybind = null;
    }

    
}

public static class npcFinder
{
    public static T ModNPC<T>(this NPC npc) where T : ModNPC
    {
        return npc.ModNPC as T;
    }
}

