namespace OctoGame.LocalPersistentData.GameSpellsAccounts
{
    public class SpellSetting
    {
        public ulong SpellId { get; set; }
        public string SpellNameEn { get; set; }
        public string SpellNameRu { get; set; }

        public int SpellType { get; set; }
        /*
         * 0 = passive
         * 1 = active 
         * 2 = Ultimate
         */
   
        public int SpellTreeNum { get; set; }
        public string SpellTreeString{ get; set; }
        /*
         * 0 = General
         * 1 = AD
         * 2 = DEF
         * 3 = AGI
         * 4 = AP
         */


        public string SpellDescriptionRu { get; set; }
        public string SpellDescriptionEn { get; set; }
        /*
         *  This is description users sees! We will change the way it's implemented, but for now leave it like this
         */

        public string SpellFormula { get; set; }
        /*
         *  general description of the formula, internal usage only ( for us only)
         */
        public int SpellDmgType { get; set; }
        /*
         *  0 = Physic
         *  1 = Magic
         *  2 = true
         *  3 = mix
         */


        public int SpellCd { get; set; }
        /*
         *  how many turns? 
         */

        public int WhereDmg { get; set; }
        /*
             0 = Regular( stamina -> health)
             1 = directly To health
             2 = only to stamina
             */
    }
}