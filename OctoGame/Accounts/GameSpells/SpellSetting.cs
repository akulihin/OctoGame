namespace OctoGame.Accounts.GameSpells
{
    public class SpellSetting
    {
        public ulong SpellId { get; set; }
        public string SpellName { get; set; }

        public int ActiveOrPassive { get; set; }

        public int SpellTree { get; set; }

        // 1 - AD
        // 2 - DEF
        // 3 - AGI
        // 4 - AP
        //
        public string SpellDescriptionRu { get; set; }
        public string SpellDescriptionEn { get; set; }
        public string SpellFormula { get; set; }
        public string SpellDmgType { get; set; }
        public int SpellCd { get; set; }
        public string Onhit { get; set; }
        public string Poisen { get; set; }
        public string Buff { get; set; }
        public string DeBuff { get; set; }

        public int WhereDmg { get; set; }
        /*
             0 = Regular
             1 = To health
             2 = only to stamina
             */
    }
}