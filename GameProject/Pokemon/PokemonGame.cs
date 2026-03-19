using System;
using System.Net.NetworkInformation;
using Framework.Engine;

public class PokemonGame : GameApp
{
    private readonly SceneManager<Scene> _scenes = new SceneManager<Scene>();

    public PokemonGame() : base(60, 30)
    {
    }

    protected override void Initialize()
    {
        ChangeToTitle();
    }

    protected override void Update(float deltaTime)
    {
        _scenes.CurrentScene?.Update(deltaTime);
    }
    protected override void Draw()
    {
        _scenes.CurrentScene?.Draw(Buffer);
    }

    private void ChangeToTitle()
    {
        var title = new TitleScene();
        title.StartRequested += ChangeToPlay;
        _scenes.ChangeScene(title);
    }

    private void ChangeToPlay()
    {
        var play = new PlayScene();
        _scenes.ChangeScene(play);
    }

    private void ChageToBattle()
    {

    }
}