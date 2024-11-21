import { Scene } from 'phaser';
import { Connection } from '../SignalR/Connection';

export class Preloader extends Scene {
  constructor() {
    super('Preloader');
  }

  init() {
    //  We loaded this image in our Boot Scene, so we can display it here
    this.add.image(512, 384, 'background');

    //  A simple progress bar. This is the outline of the bar.
    this.add.rectangle(512, 384, 468, 32).setStrokeStyle(1, 0xffffff);

    //  This is the progress bar itself. It will increase in size from the left based on the % of progress.
    const bar = this.add.rectangle(512 - 230, 384, 4, 28, 0xffffff);

    //  Use the 'progress' event emitted by the LoaderPlugin to update the loading bar
    this.load.on('progress', (progress: number) => {

      //  Update the progress bar (our bar is 464px wide, so 100% = 464px)
      bar.width = 4 + (460 * progress);

    });
  }

  preload() {

    this.load.audio("boom", [ 'assets/audio/boom.mp3', 'audio/boom.ogg', 'audio/boom.m4a' ]);   
    this.load.audio("kill", [ 'assets/audio/kill.mp3' ]);  
    this.load.audio("1up", [ 'assets/audio/1up.mp3' ]);  
    //  Load the assets for the game - Replace with your own assets
    this.load.setPath('assets');

    this.load.image('logo', 'logo.png');
    this.load.image('star', 'star.png');
    this.load.image('1up', '1up.png');
    this.load.image("borders", "borders.png");
    this.load.image("backgroundtile", "backgroundtile.png");
      
    this.load.spritesheet("playerSprite", "player.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

      this.load.spritesheet("playerSprite3", "player3.png",
          {
              frameHeight: 63,
              frameWidth: 63
          }
      );
      
    this.load.spritesheet("playerSprite2", "player2.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

    this.load.spritesheet("solidWall", "SolidWall.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

    this.load.spritesheet("destructiveWall", "DestructiveWall.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

    this.load.spritesheet("bomb", "bomb.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

      this.load.spritesheet("fire", "fire.png",
          {
              frameHeight: 63,
              frameWidth: 63
          }
      );

    this.load.spritesheet("dead", "dead.png",
      {
        frameHeight: 63,
        frameWidth: 63
      }
    );

      this.load.spritesheet("monster", "monster.png",
          {
              frameHeight: 63,
              frameWidth: 63
          }
      );

    this.loadTiledSheet("otsp_tiles_01");
    this.loadTiledSheet("otsp_town_01");
  }

  loadTiledSheet(nameSheet: string) {
    this.load.image(nameSheet, nameSheet + ".png");

    this.load.once("filecomplete-image-" + nameSheet, () => {
      const tex = this.textures.get(nameSheet);

      // add(name, sourceIndex, x, y, width, height)
      for (let j = 0; j < 64; j++)
        for (let i = 0; i < 16; i++) {
          const frame = tex.add(i + j * 16, 0, i * 32, j * 32, 32, 32);
        }
    });
  }

  create() {
    //  When all the assets have loaded, it's often worth creating global objects here that the rest of the game can use.
    //  For example, you can define global animations here, so we can use them in other scenes.

    //  Move to the MainMenu. You could also swap this for a Scene Transition, such as a camera fade.
    Connection.CreateConnection().
      then(() => {
        this.loadAnims();
        this.scene.start('Lobby');
      })
  }

  loadAnims() {
    this.anims.create({
      key: "playerSprite_walkRight",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [8, 9, 10, 11],
      }),
      frameRate: 10,
      repeat: -1,
    });

    this.anims.create({
      key: "playerSprite_walkLeft",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [4, 5, 6, 7],
      }),
      frameRate: 10,
      repeat: -1,
    });

    this.anims.create({
      key: "playerSprite_walkUp",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [12, 13, 14, 15],
      }),
      frameRate: 20,
    });

    this.anims.create({
      key: "playerSprite_walkDown",
      frames: this.anims.generateFrameNumbers("playerSprite", {
        frames: [0, 1, 2, 3],
      }),
      frameRate: 20,
    });

    this.anims.create({
      key: "playerSprite_turn",
      frames: [{ key: "playerSprite", frame: 1 }],
      frameRate: 20,
    });

    ///

    this.anims.create({
      key: "playerSprite2_walkRight",
      frames: this.anims.generateFrameNumbers("playerSprite2", {
        frames: [8, 9, 10, 11],
      }),
      frameRate: 7,
      repeat: -1,
    });

    this.anims.create({
      key: "playerSprite2_walkLeft",
      frames: this.anims.generateFrameNumbers("playerSprite2", {
        frames: [4, 5, 6, 7],
      }),
      frameRate: 7,
      repeat: -1,
    });

    this.anims.create({
      key: "playerSprite2_walkUp",
      frames: this.anims.generateFrameNumbers("playerSprite2", {
        frames: [12, 13, 14, 15],
      }),
      frameRate: 7,
    });

    this.anims.create({
      key: "playerSprite2_walkDown",
      frames: this.anims.generateFrameNumbers("playerSprite2", {
        frames: [0, 1, 2, 3],
      }),
      frameRate: 7,
    });

    this.anims.create({
      key: "playerSprite2_turn",
      frames: [{ key: "playerSprite2", frame: 1 }],
      frameRate: 7,
    });
    
    ////


      this.anims.create({
          key: "playerSprite3_walkRight",
          frames: this.anims.generateFrameNumbers("playerSprite3", {
              frames: [8, 9, 10, 11],
          }),
          frameRate: 7,
          repeat: -1,
      });

      this.anims.create({
          key: "playerSprite3_walkLeft",
          frames: this.anims.generateFrameNumbers("playerSprite3", {
              frames: [4, 5, 6, 7],
          }),
          frameRate: 7,
          repeat: -1,
      });

      this.anims.create({
          key: "playerSprite3_walkUp",
          frames: this.anims.generateFrameNumbers("playerSprite3", {
              frames: [12, 13, 14, 15],
          }),
          frameRate: 7,
      });

      this.anims.create({
          key: "playerSprite3_walkDown",
          frames: this.anims.generateFrameNumbers("playerSprite3", {
              frames: [0, 1, 2, 3],
          }),
          frameRate: 7,
      });

      this.anims.create({
          key: "playerSprite3_turn",
          frames: [{ key: "playerSprite3", frame: 1 }],
          frameRate: 7,
      });
  }

  changeScene() {
    //this.scene.start('GameLevel');
  }
}
