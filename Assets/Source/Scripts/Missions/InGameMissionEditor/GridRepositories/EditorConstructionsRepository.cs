using BugStrategy.Missions.InGameMissionEditor.EditorConstructions;

namespace BugStrategy.Missions.InGameMissionEditor.GridRepositories
{
    public class EditorConstructionsRepository : GridRepository<EditorConstruction>
    {
        public EditorConstructionsRepository(GridConfig gridConfig) : base(gridConfig) { }
    }
}