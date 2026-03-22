using System.Reflection.Metadata.Ecma335;

public static class PokemonList
{

    public static Pokemon bugi = new Pokemon("꼬부기", PokemonType.Water, 18, 5, 3, 1, 2, SkillChart.normals[0], SkillChart.normals[1], SkillChart.waters[1], SkillChart.waters[2]);
    public static Pokemon sseed = new Pokemon("이상해씨", PokemonType.Grass, 20, 5, 2, 2, 2, SkillChart.normals[0], SkillChart.normals[2], SkillChart.grasses[0], SkillChart.grasses[2]);
    public static Pokemon pairi = new Pokemon("파이리", PokemonType.FIre, 16, 5, 3, 2, 3, SkillChart.normals[1], SkillChart.normals[2], SkillChart.fires[1], SkillChart.fires[2]);
    public static Pokemon purin = new Pokemon("푸린", PokemonType.Normal, 22, 5, 1, 2, 1, SkillChart.normals[0], SkillChart.normals[1], SkillChart.normals[2], SkillChart.normals[3]);
    public static Pokemon eevee = new Pokemon("이브이", PokemonType.Normal, 17, 5, 2, 2, 3, SkillChart.normals[0], SkillChart.normals[1], SkillChart.normals[2], SkillChart.normals[3]);

    public static Pokemon[] GetPokemons1()
    {
        var purin = new Pokemon("푸린", PokemonType.Normal, 20, 4, 1, 2, 1);
        var duck = new Pokemon("고라파덕", PokemonType.Water, 18, 4, 2, 1, 1);
        var bugi = new Pokemon("꼬부기", PokemonType.Water, 20, 5, 3, 1, 2);

        purin.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.normals[0], SkillChart.normals[1]);
        duck.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.waters[0], SkillChart.waters[1]);
        bugi.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.waters[1], SkillChart.waters[2]);

        Pokemon[] list = { purin, duck, bugi };

        return list;
    }
    public static Pokemon[] GetPokemons2()
    {
        var eevee = new Pokemon("이브이", PokemonType.Normal, 23, 6, 3, 2, 3);
        var modapi = new Pokemon("모다피", PokemonType.Grass, 25, 6, 3, 2, 3);
        var sseed = new Pokemon("이상해씨", PokemonType.Grass, 27, 7, 5, 3, 3);

        eevee.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.normals[0], SkillChart.normals[2]);
        modapi.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.grasses[0], SkillChart.grasses[1]);
        sseed.GetSkills(SkillChart.normals[1], SkillChart.normals[2], SkillChart.grasses[1], SkillChart.grasses[2]);

        Pokemon[] list = { eevee, modapi, sseed };

        return list;
    }
    public static Pokemon[] GetPokemons3()
    {
        var naong = new Pokemon("나옹", PokemonType.Normal, 29, 8, 7, 5, 4);
        var gadi = new Pokemon("가디", PokemonType.FIre, 32, 9, 9, 4, 6);
        var pairi = new Pokemon("파이리", PokemonType.FIre, 34, 10, 7, 5, 6);

        naong.GetSkills(SkillChart.normals[0], SkillChart.normals[1], SkillChart.normals[1], SkillChart.normals[2]);
        gadi.GetSkills(SkillChart.normals[1], SkillChart.normals[1], SkillChart.fires[0], SkillChart.fires[1]);
        pairi.GetSkills(SkillChart.normals[1], SkillChart.normals[2], SkillChart.fires[1], SkillChart.fires[2]);

        Pokemon[] list = { naong, gadi, pairi };

        return list;
    }
}