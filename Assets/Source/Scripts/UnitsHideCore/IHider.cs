using System.Collections.Generic;
using UnityEngine;

namespace UnitsHideCore
{
    public interface IHider
    {
        public IReadOnlyList<UnitType> Access { get; }
        public IReadOnlyList<HiderCellBase> HiderCells { get; }
        
        public bool CheckAccess(UnitType unitType);

        public bool HaveFreePlace();
        
        public bool TryHideUnit(IHidableUnit unit);

        public UnitBase ExtractUnit(int index);

        public UnitBase ExtractUnit(int index, Vector3 extractPosition);
    }
}