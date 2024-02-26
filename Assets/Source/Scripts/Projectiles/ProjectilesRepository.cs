using System;
using System.Collections.Generic;

namespace Projectiles
{
    public class ProjectilesRepository
    {
        private readonly Dictionary<ProjectileType, List<ProjectileBase>> _projectiles;

        public IReadOnlyDictionary<ProjectileType, List<ProjectileBase>> Projectiles => _projectiles;
        public event Action<ProjectileBase> OnProjectileAdd;
        public event Action<ProjectileBase> OnProjectileRemove;

        public ProjectilesRepository()
        {
            _projectiles = new Dictionary<ProjectileType, List<ProjectileBase>>(5);
        }

        public void AddProjectile(ProjectileBase projectile)
        {
            if (!_projectiles.ContainsKey(projectile.ProjectileType))
                _projectiles.Add(projectile.ProjectileType, new List<ProjectileBase>(5));

            projectile.ElementReturnEvent += RemoveProjectile;
            
            _projectiles[projectile.ProjectileType].Add(projectile);
        }

        public TProjectile TryGetProjectile<TProjectile>(ProjectileType projectileType, Predicate<TProjectile> predicate = null, bool remove = false) 
            where TProjectile : ProjectileBase
        {
            if (!_projectiles.TryGetValue(projectileType, out List<ProjectileBase> projectiles))
                return default;

            int index = projectiles.IndexOf(unit => unit.CastPossible<TProjectile>() && (predicate is null || predicate(unit.Cast<TProjectile>())));

            if (index is -1)
                return default;

            TProjectile projectile = projectiles[index].Cast<TProjectile>();

            if (remove)
                projectiles.RemoveAt(index);

            return projectile;
        }

        public void RemoveProjectile(ProjectileBase projectile)
        {
            if (_projectiles.TryGetValue(projectile.ProjectileType, out var projectiles))
                projectiles.Remove(projectile);
        }
    }
}