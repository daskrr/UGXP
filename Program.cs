using UGXP.Core;
using UGXP.Game;

namespace UGXP;

internal class Program {

    static void Main(string[] args) {
        GameProcess game = new GameProcess(new GameSettings {
            Name = "Test",
            GameSize = new Vector2(400, 400),
            WindowSize = new Vector2(1920, 1080),
            FullScreen = GameSettings.FullscreenMode.WINDOWED
        });

        game.SetScenes(new SceneStructure[] {
            new SceneStructure {
                Name = "TestScene",
                Objects = new GameObjectStructure[] {
                    new GameObjectStructure {
                        Name = "GameObject test",
                    }
                }
            }
        });
        game.Start();
    }
}