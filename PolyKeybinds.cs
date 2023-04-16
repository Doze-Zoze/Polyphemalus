using Terraria.ModLoader;

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

