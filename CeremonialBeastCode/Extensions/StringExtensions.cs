using Godot;

namespace CeremonialBeast.CeremonialBeastCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    private static string ModResourcePath(string relativePath) =>
        $"{MainFile.ResPath.TrimEnd('/', '\\')}/{relativePath.TrimStart('/', '\\').Replace('\\', '/')}";

    public static string ImagePath(this string path)
    {
        return ModResourcePath($"images/{path}");
    }

    public static string CardImagePath(this string path)
    {
        path = ModResourcePath($"images/card_portraits/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find card image path: " + path);
        return ModResourcePath("images/card_portraits/card.png");
    }

    public static string BigCardImagePath(this string path)
    {
        path = ModResourcePath($"images/card_portraits/big/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big card image path: " + path);
        return ModResourcePath("images/card_portraits/big/card.png");
    }

    public static string PowerImagePath(this string path)
    {
        path = ModResourcePath($"images/powers/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find power image path: " + path);
        return ModResourcePath("images/powers/power.png");
    }

    public static string BigPowerImagePath(this string path)
    {
        path = ModResourcePath($"images/powers/big/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big power image path: " + path);
        return ModResourcePath("images/powers/big/power.png");
    }

    public static string RelicImagePath(this string path)
    {
        path = ModResourcePath($"images/relics/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find relic image path: " + path);
        return ModResourcePath("images/relics/relic.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        path = ModResourcePath($"images/relics/big/{path}");
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big relic image path: " + path);
        return ModResourcePath("images/relics/big/relic.png");
    }

    public static string CharacterUiPath(this string path)
    {
        return ModResourcePath($"images/charui/{path}");
    }
}
