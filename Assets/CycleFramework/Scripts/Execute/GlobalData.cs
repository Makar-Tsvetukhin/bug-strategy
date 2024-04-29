﻿using System;
using PoisonFog;
using Projectiles;

[Serializable]
public class GlobalData
{
    public readonly UnitRepository UnitRepository;
    public readonly ProjectilesRepository ProjectilesRepository;
    public readonly PoisonFogsRepository PoisonFogsRepository;
    public readonly ResourceRepository ResourceRepository;
    public readonly ConstructionsRepository ConstructionsRepository;
    public readonly ConstructionSelector ConstructionSelector;
    public readonly ResourceSourcesRepository ResourceSourcesRepository;
    
    public GlobalData()
    {
        UnitRepository = new UnitRepository();
        ProjectilesRepository = new ProjectilesRepository();
        PoisonFogsRepository = new PoisonFogsRepository();
        ResourceRepository = new ResourceRepository();
        ConstructionsRepository = new ConstructionsRepository();
        ConstructionSelector = new ConstructionSelector(ConstructionsRepository);
        ResourceSourcesRepository = new ResourceSourcesRepository();
    }
}
