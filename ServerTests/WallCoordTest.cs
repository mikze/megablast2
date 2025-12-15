using Game.Game.Entities;

namespace ServerTests;

public class WallCoordTest
{
    [Test]
    public void Coord_Should_Map_Pixels_To_Tiles_On_Boundaries()
    {
        // Arrange: 100px,150px with tileSize 50 => (2,3)
        var wall = new Wall(null, true)
        {
            PosX = 100,
            PosY = 150
        };

        // Act
        var (x, y) = wall.Coord;

        // Assert
        Assert.That(x, Is.EqualTo(2));
        Assert.That(y, Is.EqualTo(3));
    }

    [Test]
    public void Coord_Should_Floor_Division_For_Non_Exact_Pixels()
    {
        // Arrange: casting to int before division should floor 149.9->149 and 199.9->199
        // 149/50 = 2, 199/50 = 3
        var wall = new Wall(null, true)
        {
            PosX = 149.9,
            PosY = 199.9
        };

        // Act
        var coord = wall.Coord;

        // Assert
        Assert.That(coord.x, Is.EqualTo(2));
        Assert.That(coord.y, Is.EqualTo(3));
    }
}
