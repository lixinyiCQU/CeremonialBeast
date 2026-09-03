using System;
using System.Collections.Generic;
using Godot;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using CeremonialBeast.CeremonialBeastCode.Extensions;
using CeremonialBeast.CeremonialBeastCode.Cards;
using CeremonialBeast.CeremonialBeastCode.Relics;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace CeremonialBeast.CeremonialBeastCode.Character;

public class CeremonialBeast : PlaceholderCharacterModel
{
    public const string CharacterId = "CeremonialBeast";
    public const string PlowSfx = "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_plow";
    public const string PlowEndSfx = "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_plow_end";
    public const string ShrillSfx = "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_shrill";
    public const string StunSfx = "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_stun";
    public const string DeathSfxPath = "event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_die";
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    
    // 初始属性配置
    public override int StartingHp => 86;
    public override int StartingGold => 99;
    
    // 初始卡组配置
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<Defend>(),
        ModelDb.Card<CeremonialStomp>(),
        ModelDb.Card<Plowing>()
    ];

    // 初始遗物配置
    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<CeremonialSoil>()
    ];
    
    // 资源池绑定
    public override CardPoolModel CardPool => ModelDb.CardPool<CeremonialBeastCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<CeremonialBeastRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<CeremonialBeastPotionPool>();

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_rock_shatter"
    ];
    
    // ==========================================
    // UI 与选人界面资产映射
    // ==========================================
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    
    public override string CustomIconTexturePath => "ceremonial_beast_boss.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "res://CeremonialBeast/images/charui/ceremonial_beast_select.png";
    public override string CustomCharacterSelectLockedIconPath => "res://CeremonialBeast/images/charui/ceremonial_beast_select.png";
    public override string CustomMapMarkerPath => "ceremonial_beast_boss.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "res://CeremonialBeast/images/charui/ceremonial_beast_select_bg.tscn";
    
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        var visuals = NodeFactory<NCreatureVisuals>.CreateFromScene("res://scenes/creature_visuals/ceremonial_beast.tscn");
        if (visuals == null)
        {
            return null;
        }

        if (visuals.FindChild("FormVfx", recursive: true, owned: false) == null)
        {
            Control formVfxHolder = new()
            {
                Name = "FormVfx",
                UniqueNameInOwner = true,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            visuals.AddChild(formVfxHolder);
            formVfxHolder.Owner = visuals;
        }

        var visualsNode = visuals.GetNodeOrNull<Node2D>("Visuals");
        if (visualsNode != null)
        {
            visualsNode.Scale = new Vector2(-0.45f, 0.45f);
        }
        else
        {
            visuals.Scale = new Vector2(-0.45f, 0.45f);
        }

        return visuals;
    }

    public override CreatureAnimator GenerateAnimator(
        MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite controller,
        Creature creature)
    {
        AnimState idleState = new AnimState("idle_loop", isLooping: true);
        AnimState castState = new AnimState("shrill");
        
        AnimState attackState = new AnimState("attack");
        AnimState hitState = new AnimState("hurt");
        AnimState deadState = new AnimState("die");
        AnimState plowEndDeadState = new AnimState("plow_end_die");
        
        AnimState stunState = new AnimState("stun");
        AnimState stunLoopState = new AnimState("stun_loop", isLooping: true);
        AnimState wakeUpState = new AnimState("wake_up");

        AnimState plowState = new AnimState("plow");
        AnimState plowEndState = new AnimState("plow_end");

        idleState.AddBranch("Plow", plowState);
        castState.NextState = idleState;
        
        attackState.NextState = idleState;
        hitState.NextState = idleState;
        
        stunState.NextState = stunLoopState;   
        wakeUpState.NextState = idleState;    
        plowState.AddBranch("EndPlow", plowEndState);
        plowEndState.NextState = idleState;

        CreatureAnimator creatureAnimator = new CreatureAnimator(idleState, controller);
        
        creatureAnimator.AddAnyState("Cast", castState);
        creatureAnimator.AddAnyState("Attack", attackState);
        creatureAnimator.AddAnyState("Hit", hitState);
        
        creatureAnimator.AddAnyState("Stun", stunState);
        creatureAnimator.AddAnyState("Unstun", wakeUpState);

        creatureAnimator.AddAnyState("Plow", plowState);
        creatureAnimator.AddAnyState("PlowCharge", plowState);
        creatureAnimator.AddAnyState("PlowHit", hitState);
        creatureAnimator.AddAnyState("Dead", deadState);
        creatureAnimator.AddAnyState("PlowDead", plowEndDeadState);

        idleState.AddBranch("Hit", hitState);
        castState.AddBranch("Hit", hitState);
        hitState.AddBranch("Hit", hitState);
        attackState.AddBranch("Hit", hitState);
        plowState.AddBranch("Hit", hitState);
        plowEndState.AddBranch("Hit", hitState);
        stunState.AddBranch("Hit", hitState);
        stunLoopState.AddBranch("Hit", hitState);
        wakeUpState.AddBranch("Hit", hitState);
        
        return creatureAnimator;
    }
}
