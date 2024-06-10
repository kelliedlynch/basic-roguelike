using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Graphics.Layout;
using Roguelike.Map;
using Roguelike.UserInterface;
using Roguelike.Utility;

namespace Roguelike.Graphics.UserInterface;

public class InfoBar : SpritePanel
{
    private int _playerMoney;
    private int _dungeonLevel;
    private int _playerAtk;

    private readonly IntVector2 _moneyIconLoc = new IntVector2(35, 16);
    private readonly IntVector2 _stairsIconLoc = new IntVector2(2, 6);
    private readonly IntVector2 _heartIconLoc = new IntVector2(39, 10);
    private readonly IntVector2 _swordIconLoc = new IntVector2(32, 8);
    
    private HStackContainer _layoutContainer;
    private Container _mIconContainer;
    private TextLabel _mLabel;
    private Container _sIconContainer;
    private TextLabel _sLabel;
    private Container _hIconContainer;
    private TextLabel _hLabel;
    private Container _swIconContainer;
    private TextLabel _swLabel;
    
    
    
    
    public InfoBar(Game game) : base(game)
    {
        ContentAlignment = Alignment.Center;
        Sizing = AxisSizing.ExpandXFixedY;
        AssignedSize = new IntVector2(0, 64);
        BuildLayout();
    }

    public void BuildLayout()
    {
        // RemoveChildren();
        
        _layoutContainer = new HStackContainer(Game);
        _layoutContainer.Sizing = AxisSizing.ExpandXExpandY;
        _layoutContainer.ContentAlignment = Alignment.Center;
        _layoutContainer.Debug = true;
        _layoutContainer.Outline.BorderColor = Color.White;
        _layoutContainer.ChildSpacing = 10;
        AddChild(_layoutContainer);

        _mIconContainer = new Container(Game);
        _mIconContainer.Sizing = AxisSizing.FixedXFixedY;
        _mIconContainer.AssignedSize = TileSize;
        _mIconContainer.Debug = true;
        _layoutContainer.AddChild(_mIconContainer);

        _mLabel = new TextLabel(Game);
        _mLabel.Debug = true;
        _mLabel.Outline.BorderColor = Color.Orange;
        _layoutContainer.AddChild(_mLabel);
        
        _sIconContainer = new Container(Game);
        _sIconContainer.Sizing = AxisSizing.FixedXFixedY;
        _sIconContainer.AssignedSize = TileSize;
        _sIconContainer.Debug = true;
        _layoutContainer.AddChild(_sIconContainer);
        
        _sLabel = new TextLabel(Game);
        _sLabel.Debug = true;
        _sLabel.Outline.BorderColor = Color.Orange;
        _layoutContainer.AddChild(_sLabel);

        _hIconContainer = new Container(Game);
        _hIconContainer.Sizing = AxisSizing.FixedXFixedY;
        _hIconContainer.AssignedSize = TileSize;
        _hIconContainer.Debug = true;
        _layoutContainer.AddChild(_hIconContainer);
        
        _hLabel = new TextLabel(Game);
        _hLabel.Debug = true;
        _hLabel.Outline.BorderColor = Color.Orange;
        _layoutContainer.AddChild(_hLabel);
        
        _swIconContainer = new Container(Game);
        _swIconContainer.Sizing = AxisSizing.FixedXFixedY;
        _swIconContainer.AssignedSize = TileSize;
        _swIconContainer.Debug = true;
        _layoutContainer.AddChild(_swIconContainer);

        _swLabel = new TextLabel(Game);
        _swLabel.Debug = true;
        _swLabel.Outline.BorderColor = Color.Orange;
        _layoutContainer.AddChild(_swLabel);

    }

    public override void LayoutElements()
    {
        base.LayoutElements();
    }

    public override void Update(GameTime gameTime)
    {
        var player = Game.Services.GetService<PlayerManager>().Player;
        var dungeonLevel = Game.Services.GetService<LevelManager>().CurrentLevelNumber;
        var font = Game.Services.GetService<SpriteFont>();
        _mLabel.Text = player.Money.ToString();

        _sLabel.Text = dungeonLevel.ToString();
        
        _hLabel.Text = player.Hp.ToString();
        
        _swLabel.Text = player.CalculatedAtk.ToString();
        
        _dungeonLevel = dungeonLevel;
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // BuildPanel();
        base.Draw(gameTime);
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        var mIconRect = new Rectangle(_moneyIconLoc * TileSize, TileSize);
        var sIconRect = new Rectangle(_stairsIconLoc * TileSize, TileSize);
        var hIconRect = new Rectangle(_heartIconLoc * TileSize, TileSize);
        var swIconRect = new Rectangle(_swordIconLoc * TileSize, TileSize);
        
        var texture = Game.Content.Load<Texture2D>("Graphics/monochrome-transparent_packed");
        spriteBatch.Draw(texture, _mIconContainer.Bounds, mIconRect, Color.Gold);
        spriteBatch.Draw(texture, _sIconContainer.Bounds, sIconRect, Color.Tan);
        spriteBatch.Draw(texture, _hIconContainer.Bounds, hIconRect, Color.Red);
        spriteBatch.Draw(texture, _swIconContainer.Bounds, swIconRect, Color.Silver);
        
        
    }
}