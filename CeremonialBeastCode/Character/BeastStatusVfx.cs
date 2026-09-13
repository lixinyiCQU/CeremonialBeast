using Godot;
using CeremonialBeast.CeremonialBeastCode.Powers;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace CeremonialBeast.CeremonialBeastCode.Character;

public partial class BeastStatusVfx : Node2D
{
    public Creature Creature { get; set; } = null!;
    private Node2D? _skulls;
    private Node2D? _form;
    private bool _lowHealth;
    private bool _hasForm;

    public override void _Ready()
    {
        ZIndex = -1;
    }

    public override void _Process(double delta)
    {
        bool lowHealth = Creature.IsAlive && Creature.CurrentHp * 4m <= Creature.MaxHp;
        bool hasForm = Creature.IsAlive && Creature.HasPower<CeremonialFormPower>();
        if (lowHealth != _lowHealth)
        {
            _lowHealth = lowHealth;
            if (lowHealth)
            {
                // Load the base-game skeleton at runtime so export cannot strip its reference.
                _skulls = (Node2D)ClassDB.Instantiate("SpineSprite").AsGodotObject();
                _skulls.Set("skeleton_data_res", GD.Load<Resource>("res://animations/backgrounds/ceremonial_beast/bg_ceremonial_beast_top_skel_data.tres"));
                _skulls.Set("additive_material", new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add });
                _skulls.Position = new Vector2(-407, -422);
                _skulls.Scale = new Vector2(0.14f, 0.14f);
                AddChild(_skulls);
                using var sprite = new MegaSprite(_skulls);
                using var animation = sprite.GetAnimationState();
                animation.SetAnimation("skulls_spawn", loop: false);
                animation.AddAnimation("glow_and_skulls_idle");
            }
            else
            {
                _skulls?.QueueFree();
                _skulls = null;
            }
        }
        if (hasForm != _hasForm)
        {
            _hasForm = hasForm;
            if (hasForm)
            {
                // Keep the original scene's fixed center; the beast has different bones.
                _form = GD.Load<PackedScene>("res://scenes/vfx/forms/reaper/vfx_reaper_form_idle_vfx.tscn").Instantiate<Node2D>();
                AddChild(_form);
            }
            else
            {
                _form?.QueueFree();
                _form = null;
            }
        }
    }
}
