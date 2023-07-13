using System.Drawing;
using UGXP.Assets;
using UGXP.Core;
using UGXP.Core.Components;
using UGXP.Core.Render;
using UGXP.Game;
using UGXP.Reference;
using UGXP.Util;

namespace UGXP;

internal class Program {

    static void Main(string[] args) {
        GameProcess game = new GameProcess(new GameSettings {
            Name = "Test",
            WindowSize = new Vector2(1280, 720),
            //WindowSize = new Vector2(1920, 1080),
            FullScreen = GameSettings.FullscreenMode.WINDOWED,
            SortingLayers = new string[] {
                "layer1",
                "layer2"
            }
        });

        AssetManager.Create(new TextureAsset() { 
            Name = "TestTexture",
            Path = "Assets/Sprites/Test.png",
            Pivot = PivotPoint.CENTER
        });
        AssetManager.Create(new TextureAsset() { 
            Name = "Player",
            Path = "Assets/Sprites/Player.png",
            TextureMode = TextureMode.MULTIPLE,
            //Textures = new TextureMap {
            //    { "Idle0", 150*3, 80*2, 150, 80, PivotPoint.BOTTOM_CENTER }
            //}
            Textures = new SpriteSheet(15, 6, 150, 80) {
                { "Idle0", 4, 2, PivotPoint.BOTTOM_CENTER }
            }
        });

        game.SetScenes(new SceneStructure[] {
            new SceneStructure {
                Name = "TestScene",
                Objects = new GameObjectStructure[] {
                    new GameObjectStructure {
                        Name = "Camera",
                        Transform = new Transform {
                            position = new Vector2(0,0)
                        },
                        Components = new LazyComponent[] {
                            Component.Create<Camera>(camera => {
                                camera.orthographicSize = 7f;
                            })
                        }
                    },
                    new GameObjectStructure {
                        Name = "Parent GO",
                        Transform = new Transform {
                            position = new Vector2(0, 0),
                            rotation = new OpenTK.Mathematics.Vector3(0,0,0),
                            scale = new Vector2(1, 2)
                        },
                        Children = new GameObjectStructure[] {
                            new GameObjectStructure {
                                Name = "GameObject child test",
                                Transform = new Transform() {
                                    position = new Vector2(0,1),
                                    rotation = new OpenTK.Mathematics.Vector3(0,0,0),
                                    scale = new Vector2(2, 1)
                                },
                                Components = new LazyComponent[] {
                                    Component.Create<SpriteRenderer>(spriteRenderer => {
                                        spriteRenderer.Sprite = AssetManager.Get<Sprite>("Player", "Idle0");
                                        spriteRenderer.color = new Color32(125, 20, 40, 255);
                                    })
                                }
                            }
                        }
                    },
                    new GameObjectStructure {
                        Name = "GameObject rhino behind",
                        Transform = new Transform() {
                            position = new Vector2(0,0),
                            rotation = new OpenTK.Mathematics.Vector3(45,45,45),
                        },
                        Components = new LazyComponent[] {
                            Component.Create<SpriteRenderer>(spriteRenderer => {
                                spriteRenderer.Sprite = AssetManager.Get<Sprite>("TestTexture");
                                spriteRenderer.sortingLayer = SortingLayer.NameToId("Default");
                            })
                        }
                    },
                    new GameObjectStructure {
                        Name = "GameObject player test",
                        Transform = new Transform() {
                            position = new Vector2(0,0),
                            rotation = new OpenTK.Mathematics.Vector3(0,45f,0),
                            scale = new Vector2(1, 1)
                        },
                        Components = new LazyComponent[] {
                            Component.Create<SpriteRenderer>(spriteRenderer => {
                                spriteRenderer.Sprite = AssetManager.Get<Sprite>("Player", "Idle0");
                                spriteRenderer.color = new Color32(255, 255, 255, 255);
                                spriteRenderer.sortingLayer = SortingLayer.NameToId("layer2");
                            })
                        }
                    },
                    new GameObjectStructure {
                        Name = "GameObject rhino unmodified",
                        Transform = new Transform() {
                            position = new Vector2(1,0),
                            rotation = new OpenTK.Mathematics.Vector3(45,45,45),
                        },
                        Components = new LazyComponent[] {
                            Component.Create<SpriteRenderer>(spriteRenderer => {
                                spriteRenderer.Sprite = AssetManager.Get<Sprite>("TestTexture");
                                spriteRenderer.sortingLayer = SortingLayer.NameToId("layer1");
                            })
                        }
                    }
                }
            }
        });
        game.Start();
    }
}