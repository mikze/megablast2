public static class PlayerManager
{
    public static Player[] Players { get; set; } =
    {
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false},
        new Player(){ Id = string.Empty, Live = false}
    };

    public static Player? AddPlayer(string id)
    {
        if (GetFreePlayerSlotNumber(out int idPlayer))
        {
            Player? newPlayer = null;

            switch(idPlayer)
            {
                case 0:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 100 };
                break;
                case 1:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 100 };
                break;
                case 2:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 99 + 14 * 50, PosY = 99 + 13 * 50 };
                break;
                case 3:
                    newPlayer = new Player() { Id = id, Name = "mikze", PosX = 101, PosY = 99 + 13 * 50 };
                break;
            }

            if (newPlayer != null)
            {
                Players[idPlayer] = newPlayer;
                return newPlayer;
            }
        }

        return null;
    }

        public static bool GetFreePlayerSlotNumber(out int number)
    {
        for (int i = 0; i < 4; i++)
        {
            if(Players[i].Live == false)
            {
                number = i;
                return true;
            }
        }
        number = -1;
        return false;
    }
}