public static class TypeChart
{

    private static float[,] _chart = {
            // 물      불      풀     노말
/*물*/      { 1.0f,   2.0f,   0.5f,   1.0f },
/*불*/      { 0.5f,   1.0f,   2.0f,   1.0f },
/*풀*/      { 2.0f,   0.5f,   1.0f,   1.0f },
/*노말*/    { 1.0f,   1.0f,   1.0f,   1.0f }
    };

    public static float GetMultiplier(PokemonType atk, PokemonType def)
    {
        return _chart[(int)atk, (int)def];
    }
}

public enum PokemonType
{
    Water,
    FIre,
    Grass,
    Normal
}