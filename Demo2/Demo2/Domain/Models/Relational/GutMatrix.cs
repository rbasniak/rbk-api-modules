namespace GCAB.Models.Domain
{
    public class GutMatrix
    {
        public GutMatrix(int gravity, int urgency, int tendency)
        {
            Gravity = gravity;
            Urgency = urgency;
            Tendency = tendency;
        }

        public int Gravity { get; set; }
        public int Urgency { get; set; }
        public int Tendency { get; set; }

        public int Priority => Gravity * Urgency * Tendency;
    }
}