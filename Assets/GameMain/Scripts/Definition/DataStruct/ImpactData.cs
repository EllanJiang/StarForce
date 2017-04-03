namespace AirForce
{
    public struct ImpactData
    {
        private readonly int m_HP;
        private readonly int m_Attack;

        public ImpactData(int hp, int attack)
        {
            m_HP = hp;
            m_Attack = attack;
        }

        public int HP
        {
            get
            {
                return m_HP;
            }
        }

        public int Attack
        {
            get
            {
                return m_Attack;
            }
        }
    }
}
