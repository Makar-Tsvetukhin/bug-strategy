namespace Unit.Effects.Interfaces
{
    public interface IStickyHoneyEffectable
    {
        public bool IsSticky { get; }

        public void SwitchSticky(bool isSticky);
    }
}