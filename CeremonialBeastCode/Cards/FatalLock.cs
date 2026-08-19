using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using CeremonialBeast.CeremonialBeastCode.Enums; // 确保引入 CustomTags 所在的命名空间
using CeremonialBeast.CeremonialBeastCode.Powers;

namespace CeremonialBeast.CeremonialBeastCode.Cards;

public sealed class FatalLock : CeremonialBeastCard
{
    // 注册 Ringing 标签
    protected override HashSet<CardTag> CanonicalTags => [CustomTags.Ringing];

    // 定义我们自定义的变量名 Key
    private const string _multiplier = "Multiplier";

    // 使用官方的标准写法：直接 new DynamicVar(字符串Key, 初始数值)
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar(_multiplier, 2m)
    };

    public FatalLock()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlayCard(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 通过我们定义的 Key 来安全获取数值，并传递给 Power
        var power = await PowerCmd.Apply<FatalLockPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        if (power != null)
        {
            power.Multiplier = base.DynamicVars[_multiplier].BaseValue;
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后增加 0.5，倍率变为 2.5 倍
        base.DynamicVars[_multiplier].UpgradeValueBy(0.5m);
    }
}
