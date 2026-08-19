using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using CeremonialBeast.CeremonialBeastCode.Character;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using Godot;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

[Pool(typeof(CeremonialBeastRelicPool))]
public abstract class CeremonialBeastRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}