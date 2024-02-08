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
        Size = new IntVector2(0, 64);
        BuildLayout();
    }

    public void BuildLayout()
    {
        // RemoveChildren();
        
        _layoutContainer = new HStackContainer(Game);
        _layoutContainer.Sizing = AxisSizing.ExpandXExpandY;
        _layoutContainer.ContentAlignment = Alignment.Center;
        AddChild(_layoutContainer);

        _mIconContainer = new Container(Game);
        _mIconContainer.Sizing = AxisSizing.FixedXFixedY;
        _mIconContainer.Size = TileSize;
        _layoutContainer.AddChild(_mIconContainer);

        _mLabel = new TextLabel(Game);
        _layoutContainer.AddChild(_mLabel);

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
        var mSize = font.MeasureString(_mLabel.Text).ToIntVector2();
        
        _playerAtk = player.CalculatedAtk;
        _dungeonLevel = dungeonLevel;
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        // BuildPanel();
        var spriteBatch = Game.Services.GetService<SpriteBatch>();
        var mIconRect = new Rectangle(_moneyIconLoc * TileSize, TileSize);
        var sIconRect = new Rectangle(_stairsIconLoc * TileSize, TileSize);
        var hIconRect = new Rectangle(_heartIconLoc * TileSize, TileSize);
        var swIconRect = new Rectangle(_swordIconLoc * TileSize, TileSize);
        
        var texture = Game.Content.Load<Texture2D>(SpriteSheet);
        spriteBatch.Draw(texture, _mIconContainer.Bounds, mIconRect, Color.Gold);
        // base.Draw(gameTime);
        
    }
}