using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace CeremonialBeast.CeremonialBeastCode.Relics;

public class BeastBoneFragment : CeremonialBeastRelic
{
    private bool _isActivating;
    private int _hpLost;

    // 设置稀有度为稀有 (Rare)
    public override RelicRarity Rarity => RelicRarity.Rare;

    // ==========================================
    // ✨ UI 控制：允许在遗物下方显示计数器
    // ==========================================
    public override bool ShowCounter => true;

    public override int DisplayAmount
    {
        get
        {
            // 如果正在触发特效，短暂显示目标阈值（12）
            if (!IsActivating)
            {
                return HpLost % base.DynamicVars["HpThreshold"].IntValue;
            }
            return base.DynamicVars["HpThreshold"].IntValue;
        }
    }

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            AssertMutable();
            _isActivating = value;
            UpdateDisplay();
        }
    }

    // ==========================================
    // ✨ 核心机制 1：跨战斗/跨档保存的计数器
    // ==========================================
    [SavedProperty]
    public int HpLost
    {
        get => _hpLost;
        set
        {
            AssertMutable();
            _hpLost = value;
            UpdateDisplay();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("HpThreshold", 12m),
        new DynamicVar("MaxHpGain", 1m)
    };

    private void UpdateDisplay()
    {
        if (IsActivating)
        {
            base.Status = RelicStatus.Normal;
        }
        else
        {
            int threshold = base.DynamicVars["HpThreshold"].IntValue;
            // 当累计失去的生命等于 阈值-1 (11) 时，遗物持续闪烁 (Active)
            base.Status = ((HpLost == threshold - 1) ? RelicStatus.Active : RelicStatus.Normal);
        }
        InvokeDisplayAmountChanged();
    }

    // ==========================================
    // ✨ 核心机制 2：在受到伤害时拦截并累计
    // ==========================================
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 确保是玩家自己受到伤害，且受到了真实的未格挡伤害
        if (target == base.Owner?.Creature && result.UnblockedDamage > 0)
        {
            int damageTaken = (int)result.UnblockedDamage;
            HpLost += damageTaken;

            int threshold = base.DynamicVars["HpThreshold"].IntValue;
            int maxHpGain = base.DynamicVars["MaxHpGain"].IntValue;

            if (HpLost >= threshold)
            {
                // 计算满足了几次阈值（防止玩家单次受到极高伤害，例如一次掉 25 血，应该获得 2 点最大生命，保留 1 点计数）
                int timesToTrigger = HpLost / threshold;
                
                // 触发遗物闪现特效
                _ = TaskHelper.RunSafely(DoActivateVisuals());

                // 增加最大生命值指令
                int totalMaxHpToGain = timesToTrigger * maxHpGain;
                await CreatureCmd.GainMaxHp(base.Owner.Creature, totalMaxHpToGain);

                // 保留溢出的伤害值计数
                HpLost %= threshold;
            }
            UpdateDisplay();
        }
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}